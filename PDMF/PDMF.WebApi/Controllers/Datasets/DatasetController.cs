using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PDMF.Data.Azure;
using PDMF.Data.Entities;
using PDMF.Data.Enums;
using PDMF.Data.Repositories;
using PDMF.Parsing.Models;
using PDMF.Parsing.Services;
using PDMF.WebApi.Controllers.Datasets.Models;
using PDMF.WebApi.Models.Identity.Data;

namespace PDMF.WebApi.Controllers.Datasets
{
    [Route("api/dataset")]
    [Authorize]
    public class DatasetController : ControllerBase
    {
        private readonly DatasetRepository _datasetRepository;
        private readonly DatasetBlobClient _blobClient;
        
        public DatasetController(
            DatasetRepository datasetRepository, 
            DatasetBlobClient blobClient)
        {
            _datasetRepository = datasetRepository;
            _blobClient = blobClient;
        }

        [HttpGet]
        [Route("modeling")]
        public async Task<IActionResult> GetAllForModeling()
        {
            HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader);
            
            var token = JWTHelper.GetJWTFromHeader(authHeader);
            var userId = JWTHelper.GetJWTValue(token, ClaimsTypes.Id);

            var datasets = await _datasetRepository.DatasetsContextAccess
                .Where(dataset => dataset.UserId == userId && dataset.Status == DatasetStatus.Parsed && dataset.Type == DatasetType.Default)
                .ToArrayAsync();

            var datasetsList = datasets.Select(dataset => new DatasetListModel()
            {
                Id = dataset.Id,
                Name = dataset.Name,
                UserId = dataset.UserId,
                AreaId = dataset.AreaId,
                CreateDate = dataset.CreateDate,
                Status = dataset.Status,
                Size = dataset.Size,
                Columns = string.IsNullOrEmpty(dataset.Columns) ? null : JsonConvert
                    .DeserializeObject<string[]>(dataset.Columns)
                    .Select((value, index) => new DatasetColumn()
                    {
                        Index = index,
                        Name = value
                    }).ToArray()
            });
            
            return Ok(datasetsList);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader);
            
            var token = JWTHelper.GetJWTFromHeader(authHeader);
            var userId = JWTHelper.GetJWTValue(token, ClaimsTypes.Id);

            var datasets = await _datasetRepository.DatasetsContextAccess
                .Where(dataset => dataset.UserId == userId && dataset.Type == DatasetType.Default)
                .ToArrayAsync();

            var datasetsList = datasets.Select(dataset => new DatasetListModel()
            {
                Id = dataset.Id,
                Name = dataset.Name,
                UserId = dataset.UserId,
                AreaId = dataset.AreaId,
                CreateDate = dataset.CreateDate,
                Status = dataset.Status,
                Size = dataset.Size,
                Columns = string.IsNullOrEmpty(dataset.Columns) ? null : JsonConvert
                    .DeserializeObject<string[]>(dataset.Columns)
                    .Select((value, index) => new DatasetColumn()
                {
                    Index = index,
                    Name = value
                }).ToArray()
            });
            
            return Ok(datasetsList);
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] DatasetModel datasetModel)
        {
            HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader);
            
            var token = JWTHelper.GetJWTFromHeader(authHeader);
            var userId = JWTHelper.GetJWTValue(token, ClaimsTypes.Id);

            var dataset = new Dataset
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Name = datasetModel.Name,
                CreateDate = DateTime.UtcNow,
                Status = DatasetStatus.Uploaded,
                Size = datasetModel.Dataset.Length,
                Columns = string.Empty,
                Type = DatasetType.Default
            };
            
            try
            {
                await _datasetRepository.Create(dataset);

                await _blobClient.Insert(dataset.Id, datasetModel.Dataset.OpenReadStream());

                await _datasetRepository.Commit();
            }
            catch (Exception exception)
            {
                await _blobClient.DeleteBlobIfExists(dataset.Id);
                return BadRequest($"Unexpected error: {exception.Message}. {exception.StackTrace}");
            }
            
            return Ok(dataset);
        }
        
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromForm] DatasetUpdateModel datasetModel)
        {
            var dataset = await _datasetRepository.Get(datasetModel.Id);

            if (dataset == null)
            {
                return BadRequest();
            }

            dataset.Name = datasetModel.Name;

            if (datasetModel.Dataset != null && datasetModel.Dataset.Length > 0)
            {
                await _blobClient.DeleteBlobIfExists(dataset.Id);
                await _blobClient.Insert(dataset.Id, datasetModel.Dataset.OpenReadStream());
            }

            dataset.Status = DatasetStatus.Uploaded;
            
            await _datasetRepository.Update(dataset);

            await _datasetRepository.Commit();
            
            return Ok();
        }

        [HttpPost("update-column")]
        public async Task<IActionResult> EditColumn([FromBody] UpdateColumnModel model)
        {
            var dataset = await _datasetRepository.Get(model.DatasetId);

            if (dataset == null)
            {
                return BadRequest();
            }

            var columns = JsonConvert.DeserializeObject<string[]>(dataset.Columns);

            columns[model.ColumnIndex-1] = model.ColumnName;

            dataset.Columns = JsonConvert.SerializeObject(columns);
            
            await _datasetRepository.Update(dataset);

            await _datasetRepository.Commit();
            
            return Ok(columns);
        }
    }
}