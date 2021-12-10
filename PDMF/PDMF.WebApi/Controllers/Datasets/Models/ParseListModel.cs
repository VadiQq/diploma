using System;
using PDMF.Data.Enums;

namespace PDMF.WebApi.Controllers.Datasets.Models
{
    public class ParseListModel
    {
        public string Id { get; set; }
        public string DatasetId { get; set; }
        public string UserId { get; set; }
        public string DatasetName { get; set; }
        public DateTime CreateDate { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime? CompleteDate { get; set; }
        public string ResultDescription { get; set; }
    }
}