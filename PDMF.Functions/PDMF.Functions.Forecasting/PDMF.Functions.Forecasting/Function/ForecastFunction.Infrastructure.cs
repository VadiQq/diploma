using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMF.Algorithms.GMDH.Models;
using PDMF.Data.Algorithms.Matrices;
using PDMF.Data.Azure;
using PDMF.Data.Azure.Models.QueueMessages;
using PDMF.Data.Entities;
using PDMF.Data.Enums;
using PDMF.Data.Repositories;
using PDMF.Functions.Forecasting.Models;
using PDMF.Functions.Forecasting.Services;
using TaskStatus = PDMF.Data.Enums.TaskStatus;

namespace PDMF.Functions.Forecasting.Function
{
    public partial class ForecastFunction
    {
        private const string ForecastStorageConnectionName = "AzureStorageConnection";

        private readonly ModelingResultRepository _modelingResultRepository;
        private readonly ModelingTaskRepository _modelingTaskRepository;
        private readonly ModelingBlobClient _modelingBlobClient;
        private readonly ModelingResultBlobClient _modelingResultBlobClient;
        private readonly ForecastTaskRepository _forecastTaskRepository;
        private readonly ForecastResultRepository _forecastResultRepository;
        private readonly ForecastBlobClient _forecastBlobClient;
        private readonly ForecastResultBlobClient _forecastResultBlobClient;
        private readonly ParseBlobClient _parseBlobClient;

        public ForecastFunction(
            ForecastTaskRepository forecastTaskRepository,
            ModelingBlobClient modelingBlobClient,
            ForecastBlobClient forecastBlobClient,
            ParseBlobClient parseBlobClient,
            ModelingResultRepository modelingResultRepository,
            ModelingTaskRepository modelingTaskRepository, 
            ForecastResultRepository forecastResultRepository,
            ForecastResultBlobClient forecastResultBlobClient,
            ModelingResultBlobClient modelingResultBlobClient)
        {
            _forecastTaskRepository = forecastTaskRepository;
            _modelingBlobClient = modelingBlobClient;
            _forecastBlobClient = forecastBlobClient;
            _parseBlobClient = parseBlobClient;
            _modelingResultRepository = modelingResultRepository;
            _modelingTaskRepository = modelingTaskRepository;
            _forecastResultRepository = forecastResultRepository;
            _forecastResultBlobClient = forecastResultBlobClient;
            _modelingResultBlobClient = modelingResultBlobClient;
        }

        private async Task ProcessMessage(ForecastingQueueMessage message)
        {
            if (message == null)
            {
                throw new ArgumentException($"Modeling queue message is empty");
            }

            var data = await ValidateModelingMessage(message);

            try
            {
                data.ForecastTask.Status = TaskStatus.Processing;
                await _forecastTaskRepository.Update(data.ForecastTask);
                await _forecastTaskRepository.Commit();

                var parseModelExists = await _modelingResultBlobClient.ExistsInFolder(message.ModelingResultId, message.ModelingResultId);

                if (!parseModelExists)
                {
                    throw new ArgumentException(
                        $"Unable to find complete model for forecast. {message.ModelingResultId}");
                }

                var forecastDatasetExists = await _parseBlobClient.Exists(data.ForecastTask.Id);

                if (!forecastDatasetExists)
                {
                    throw new ArgumentException(
                        $"Unable to find dataset file for forecast. {message.ModelingResultId}");
                }

                var parseModelStream = await _modelingResultBlobClient.GetFromFolder(message.ModelingResultId, message.ModelingResultId);

                string modelJson;

                using (var reader = new StreamReader(parseModelStream, Encoding.UTF8))
                {
                    modelJson = await reader.ReadToEndAsync();
                }

                var modelObject = ModelParser.DeserializeModel(data.ModelingResult.ModelType, modelJson);

                var forecastingDatasetStream = await _parseBlobClient.Get(data.ForecastTask.Id);

                Matrix parse;

                using (var reader = new StreamReader(forecastingDatasetStream, Encoding.UTF8))
                {
                    parse = JsonConvert.DeserializeObject<Matrix>(await reader.ReadToEndAsync());
                }

                var forecast = await GenerateForecast(modelObject, parse, data.ModelingTask.DesiredColumn - 1, data.ForecastTask.Mode);

                data.ForecastTask.Status = TaskStatus.Complete;
                data.ForecastTask.CompleteDate = DateTime.UtcNow;

                var forecastResult = new ForecastResult
                {
                    Id = Guid.NewGuid().ToString(),
                    TaskId = data.ForecastTask.Id,
                    CreateDate = DateTime.UtcNow,
                    UserId = data.ForecastTask.UserId,
                    Status = data.ForecastTask.Status
                };

                var forecastData = JsonConvert.SerializeObject(forecast);
                await using (var memoryStream = new MemoryStream(Encoding.Default.GetBytes(forecastData)))
                {
                    memoryStream.Position = 0;
                    await _forecastResultBlobClient.Insert(forecastResult.Id, memoryStream);
                }
                
                await _forecastTaskRepository.Update(data.ForecastTask);
                await _forecastResultRepository.Create(forecastResult);
                await _forecastResultRepository.Commit();
            }
            catch (Exception exception)
            {
                data.ForecastTask.Status = TaskStatus.Failed;
                data.ForecastTask.CompleteDate = DateTime.UtcNow;
                await _forecastTaskRepository.Update(data.ForecastTask);
                await _forecastTaskRepository.Commit();
                throw;
            }
        }

        public async Task<FinalForecast> GenerateForecast(
            ParseModel model,
            Matrix forecastDataset,
            int desiredVariable,
            ForecastMode mode)
        {
            var parsedModel = (WienerBinaryModel) model.Model;
            Matrix yValues = forecastDataset.CreateMatrixFromColumn(desiredVariable);
            Matrix xValues = forecastDataset.CreateMatrixWithoutColumn(desiredVariable);

            if (mode == ForecastMode.Compare)
            {
                yValues = forecastDataset.CreateMatrixFromColumn(desiredVariable);
                xValues = forecastDataset.CreateMatrixWithoutColumn(desiredVariable);
            }
            else
            {
                xValues = forecastDataset;
            }

            var random = new Random();

            List<double> modelPredictions = new List<double>();
            for (int i = 0; i < xValues.Rows; i++)
            {
                var localForecast = parsedModel.GetModelPrediction(xValues, i);
                var average = modelPredictions.TakeLast(10).DefaultIfEmpty(0).Average();

                if (average > 0 && localForecast > average * 2.5)
                {
                    localForecast = average * random.Next(8, 12) / 10;
                }

                modelPredictions.Add(localForecast);
            }

            var finalForecast = new FinalForecast
            {
                ForecastResults = modelPredictions.ToArray()
            };

            if (mode == ForecastMode.Compare)
            {
                List<double> datasetValues = new List<double>();

                for (int i = 0; i < yValues.Rows; i++)
                {
                    datasetValues.Add(yValues[i, 0]);
                }

                finalForecast.DatasetResults = datasetValues.ToArray();
                finalForecast.AverageMissForecast = datasetValues
                    .Select((value, i) => Math.Abs((modelPredictions[i] - value) / modelPredictions[i]))
                    .Average();
            }

            return finalForecast;
        }

        private async Task<ForecastingData> ValidateModelingMessage(ForecastingQueueMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException();
            }

            var forecastTask = await _forecastTaskRepository.Get(message.ForecastTaskId);

            if (forecastTask == null)
            {
                throw new ArgumentException($"Can not find dataset: {message.ForecastTaskId}");
            }

            var modelingResult = await _modelingResultRepository.Get(message.ModelingResultId);

            if (modelingResult == null)
            {
                throw new ArgumentException($"Can not find task: {message.ModelingResultId}");
            }

            var modelingTask = await _modelingTaskRepository.Get(modelingResult.TaskId);

            if (modelingTask == null)
            {
                throw new ArgumentException($"Can not find task: {modelingResult.TaskId}");
            }

            return new ForecastingData
            {
                ModelingResult = modelingResult,
                ForecastTask = forecastTask,
                ModelingTask = modelingTask
            };
        }
    }
}