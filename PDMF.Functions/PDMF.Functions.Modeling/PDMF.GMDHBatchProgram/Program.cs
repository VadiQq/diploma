using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PDMF.Algorithms.GMDH.Method;
using PDMF.Data.Algorithms.Matrices;
using PDMF.Data.Azure;
using PDMF.Data.Azure.Models;
using PDMF.Data.Contexts;
using PDMF.Data.Entities;
using PDMF.Data.Repositories;
using PDMF.Data.Utilities.Configuration;
using PDMF.Data.Utilities.Configuration.Managers;
using PDMF.GMDHBatchProgram.Configuration;
using TaskStatus = PDMF.Data.Enums.TaskStatus;

namespace PDMF.GMDHBatchProgram
{
    public class Program
    {
        public static PDMFDatabaseContext Context;
        public static ParseBlobClient ParseBlobClient;
        public static ModelingResultBlobClient ModelingBlobClient;
        public static ModelingTaskRepository ModelingTaskRepository;
        public static ParseTaskRepository ParseTaskRepository;
        public static ModelingResultRepository ModelingResultRepository;
        
        //args[0] - desired column
        //args[1] - dataset id
        //args[2] - modeling task id
        static async Task Main(string[] args)
        {
            Configurations.InitializeConfigurationManager(new StaticConfigurationManager(StaticSettings.Settings));
            
            /*args = new[]
            {
                "1",
                "7b5a52d3-aba4-47d3-8242-603fbc169a30",
                ""
            };*/

            try
            {
                if (!int.TryParse(args[0], out var desiredColumn))
                {
                    throw new ArgumentException("Invalid program parameter");
                }

                SetServices();

                var parseTask = await ParseTaskRepository.ParseTasksContextAccess
                    .OrderByDescending(task => task.CompleteDate)
                    .FirstOrDefaultAsync(task => task.Status == TaskStatus.Complete && task.DatasetId == args[1]);

                if (parseTask == null)
                {
                    throw new ArgumentException("Unable to find parse complete for modeling.");
                }

                var modelingTask = await ModelingTaskRepository.Get(args[2]);

                if (modelingTask == null)
                {
                    throw new ArgumentException("Unable to find modeling task");
                }

                Console.WriteLine($"Container name: {ModelingBlobClient.GetContainerName()}");
                
                modelingTask.Status = TaskStatus.Processing;

                await ModelingTaskRepository.Update(modelingTask);
                await ModelingTaskRepository.Commit();

                var parseStream = await ParseBlobClient.Get(parseTask.Id);

                Matrix parse;

                using (var reader = new StreamReader(parseStream, Encoding.UTF8))
                {
                    parse = JsonConvert.DeserializeObject<Matrix>(await reader.ReadToEndAsync());
                }

                Console.WriteLine($"Number of processes: {Environment.ProcessorCount}");
                
                var gmdh = new GMDH();
                var parseModel = gmdh.CreateModel(desiredColumn, parse, Environment.ProcessorCount);

                var modelingTaskResult = new ModelingResult
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = modelingTask.UserId,
                    ModelType = modelingTask.ModelType,
                    CreateDate = DateTime.UtcNow,
                    TaskId = modelingTask.Id
                };
                
                var data = JsonConvert.SerializeObject(parseModel);
                
                await using (var memoryStream = new MemoryStream(Encoding.Default.GetBytes(data)))
                {
                    memoryStream.Position = 0;
                    await ModelingBlobClient.InsertWithFolder(modelingTaskResult.Id, modelingTaskResult.Id, memoryStream);
                }

                modelingTask.Status = TaskStatus.Finishing;
                modelingTaskResult.Status = modelingTask.Status;
                
                await ModelingResultRepository.Create(modelingTaskResult);
                await ModelingTaskRepository.Update(modelingTask);
                await ModelingTaskRepository.Commit();
            }
            catch (Exception exception)
            {
                var modelingTask = await ModelingTaskRepository.Get(args[2]);

                if (modelingTask != null)
                {
                    modelingTask.Status = TaskStatus.Failed;
                    modelingTask.CompleteDate = DateTime.UtcNow;

                    var modelingResult = await ModelingResultRepository
                        .ModelingResultsContextAccess
                        .FirstOrDefaultAsync(result => result.TaskId == modelingTask.Id);

                    if (modelingResult != null)
                    {
                        modelingResult.Status = modelingTask.Status;
                        await ModelingBlobClient.DeleteBlobIfExists(modelingResult.Id);
                    }
                    
                    await ModelingTaskRepository.Update(modelingTask);
                    await ModelingResultRepository.Update(modelingResult);
                    await ModelingTaskRepository.Commit();
                }
            }
        }

        private static void SetServices()
        {
            Context = new PDMFDatabaseContext();
            ParseBlobClient = new ParseBlobClient(new AzureBlobClientConfiguration<ParseTask>()
            {
                ConnectionString = Configurations.GetSetting("AzureStorageConnection")
            });
            ModelingBlobClient = new ModelingResultBlobClient(new AzureBlobClientConfiguration<ModelingResult>()
            {
                ConnectionString = Configurations.GetSetting("AzureStorageConnection")
            });
            ParseTaskRepository = new ParseTaskRepository(Context);
            ModelingTaskRepository = new ModelingTaskRepository(Context);
            ModelingResultRepository = new ModelingResultRepository(Context);
        }
    }
}