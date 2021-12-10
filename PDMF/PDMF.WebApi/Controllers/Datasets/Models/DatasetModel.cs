using Microsoft.AspNetCore.Http;

namespace PDMF.WebApi.Controllers.Datasets.Models
{
    public class DatasetModel
    {
        public string Name { get; set; }
        public IFormFile Dataset { get; set; }
    }
}