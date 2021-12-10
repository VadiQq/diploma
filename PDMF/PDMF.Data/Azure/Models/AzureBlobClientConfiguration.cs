using PDMF.Data.Entities.Abstract;

namespace PDMF.Data.Azure.Models
{
    public class AzureBlobClientConfiguration<T> where T : DataEntity
    {
        public string ConnectionString { get; set; }
    }
}