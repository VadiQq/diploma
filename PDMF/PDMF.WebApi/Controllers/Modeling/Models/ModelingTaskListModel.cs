using System;
using PDMF.Data.Enums;

namespace PDMF.WebApi.Controllers.Modeling.Models
{
    public class ModelingTaskListModel
    {
        public string Id { get; set; }
        public string DatasetId { get; set; }
        public string UserId { get; set; }
        public string DatasetName { get; set; }
        public string DesiredColumn { get; set; }
        public DateTime CreateDate { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime? CompleteDate { get; set; }
    }
}