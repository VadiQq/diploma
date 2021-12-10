using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PDMF.Data.Azure.Abstract;
using PDMF.Data.Azure.Models.QueueMessages;
using PDMF.Data.Entities;
using PDMF.Data.Enums;
using PDMF.Data.Repositories;
using PDMF.WebApi.Controllers.Datasets.Models;
using PDMF.WebApi.Models.Identity.Data;
using TaskStatus = PDMF.Data.Enums.TaskStatus;

namespace PDMF.WebApi.Controllers.Datasets
{
    [Route("api/parse")]
    [Authorize]
    public class ParseController : ControllerBase
    {
        private readonly IAzureQueueClient<ParseQueueMessage> _queueClient;
        private readonly DatasetRepository _datasetRepository;
        private readonly ParseTaskRepository _parseTaskRepository;
        
        public ParseController(
            IAzureQueueClient<ParseQueueMessage> queueClient, 
            DatasetRepository datasetRepository, 
            ParseTaskRepository parseTaskRepository)
        {
            _queueClient = queueClient;
            _datasetRepository = datasetRepository;
            _parseTaskRepository = parseTaskRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader);
            
            var token = JWTHelper.GetJWTFromHeader(authHeader);
            var userId = JWTHelper.GetJWTValue(token, ClaimsTypes.Id);

            var parseTasks = await _parseTaskRepository.ParseTasksContextAccess
                .Include(parse => parse.Dataset)
                .Where(task => task.UserId == userId && task.Dataset.Type == DatasetType.Default)
                .ToArrayAsync();

            var parseTaskList = parseTasks.Select(parse => new ParseListModel
            {
                Id = parse.Id,
                DatasetId = parse.DatasetId,
                UserId = parse.UserId,
                DatasetName = parse.Dataset.Name,
                CompleteDate = parse.CompleteDate,
                CreateDate = parse.CreateDate,
                ResultDescription = parse.ResultDescription,
                Status = parse.Status
            }).OrderByDescending(parse => parse.CreateDate)
                .ThenByDescending(parse => parse.CompleteDate);
            
            return Ok(parseTaskList);
        }
        
        [HttpPost]
        public async Task<IActionResult> ParseDataset([FromBody] ParseRequest request)
        {
            HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader);
            
            var token = JWTHelper.GetJWTFromHeader(authHeader);
            var userId = JWTHelper.GetJWTValue(token, ClaimsTypes.Id);
            
            var dataset = await _datasetRepository.Get(request.DatasetId);

            if (dataset == null)
            {
                return BadRequest();
            }
            
            var parseTask = new ParseTask
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Status = TaskStatus.Pending,
                DatasetId = request.DatasetId,
                CreateDate = DateTime.UtcNow,
                TaskType = ParseTaskType.Default
            };

            await _parseTaskRepository.Create(parseTask);

            await _parseTaskRepository.Commit();
                
            await _queueClient.AddMessage(new ParseQueueMessage
            {
                DatasetId = request.DatasetId,
                TaskId = parseTask.Id,
                Mode = request.Mode,
                DataSeparator = request.DataSeparator
            });
            var d = JsonConvert.DefaultSettings;
            
            return Ok(parseTask);
        }
    }
}