using System;
using System.Collections.Generic;
using PDMF.Data.Entities.Abstract;
using PDMF.Data.Enums;

#nullable disable

namespace PDMF.Data.Entities
{
    public class ModelingResult : DataEntity
    {
        public ModelingResult()
        {
            ForecastTasks = new HashSet<ForecastTask>();
        }

        public string TaskId { get; set; }
        public TaskStatus Status { get; set; }
        public ModelType ModelType { get; set; }
        public string UserId { get; set; }

        public virtual ModelingTask Task { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<ForecastTask> ForecastTasks { get; set; }
    }
}
