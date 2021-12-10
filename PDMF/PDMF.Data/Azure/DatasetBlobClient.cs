using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using PDMF.Data.Azure.Base;
using PDMF.Data.Azure.Models;
using PDMF.Data.Entities;

namespace PDMF.Data.Azure
{
    public class DatasetBlobClient : AzureBlobClient<AzureBlobClientConfiguration<Dataset>, Dataset>
    {
        private const string DatasetBlobContainerName = "datasets";
        
        public DatasetBlobClient(AzureBlobClientConfiguration<Dataset> azureBlobClientConfiguration) : base(azureBlobClientConfiguration)
        {
        }

        public override Task<BlobClient> Insert(string fileName, Stream stream)
        {
            return base.Insert($"{DatasetBlobContainerName}/{fileName}", stream);
        }
        
        public override Task DeleteBlobIfExists(string fileName)
        {
            return base.DeleteBlobIfExists($"{DatasetBlobContainerName}/{fileName}");
        }
        
        public override Task<Stream> Get(string fileName)
        {
            return base.Get($"{DatasetBlobContainerName}/{fileName}");
        }
    }
}