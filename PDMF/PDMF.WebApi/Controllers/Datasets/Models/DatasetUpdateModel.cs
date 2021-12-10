using Microsoft.AspNetCore.Http;

namespace PDMF.WebApi.Controllers.Datasets.Models
{
    public class DatasetUpdateModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IFormFile Dataset { get; set; }
    }
}