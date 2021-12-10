using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using PDMF.Data.Azure.Base;
using PDMF.Data.Azure.Models;
using PDMF.Data.Entities;

namespace PDMF.Data.Azure
{
    public class ModelingBlobClient : AzureBlobClient<AzureBlobClientConfiguration<ModelingTask>, ModelingTask>
    {
        private const string  ModelingBlobContainerName = "modeling";
        
        public ModelingBlobClient(AzureBlobClientConfiguration<ModelingTask> azureBlobClientConfiguration) : base(azureBlobClientConfiguration)
        {
        }

        public override Task<BlobClient> Insert(string fileName, Stream stream)
        {
            return base.Insert($"{ModelingBlobContainerName}/{fileName}", stream);
        }
        
        public override Task DeleteBlobIfExists(string fileName)
        {
            return base.DeleteBlobIfExists($"{ModelingBlobContainerName}/{fileName}");
        }
        
        public override Task<Stream> Get(string fileName)
        {
            return base.Get($"{ModelingBlobContainerName}/{fileName}");
        }
    }
}