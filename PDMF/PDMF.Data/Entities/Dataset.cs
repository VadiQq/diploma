using System;
using System.Collections.Generic;
using PDMF.Data.Entities.Abstract;
using PDMF.Data.Enums;

#nullable disable

namespace PDMF.Data.Entities
{
    public class Dataset : DataEntity
    {
        public Dataset()
        {
            ModelingTasks = new HashSet<ModelingTask>();
            ParseTasks = new HashSet<ParseTask>();
        }

        public DatasetType Type { get; set; }
        public string UserId { get; set; }
        public string AreaId { get; set; }
        public string Name { get; set; }
        public string Columns { get; set; }
        public DatasetStatus Status { get; set; }
        public long Size { get; set; }
        public virtual Area Area { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<ModelingTask> ModelingTasks { get; set; }
        public virtual ICollection<ParseTask> ParseTasks { get; set; }
    }
}
