using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PDMF.Algorithms.GMDH.Method;
using PDMF.Algorithms.GMDH.Models;
using PDMF.Data.Algorithms.Matrices;
using PDMF.Data.Azure;
using PDMF.Data.Azure.Abstract;
using PDMF.Data.Azure.Models.QueueMessages;
using PDMF.Data.Entities;
using PDMF.Data.Repositories;
using PDMF.WebApi.Controllers.Datasets.Models;
using PDMF.WebApi.Controllers.Modeling.Models;
using PDMF.WebApi.Models.Identity.Data;
using TaskStatus = PDMF.Data.Enums.TaskStatus;

namespace PDMF.WebApi.Controllers.Modeling
{
    [Route("api/modeling")]
    [Authorize]
    public class ModelingController : ControllerBase
    {
        private readonly ParseTaskRepository _parseTaskRepository;
        private readonly ModelingTaskRepository _modelingTaskRepository;
        private readonly ParseBlobClient _parseBlobClient;
        private readonly IAzureQueueClient<ModelingQueueMessage> _queueClient;
        
        public ModelingController(
            ParseTaskRepository parseTaskRepository,
            ParseBlobClient parseBlobClient,
            ModelingTaskRepository modelingTaskRepository, 
            IAzureQueueClient<ModelingQueueMessage> queueClient)
        {
            _parseTaskRepository = parseTaskRepository;
            _parseBlobClient = parseBlobClient;
            _modelingTaskRepository = modelingTaskRepository;
            _queueClient = queueClient;
        }
    
        [HttpGet("forecast")]
        public async Task<IActionResult> GetAllForForecasting()
        {
            HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader);
            
            var token = JWTHelper.GetJWTFromHeader(authHeader);
            var userId = JWTHelper.GetJWTValue(token, ClaimsTypes.Id);

            var modelingTasks = await _modelingTaskRepository.ModelingTasksContextAccess
                .Include(parse => parse.Dataset)
                .Where(task => task.CompleteDate != null && task.Status == TaskStatus.Complete && task.UserId == userId)
                .OrderByDescending(task => task.CompleteDate)
                .ToArrayAsync();

            var lastModelingTasks = modelingTasks
                .GroupBy(task => task.Dataset)
                .Select(group => group.First());
            
            var completeTasks = lastModelingTasks.Select(task => new CompleteModelingTask
            {
                ModelingTaskId = task.Id,
                DatasetName = task.Dataset.Name
            });
            
            return Ok(completeTasks);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader);
            
            var token = JWTHelper.GetJWTFromHeader(authHeader);
            var userId = JWTHelper.GetJWTValue(token, ClaimsTypes.Id);

            var modelingTasks = await _modelingTaskRepository.ModelingTasksContextAccess
                .Include(parse => parse.Dataset)
                .Where(task => task.UserId == userId)
                .ToArrayAsync();

            var modelingTaskList = modelingTasks.Select(modelingTask => new ModelingTaskListModel
                {
                    Id = modelingTask.Id,
                    DatasetId = modelingTask.DatasetId,
                    UserId = modelingTask.UserId,
                    DatasetName = modelingTask.Dataset.Name,
                    CompleteDate = modelingTask.CompleteDate,
                    CreateDate = modelingTask.CreateDate,
                    DesiredColumn =  JsonConvert.DeserializeObject<string[]>(modelingTask.Dataset.Columns)
                        .Select((value, index) => new DatasetColumn()
                        {
                            Index = index,
                            Name = value
                        }).ToArray()[modelingTask.DesiredColumn - 1]
                        .Name,
                    Status = modelingTask.Status
                })
                .OrderByDescending(modelingTask => modelingTask.CreateDate)
                .ThenByDescending(modelingTask => modelingTask.Status)
                .ThenByDescending(modelingTask => modelingTask.CompleteDate);
            
            return Ok(modelingTaskList);
        }
        
        [HttpPost("local")]
        public async Task<IActionResult> CreateModelLocally([FromBody] CreateModelRequest request)
        {
            var parseTask = await _parseTaskRepository.ParseTasksContextAccess
                .OrderByDescending(task => task.CompleteDate)
                .FirstOrDefaultAsync(task => task.Status == TaskStatus.Complete && task.DatasetId == request.DatasetId);

            if (parseTask == null)
            {
                return BadRequest("Unable to find parse complete for modeling.");
            }

            var parseStream = await _parseBlobClient.Get(parseTask.Id);
            
            Matrix parse;
            
            using (var reader = new StreamReader(parseStream, Encoding.UTF8))
            {
                parse = JsonConvert.DeserializeObject<Matrix>(await reader.ReadToEndAsync());
            }

            // var model = new WienerBinaryModel();
            // model.BModelValues = parse;
            // model.BTestModelValues = parse;
            // var c = JsonConvert.SerializeObject(model);
            
            var GMDH = new GMDH();
            
            var parseModel = GMDH.CreateModel(1, parse, 10);
            
            return Ok();
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateModelingTask([FromBody] CreateModelRequest request)
        {
            HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader);
            
            var token = JWTHelper.GetJWTFromHeader(authHeader);
            var userId = JWTHelper.GetJWTValue(token, ClaimsTypes.Id);

            var modelingTask = new ModelingTask
            {
                Id = Guid.NewGuid().ToString(),
                DatasetId = request.DatasetId,
                DesiredColumn = request.DesiredColumn,
                UserId = userId,
                Status = TaskStatus.Pending,
                ModelType = request.ModelType,
                CreateDate = DateTime.UtcNow
            };

            await _modelingTaskRepository.Create(modelingTask);
            
            await _queueClient.AddMessage(new ModelingQueueMessage
            {
                DatasetId = request.DatasetId,
                ModelType = request.ModelType,
                DesiredColumn = request.DesiredColumn,
                ModelingTaskId = modelingTask.Id
            });
            
            await _modelingTaskRepository.Commit();
            return Ok(modelingTask);
        }
    }
}