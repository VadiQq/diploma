using System;
using PDMF.Data.Enums;

namespace PDMF.WebApi.Controllers.Datasets.Models
{
    public class DatasetListModel
    {
        public string Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string UserId { get; set; }
        public string AreaId { get; set; }
        public string Name { get; set; }
        public DatasetColumn[] Columns { get; set; }
        public DatasetStatus Status { get; set; }
        public long Size { get; set; }
    }

    public class DatasetColumn
    {
        public int Index { get; set; }
        public string Name { get; set; }
    }
}