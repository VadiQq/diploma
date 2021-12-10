using System;
using System.Collections.Generic;
using PDMF.Data.Entities.Abstract;

#nullable disable

namespace PDMF.Data.Entities
{
    public class Area : DataEntity
    {
        public Area()
        {
            Datasets = new HashSet<Dataset>();
        }

        public string UserId { get; set; }
        public string Name { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Dataset> Datasets { get; set; }
    }
}
