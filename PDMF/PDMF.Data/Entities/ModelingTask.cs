using System;
using System.Collections.Generic;
using PDMF.Data.Entities.Abstract;
using PDMF.Data.Enums;

#nullable disable

namespace PDMF.Data.Entities
{
    public class ModelingTask : DataEntity
    {
        public ModelingTask()
        {
            ModelingResults = new HashSet<ModelingResult>();
        }

        public string DatasetId { get; set; }
        public ModelType ModelType { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime? CompleteDate { get; set; }
        public int DesiredColumn { get; set; }
        public string UserId { get; set; }

        public virtual Dataset Dataset { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<ModelingResult> ModelingResults { get; set; }
    }
}
