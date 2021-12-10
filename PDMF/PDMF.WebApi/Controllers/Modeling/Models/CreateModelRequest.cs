using PDMF.Data.Enums;

namespace PDMF.WebApi.Controllers.Modeling.Models
{
    public class CreateModelRequest
    {
        public string DatasetId { get; set; }
        public int DesiredColumn { get; set; }
        public ModelType ModelType { get; set; }
    }
}