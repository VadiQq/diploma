using System;
using PDMF.Data.Entities.Abstract;
using PDMF.Data.Enums;

#nullable disable

namespace PDMF.Data.Entities
{
    public class ParseTask : DataEntity
    {
        public string DatasetId { get; set; }
        public ParseTaskType TaskType { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime? CompleteDate { get; set; }
        public string ResultDescription { get; set; }
        public string UserId { get; set; }

        public virtual Dataset Dataset { get; set; }
        public virtual User User { get; set; }
    }
}
