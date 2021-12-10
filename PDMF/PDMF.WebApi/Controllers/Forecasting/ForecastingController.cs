using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PDMF.Algorithms.GMDH.Models;
using PDMF.Data.Azure;
using PDMF.Data.Azure.Abstract;
using PDMF.Data.Azure.Models.QueueMessages;
using PDMF.Data.Entities;
using PDMF.Data.Enums;
using PDMF.Data.Repositories;
using PDMF.WebApi.Controllers.Datasets.Models;
using PDMF.WebApi.Controllers.Forecasting.Models;
using PDMF.WebApi.Models.Identity.Data;
using TaskStatus = PDMF.Data.Enums.TaskStatus;

namespace PDMF.WebApi.Controllers.Forecasting
{
    [Route("api/forecasting")]
    [ApiController]
    public class ForecastingController : ControllerBase
    {
        private readonly ForecastBlobClient _forecastBlobClient;
        private readonly ForecastResultBlobClient _forecastResultBlobClient;
        private readonly ForecastTaskRepository _forecastTaskRepository;
        private readonly ForecastResultRepository _forecastResultRepository;
        private readonly DatasetRepository _datasetRepository;
        private readonly ModelingResultRepository _modelingResultRepository;
        private readonly ParseTaskRepository _parseTaskRepository;
        private readonly IAzureQueueClient<ParseQueueMessage> _queueClient;

        public ForecastingController(
            ForecastTaskRepository forecastTaskRepository, 
            ForecastBlobClient forecastBlobClient,
            IAzureQueueClient<ParseQueueMessage> queueClient,
            ParseTaskRepository parseTaskRepository,
            ModelingResultRepository modelingResultRepository, 
            DatasetRepository datasetRepository,
            ForecastResultRepository forecastResultRepository, 
            ForecastResultBlobClient forecastResultBlobClient)
        {
            _forecastTaskRepository = forecastTaskRepository;
            _queueClient = queueClient;
            _parseTaskRepository = parseTaskRepository;
            _modelingResultRepository = modelingResultRepository;
            _datasetRepository = datasetRepository;
            _forecastResultRepository = forecastResultRepository;
            _forecastResultBlobClient = forecastResultBlobClient;
            _forecastBlobClient = forecastBlobClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader);
            
            var token = JWTHelper.GetJWTFromHeader(authHeader);
            var userId = JWTHelper.GetJWTValue(token, ClaimsTypes.Id);

            var forecastingTasks = await _forecastTaskRepository.ForecastTasksContextAccess
                .Include(task => task.ModelingTask)
                .ThenInclude(modelingTask => modelingTask.Dataset)
                .Where(dataset => dataset.UserId == userId)
                .ToArrayAsync();

            var taskList = forecastingTasks.Select(task => new ForecastListModel
            {
                Id = task.Id,
                ModelingResultId = task.ModelingResultId,
                ModelingTaskId = task.ModelingTaskId,
                DatasetName = task.ModelingTask.Dataset.Name,
                DesiredColumn = JsonConvert.DeserializeObject<string[]>(task.ModelingTask.Dataset.Columns)
                    .Select((value, index) => new DatasetColumn()
                    {
                        Index = index,
                        Name = value
                    }).ToArray()[task.ModelingTask.DesiredColumn - 1]
                    .Name,
                CompleteDate = task.CompleteDate,
                CreateDate = task.CreateDate,
                Mode = task.Mode,
                Status = task.Status
            })
            .OrderByDescending(task => task.CreateDate)
            .ThenByDescending(task => task.Status)
            .ThenByDescending(task => task.CompleteDate);
            
            return Ok(taskList);
        }

        [HttpGet("result/{forecastTaskId}")]
        public async Task<IActionResult> GetForecastResult([FromRoute] string forecastTaskId)
        {
            var forecastResult = await _forecastResultRepository
                .ForecastResultsContextAccess
                .FirstOrDefaultAsync(result => result.TaskId == forecastTaskId);

            var forecastStream = await _forecastResultBlobClient.Get(forecastResult.Id);

            if (forecastStream.Length == 0)
            {
                throw new ArgumentException($"Can not find forecast result {forecastResult.Id}");
            }
            
            string forecastJson;

            using (var reader = new StreamReader(forecastStream, Encoding.UTF8))
            {
                forecastJson = await reader.ReadToEndAsync();
            }

            var forecast = JsonConvert.DeserializeObject<FinalForecast>(forecastJson);

            return Ok(forecast);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateForecastingTask([FromForm] CreateForecastRequest request)
        {
            HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader);

            var token = JWTHelper.GetJWTFromHeader(authHeader);
            var userId = JWTHelper.GetJWTValue(token, ClaimsTypes.Id);

            var modelingResult = await _modelingResultRepository
                .ModelingResultsContextAccess
                .OrderByDescending(result => result.CreateDate)
                .FirstOrDefaultAsync(result => result.TaskId == request.ModelingTaskId && result.Status == TaskStatus.Complete);
            
            var forecastTask = new ForecastTask
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Mode = request.Mode,
                CreateDate = DateTime.UtcNow,
                Status = TaskStatus.Pending,
                ModelingTaskId = request.ModelingTaskId,
                ModelingResultId = modelingResult.Id
            };

            try
            {
                await _forecastTaskRepository.Create(forecastTask);

                await _forecastBlobClient.Insert(forecastTask.Id, request.ForecastDataset.OpenReadStream());

                await _forecastTaskRepository.Commit();
                
                var dataset = new Dataset
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Name = forecastTask.Id,
                    CreateDate = DateTime.UtcNow,
                    Status = DatasetStatus.Uploaded,
                    Size = request.ForecastDataset.Length,
                    Columns = string.Empty,
                    Type = DatasetType.Forecast
                };
                
                var parseTask = new ParseTask
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Status = TaskStatus.Pending,
                    DatasetId = dataset.Id,
                    CreateDate = DateTime.UtcNow,
                    TaskType = ParseTaskType.Forecast
                };

                await _parseTaskRepository.Create(parseTask);
                await _datasetRepository.Create(dataset);
                
                await _parseTaskRepository.Commit();
                
                await _queueClient.AddMessage(new ParseQueueMessage
                {
                    DatasetId = forecastTask.Id,
                    TaskId = parseTask.Id,
                    Mode = request.ParseMode,
                    DataSeparator = request.DataSeparator
                });

                await _forecastTaskRepository.Commit();
            }
            catch (Exception exception)
            {
                await _forecastBlobClient.DeleteBlobIfExists(forecastTask.Id);
                return BadRequest($"Unexpected error: {exception.Message}. {exception.StackTrace}");
            }
            
            return Ok(forecastTask);
        }
    }
}