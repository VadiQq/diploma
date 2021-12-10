using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Auth;
using Microsoft.Azure.Batch.Common;
using PDMF.Data.Azure.Models;
using PDMF.Data.Utilities;
using PDMF.Data.Utilities.Configuration;

namespace PDMF.Data.Azure.Base
{
    public class AzureBatchClient
    {
        private readonly string _accountName;
        private readonly string _accountKey;
        private readonly string _accountUrl;
        private readonly BlobServiceClient _blobServiceClient;
        
        private string PoolId { get; set; }
        private string JobId { get; set; }
        
        private const string AppContainerName = "application";
        private const string VmSize = "Standard_D2_v3"; // Standard_D2
        private const int NodeCount = 1;

        public AzureBatchClient(BlobServiceClient blobServiceClient, AzureBatchConfiguration configuration)
        {
            _accountName = Configurations.GetSetting("BatchAccountName");
            _accountKey = Configurations.GetSetting("BatchAccountKey");
            _accountUrl = Configurations.GetSetting("BatchAccountUrl");
            PoolId = configuration.PoolId;
            JobId = configuration.JobId;
            _blobServiceClient = blobServiceClient;
        }
        
        public async Task StartBatchAsync(BatchProgramSettings settings)
        {
            var path = VirtualPathMapper.MapToPhysicalPath(settings.ProgramPath);
            var applicationFilePaths = Directory.GetFiles(path);

            await UploadFilesToContainerAsync(applicationFilePaths.ToList(), settings);
            var appContainerReference = GetContainerReference(settings);
            
            BatchSharedKeyCredentials cred = new BatchSharedKeyCredentials(_accountUrl, _accountName, _accountKey);
            
            using BatchClient batchClient = BatchClient.Open(cred);
            
            await CreatePoolIfNotExistAsync(batchClient, PoolId, appContainerReference);
            await CreateJobAsync(batchClient, JobId, PoolId);
            await AddTasksAsync(batchClient, JobId, settings);
        }

        private async Task UploadFilesToContainerAsync(List<string> filePaths, BatchProgramSettings settings)
        {
            var container = _blobServiceClient.GetBlobContainerClient(settings.ProgramPath.ToLower());

            if (!await container.ExistsAsync())
            {
                await container.CreateIfNotExistsAsync();
            
                foreach (string filePath in filePaths)
                {
                    string blobName = Path.GetFileName(filePath);

                    var blobData = container.GetBlobClient(blobName);

                    await blobData.UploadAsync(filePath);
                }
            }
        }

        private ResourceFile GetContainerReference(BatchProgramSettings settings)
        {
            var container = _blobServiceClient.GetBlobContainerClient(settings.ProgramPath.ToLower());
            var containerReference = ResourceFile.FromAutoStorageContainer(container.Name);
            return containerReference;
        }
        
        private static async Task CreatePoolIfNotExistAsync(
            BatchClient batchClient, 
            string poolId, 
            ResourceFile containerReference)
        {
            try
            {
                ImageReference imageReference = CreateImageReference();
                VirtualMachineConfiguration vmConfiguration = CreateVirtualMachineConfiguration(imageReference);

                CloudPool pool = batchClient.PoolOperations.CreatePool(
                    poolId: poolId,
                    targetDedicatedComputeNodes: NodeCount,
                    virtualMachineConfiguration: vmConfiguration,
                    virtualMachineSize: VmSize);

                pool.StartTask = new StartTask
                {
                    CommandLine = "cmd /c (robocopy %AZ_BATCH_TASK_WORKING_DIR% %AZ_BATCH_NODE_SHARED_DIR%) ^& IF %ERRORLEVEL% LEQ 1 exit 0",
                    ResourceFiles = new List<ResourceFile>{containerReference},
                    WaitForSuccess = true
                };

                pool.TaskSlotsPerNode = 1;
                pool.TaskSchedulingPolicy = new TaskSchedulingPolicy(ComputeNodeFillType.Spread);

                await pool.CommitAsync();
            }
            catch (BatchException be)
            {
                // Swallow the specific error code PoolExists since that is expected if the pool already exists
                if (!(be.RequestInformation != null && be.RequestInformation.BatchError != null &&
                    be.RequestInformation.BatchError.Code == BatchErrorCodeStrings.PoolExists))
                {
                    throw; // Any other exception is unexpected
                }
            }
        }

        private static async Task CreateJobAsync(BatchClient batchClient, string jobId, string poolId)
        {
            try
            {
                CloudJob job = batchClient.JobOperations.CreateJob();
                job.Id = jobId;
                job.PoolInformation = new PoolInformation { PoolId = poolId };

                await job.CommitAsync();
            }
            catch(BatchException be)
            {
                // Swallow the specific error code JobExists since that is expected if the job already exists
                if (!(be.RequestInformation != null && be.RequestInformation.BatchError != null &&
                   be.RequestInformation.BatchError.Code == BatchErrorCodeStrings.JobExists))
                {
                    throw; // Any other exception is unexpected
                }
            }
        }

        private static async Task AddTasksAsync(BatchClient batchClient, string jobId, BatchProgramSettings settings)
        {
            List<CloudTask> tasks = new List<CloudTask>();

            for (int i = 1; i <= NodeCount; i++)
            {
                string taskId = $"modelingtask{i}";
                string taskCommandLine =
                    $"cmd /c %AZ_BATCH_NODE_SHARED_DIR%\\{settings.ProgramName}.exe " +
                    $"{settings.DesiredColumn} {settings.DatasetId} {settings.ModelingTaskId}";

                CloudTask task = new CloudTask(taskId, taskCommandLine);
                tasks.Add(task);
            }

            await batchClient.JobOperations.AddTaskAsync(jobId, tasks);
        }

        public async Task<bool> MonitorTaskAsync()
        {
            BatchSharedKeyCredentials cred = new BatchSharedKeyCredentials(_accountUrl, _accountName, _accountKey);
            
            using BatchClient batchClient = BatchClient.Open(cred);
            
            ODATADetailLevel detail = new ODATADetailLevel(selectClause: "id");
            List<CloudTask> tasks = await batchClient.JobOperations.ListTasks(JobId, detail).ToListAsync();

            if (tasks.Count == 0)
            {
                return true;
            }

            TaskStateMonitor taskStateMonitor = batchClient.Utilities.CreateTaskStateMonitor();

            try
            {
                // wait 300sec
                await taskStateMonitor.WhenAll(tasks, TaskState.Completed, TimeSpan.FromMinutes(5));
            }
            catch (TimeoutException)
            {
                return false;
            }

            await batchClient.JobOperations.TerminateJobAsync(JobId, "All tasks reached state Completed.");

            return true;
        }

        public async Task<bool> GetFailedTaskAsync()
        {
            BatchSharedKeyCredentials cred = new BatchSharedKeyCredentials(_accountUrl, _accountName, _accountKey);
            using BatchClient batchClient = BatchClient.Open(cred);
            
            ODATADetailLevel detail = new ODATADetailLevel(selectClause: "id, executionInfo");
            List<CloudTask> tasks = await batchClient.JobOperations.ListTasks(JobId, detail).ToListAsync();

            foreach (CloudTask task in tasks)
            {
                await task.RefreshAsync(detail);

                if (task.ExecutionInformation.Result == TaskExecutionResult.Failure)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task GetLogAsync(string modelingResultId)
        {
            var container = _blobServiceClient.GetBlobContainerClient($"modeling-result/{modelingResultId}");
            await container.CreateIfNotExistsAsync();

            BatchSharedKeyCredentials cred = new BatchSharedKeyCredentials(_accountUrl, _accountName, _accountKey);
            
            using BatchClient batchClient = BatchClient.Open(cred);
            
            IPagedEnumerable<CloudTask> tasks = batchClient.JobOperations.ListTasks(JobId);
            foreach (var task in tasks)
            {
                IPagedEnumerable<NodeFile> files = task.ListNodeFiles(true);
                List<NodeFile> results = await files.ToListAsync();

                foreach (var nodeFile in results.Where(x => x.IsDirectory == false))
                {
                    if (nodeFile.Path.Equals(Constants.StandardOutFileName))
                        await UploadFromStreamAsync(Constants.StandardOutFileName);

                    async Task UploadFromStreamAsync(string fileName)
                    {
                        var blobName = $"{task.Id}-{fileName}";
                        
                        var blob = container.GetBlockBlobClient(blobName);
                            
                        string content = await nodeFile.ReadAsStringAsync();

                        await using MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
                        await blob.UploadAsync(ms);
                    }
                }
            }
        }

        public async Task CleanUpBatchAsync()
        {
            var appContainer = _blobServiceClient.GetBlobContainerClient("application");
            await appContainer.DeleteIfExistsAsync();

            BatchSharedKeyCredentials cred = new BatchSharedKeyCredentials(_accountUrl, _accountName, _accountKey);
            using (BatchClient batchClient = BatchClient.Open(cred))
            {
                await batchClient.JobOperations.DeleteJobAsync(JobId);
                await batchClient.PoolOperations.DeletePoolAsync(PoolId);
            }
        }
        
        private static VirtualMachineConfiguration CreateVirtualMachineConfiguration(ImageReference imageReference)
        {
            return new(
                imageReference,
                "batch.node.windows amd64");
        }

        private static ImageReference CreateImageReference()
        {
            return new(
                publisher: "MicrosoftWindowsServer",
                offer: "WindowsServer",
                sku: "2016-datacenter-smalldisk",
                version: "latest");
        }
    }
}