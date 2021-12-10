using System;
using PDMF.Data.Enums;

namespace PDMF.WebApi.Controllers.Forecasting.Models
{
    public class ForecastListModel
    {
        public string Id { get; set; }
        public string ModelingTaskId { get; set; }
        public string ModelingResultId { get; set; }
        public string DatasetName { get; set; }
        public string DesiredColumn { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? CompleteDate { get; set; }
        public ForecastMode Mode { get; set; }
    }
}