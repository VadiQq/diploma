using Microsoft.EntityFrameworkCore.Design;

namespace PDMF.Data.Azure.Models
{
    public class AzureBatchConfiguration
    {
        public string PoolId { get; set; }
        public string JobId { get; set; }
    }
}