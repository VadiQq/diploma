using System;
using PDMF.Data.Entities.Abstract;

#nullable disable

namespace PDMF.Data.Entities
{
    public class TaskAudit : DataEntity
    {
        public string TaskId { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
    }
}
