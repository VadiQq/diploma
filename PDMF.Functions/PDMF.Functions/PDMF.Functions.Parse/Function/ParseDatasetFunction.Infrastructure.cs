using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PDMF.Data.Azure;
using PDMF.Data.Azure.Abstract;
using PDMF.Data.Azure.Models.QueueMessages;
using PDMF.Data.Entities;
using PDMF.Data.Enums;
using PDMF.Data.Repositories;
using PDMF.Parsing.Models;
using PDMF.Parsing.Services.Abstract;
using TaskStatus = PDMF.Data.Enums.TaskStatus;

namespace PDMF.Functions.Parse.Function
{
    public partial class ParseDatasetFunction
    {
        private readonly ParseTaskRepository _parseTaskRepository;
        private readonly DatasetRepository _datasetRepository;
        private readonly DatasetBlobClient _datasetBlobClient;
        private readonly ParseBlobClient _parseBlobClient;
        private readonly ForecastBlobClient _forecastBlobClient;
        private readonly ForecastTaskRepository _forecastTaskRepository;
        private readonly IAzureQueueClient<ForecastingQueueMessage> _queueClient;
        private readonly IDatasetParser _datasetParser;
        
        public ParseDatasetFunction(
            DatasetRepository datasetRepository,
            ParseTaskRepository parseTaskRepository,
            DatasetBlobClient datasetBlobClient,
            ParseBlobClient parseBlobClient,
            IDatasetParser datasetParser,
            ForecastBlobClient forecastBlobClient, 
            IAzureQueueClient<ForecastingQueueMessage> queueClient,
            ForecastTaskRepository forecastTaskRepository)
        {
            _datasetRepository = datasetRepository;
            _parseTaskRepository = parseTaskRepository;
            _datasetBlobClient = datasetBlobClient;
            _datasetParser = datasetParser;
            _forecastBlobClient = forecastBlobClient;
            _forecastTaskRepository = forecastTaskRepository;
            _queueClient = queueClient;
            _parseBlobClient = parseBlobClient;
        }

        private async Task ProcessMessage(ParseQueueMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException();
            }

            var task = await _parseTaskRepository.Get(message.TaskId);

            if (task == null)
            {
                throw new ArgumentException($"Can not find task: {message.TaskId}");
            }

            try
            {
                task.Status = TaskStatus.Processing;

                await _parseTaskRepository.Update(task);
                await _parseTaskRepository.Commit();

                switch (task.TaskType)
                {
                    case ParseTaskType.Default:
                    {
                        await ParseDataset(task, message);
                        break;
                    }
                    case ParseTaskType.Forecast:
                    {
                        await ParseForecastDataset(message);
                        break;
                    }
                    default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(task.TaskType), task.TaskType, null);
                    }
                }
                
                task.Status = TaskStatus.Complete;
                task.ResultDescription = "Dataset successfully parsed.";
                task.CompleteDate = DateTime.UtcNow;

                await _parseTaskRepository.Update(task);
                await _parseTaskRepository.Commit();
            }
            catch (Exception exception)
            {
                await _parseBlobClient.DeleteBlobIfExists(task.Id);

                task.Status = TaskStatus.Failed;
                task.ResultDescription =
                    "Unable to parse dataset. " +
                    "Please, ensure that you set correct data separator and dataset data is valid.";
                await _parseTaskRepository.Update(task);
                await _parseTaskRepository.Commit();
                
                throw;
            }
        }
        
        private async Task ParseForecastDataset(ParseQueueMessage message)
        {
            var forecastTask = await _forecastTaskRepository.Get(message.DatasetId);
            forecastTask.Status = TaskStatus.Processing;
            await _forecastTaskRepository.Update(forecastTask);
            await _forecastTaskRepository.Commit();
            
            var datasetFile = await _forecastBlobClient.Get(message.DatasetId);

            var forecastDataset = await _datasetRepository
                .DatasetsContextAccess
                .FirstOrDefaultAsync(dataset => dataset.Name == forecastTask.Id);
                
            var parseResult = await _datasetParser.Parse(
                datasetFile,
                new ParseOptions
                {
                    Mode = message.Mode,
                    DataSeparator = message.DataSeparator
                });

            var data = JsonConvert.SerializeObject(parseResult.Values);

            await using (var memoryStream = new MemoryStream(Encoding.Default.GetBytes(data)))
            {
                memoryStream.Position = 0;
                await _parseBlobClient.Insert(message.DatasetId, memoryStream);
            }
            
            forecastTask.Status = TaskStatus.WaitingForNextStep;
            forecastDataset.Status = DatasetStatus.Parsed;
               
            await _datasetRepository.Update(forecastDataset);
            await _forecastTaskRepository.Update(forecastTask);
            await _forecastTaskRepository.Commit();
            
            await _queueClient.AddMessage(new ForecastingQueueMessage
            {
                Mode = forecastTask.Mode,
                ForecastTaskId = forecastTask.Id,
                ModelingResultId = forecastTask.ModelingResultId
            });
        }
        
        private async Task ParseDataset(ParseTask task, ParseQueueMessage message)
        {
            var dataset = await _datasetRepository.Get(message.DatasetId);

            if (dataset == null)
            {
                throw new ArgumentException($"Can not find dataset: {message.DatasetId}");
            }
            
            var datasetFile = await _datasetBlobClient.Get(dataset.Id);

            var parseResult = await _datasetParser.Parse(
                datasetFile,
                new ParseOptions
                {
                    Mode = message.Mode,
                    DataSeparator = message.DataSeparator
                });

            dataset.Columns = JsonConvert.SerializeObject(parseResult.Headers);
            dataset.Status = DatasetStatus.Parsed;

            var data = JsonConvert.SerializeObject(parseResult.Values);

            await using (var memoryStream = new MemoryStream(Encoding.Default.GetBytes(data)))
            {
                memoryStream.Position = 0;
                await _parseBlobClient.Insert(task.Id, memoryStream);
            } 
            
            await _datasetRepository.Update(dataset);
        }
    }
}