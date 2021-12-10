using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using PDMF.Data.Azure;
using PDMF.Data.Azure.Abstract;
using PDMF.Data.Azure.Base;
using PDMF.Data.Azure.Models;
using PDMF.Data.Azure.Models.QueueMessages;
using PDMF.Data.Entities;
using PDMF.Data.Enums;
using PDMF.Data.Repositories;
using PDMF.Data.Utilities.Configuration;
using PDMF.Functions.Modeling.Models;
using PDMF.Functions.Modeling.Models.Enums;
using PDMF.Functions.Modeling.Services;
using Serilog;
using TaskStatus = PDMF.Data.Enums.TaskStatus;

namespace PDMF.Functions.Modeling.Function
{
    public partial class ModelingFunction
    {
        private const string ModelingStorageConnectionName = "AzureStorageConnection";

        private readonly ModelingTaskRepository _modelingTaskRepository;
        private readonly ModelingResultRepository _modelingResultRepository;
        private readonly ParseTaskRepository _parseTaskRepository;
        private readonly DatasetRepository _datasetRepository;
        private readonly ParseBlobClient _parseBlobClient;
        private readonly IAzureQueueClient<ModelingMonitoringQueueMessage> _monitoringQueueClient;
        private readonly IAzureQueueClient<ModelingCleanupQueueMessage> _cleanupQueueClient;

        public ModelingFunction(
            ModelingTaskRepository modelingTaskRepository,
            DatasetRepository datasetRepository,
            ParseBlobClient parseBlobClient,
            ParseTaskRepository parseTaskRepository,
            IAzureQueueClient<ModelingMonitoringQueueMessage> monitoringQueueClient,
            IAzureQueueClient<ModelingCleanupQueueMessage> cleanupQueueClient, 
            ModelingResultRepository modelingResultRepository)
        {
            _modelingTaskRepository = modelingTaskRepository;
            _datasetRepository = datasetRepository;
            _parseBlobClient = parseBlobClient;
            _parseTaskRepository = parseTaskRepository;
            _monitoringQueueClient = monitoringQueueClient;
            _cleanupQueueClient = cleanupQueueClient;
            _modelingResultRepository = modelingResultRepository;
        }

        #region Batch

        private async Task ProcessMessage(ModelingQueueMessage message)
        {
            Log.Information($"Modeling task {message.ModelingTaskId} execution began.");
            
            if (message == null)
            {
                throw new ArgumentException("Modeling queue message is empty.");
            }
            
            var data = await ValidateModelingMessage(message);
            
            if(data.Task.Status == TaskStatus.Failed)
            {
                return;
            }

            try
            {
                var parseTask = await _parseTaskRepository.ParseTasksContextAccess
                    .OrderByDescending(task => task.CompleteDate)
                    .FirstOrDefaultAsync(task => task.Status == TaskStatus.Complete && task.DatasetId == message.DatasetId);

                if (parseTask == null)
                {
                    throw new ArgumentException($"Unable to find complete parse for modeling. {data.Dataset.Id}");
                }

                var parseExists = await _parseBlobClient.Exists(parseTask.Id);

                if (!parseExists)
                {
                    throw new ArgumentException($"Unable to find complete parse file for modeling. {parseTask.Id}");
                }

                data.Task.Status = TaskStatus.WaitingForNextStep;

                await _modelingTaskRepository.Update(data.Task);
                await _modelingTaskRepository.Commit();
            
                var batchClient = new AzureBatchClient(
                    new BlobServiceClient(Configurations.GetSetting("AzureStorageConnection")),
                    new AzureBatchConfiguration {JobId = message.ModelingTaskId, PoolId = message.ModelingTaskId});

                await batchClient.StartBatchAsync(ProgramDefiner.GetProgram(message));

                await AddPollingMessage(PollingAction.Wait, new ModelingMonitoringQueueMessage
                {
                    DatasetId = message.DatasetId,
                    ModelingTaskId = message.ModelingTaskId
                });
            }
            catch (Exception e)
            {
                Log.Error($"Error during modeling task execution: {e.Message}. Stack trace: {e.StackTrace}");
                
                data.Task.Status = TaskStatus.Failed;
                data.Task.CompleteDate = DateTime.UtcNow;
                await _modelingTaskRepository.Update(data.Task);
                await _modelingTaskRepository.Commit();
                throw;
            }
        }

        #endregion

        #region Monitoring

        private async Task ProcessMessage(ModelingMonitoringQueueMessage message)
        {
            var blobServiceClient = new BlobServiceClient(Configurations.GetSetting(ModelingStorageConnectionName));

            var service = new AzureBatchClient(
                blobServiceClient,
                new AzureBatchConfiguration
                {
                    JobId = message.ModelingTaskId,
                    PoolId = message.ModelingTaskId
                });

            var result = await service.MonitorTaskAsync();

            if (result)
            {
                await AddPollingMessage(PollingAction.End, message);
                return;
            }

            await AddPollingMessage(PollingAction.Wait, message);
        }

        #endregion

        #region Cleanup

        private async Task ProcessMessage(ModelingCleanupQueueMessage message)
        {
            
            var blobServiceClient = new BlobServiceClient(Configurations.GetSetting(ModelingStorageConnectionName));

            var service = new AzureBatchClient(
                blobServiceClient,
                new AzureBatchConfiguration
                {
                    JobId = message.ModelingTaskId,
                    PoolId = message.ModelingTaskId
                });
            
            try
            {
                var modelingTask = await _modelingTaskRepository.Get(message.ModelingTaskId);

                var modelingResult = await _modelingResultRepository
                    .ModelingResultsContextAccess
                    .FirstOrDefaultAsync(result =>
                        result.Status == modelingTask.Status && result.TaskId == message.ModelingTaskId);

                if (modelingTask.Status != TaskStatus.Failed)
                {
                    modelingTask.Status = TaskStatus.Complete;
                    modelingTask.CompleteDate = DateTime.UtcNow;
                }
                
                modelingResult.Status = modelingTask.Status;

                await _modelingResultRepository.Update(modelingResult);
                await _modelingTaskRepository.Update(modelingTask);
                await _modelingTaskRepository.Commit();

                //await service.GetLogAsync(modelingResult.Id);
            }
            finally
            {
                await service.CleanUpBatchAsync();
            }
        }

        #endregion

        #region private methods

        private async Task AddPollingMessage(PollingAction action, ModelingMonitoringQueueMessage message)
        {
            try
            {
                switch (action)
                {
                    case PollingAction.End:
                    {
                        await _cleanupQueueClient.AddMessage(new ModelingCleanupQueueMessage
                        {
                            DatasetId = message.DatasetId,
                            ModelingTaskId = message.ModelingTaskId
                        });
                        break;
                    }
                    default:
                    {
                        await _monitoringQueueClient.AddMessage(message);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private async Task<ModelingData> ValidateModelingMessage(ModelingQueueMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException();
            }

            var task = await _modelingTaskRepository.Get(message.ModelingTaskId);

            if (task == null)
            {
                throw new ArgumentException($"Can not find task: {message.ModelingTaskId}");
            }

            var dataset = await _datasetRepository.Get(message.DatasetId);

            if (dataset == null)
            {
                throw new ArgumentException($"Can not find dataset: {message.DatasetId}");
            }

            return new ModelingData
            {
                Dataset = dataset,
                Task = task
            };
        }

        #endregion
    }
}