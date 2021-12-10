using System;
using System.Collections.Generic;
using PDMF.Data.Entities.Abstract;
using PDMF.Data.Enums;

#nullable disable

namespace PDMF.Data.Entities
{
    public class ForecastTask : DataEntity
    {
        public ForecastTask()
        {
            ForecastResults = new HashSet<ForecastResult>();
        }

        public string ModelingTaskId { get; set; }
        public string ModelingResultId { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime? CompleteDate { get; set; }
        public ForecastMode Mode { get; set; }
        public string UserId { get; set; }

        public virtual ModelingTask ModelingTask { get; set; }
        public virtual ModelingResult ModelingResult { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<ForecastResult> ForecastResults { get; set; }
    }
}
