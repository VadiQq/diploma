namespace PDMF.WebApi.Controllers.Datasets.Models
{
    public class UpdateColumnModel
    {
        public string DatasetId { get; set; }
        public string ColumnName { get; set; }
        public int ColumnIndex { get; set; }
    }
}