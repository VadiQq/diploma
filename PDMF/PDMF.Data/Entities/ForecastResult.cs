using PDMF.Data.Entities.Abstract;
using PDMF.Data.Enums;

#nullable disable

namespace PDMF.Data.Entities
{
    public class ForecastResult : DataEntity
    {
        public string TaskId { get; set; }
        public TaskStatus Status { get; set; }
        public string UserId { get; set; }

        public virtual ForecastTask Task { get; set; }
        public virtual User User { get; set; }
    }
}
