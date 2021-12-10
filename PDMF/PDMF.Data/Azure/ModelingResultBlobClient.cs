using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using PDMF.Data.Azure.Base;
using PDMF.Data.Azure.Models;
using PDMF.Data.Entities;

namespace PDMF.Data.Azure
{
    public class ModelingResultBlobClient : AzureBlobClient<AzureBlobClientConfiguration<ModelingResult>, ModelingResult>
    {
        private const string  ModelingBlobContainerName = "modeling-result";

        public string GetContainerName()
        {
            return ModelingBlobContainerName;
        }
            
        public ModelingResultBlobClient(AzureBlobClientConfiguration<ModelingResult> azureBlobClientConfiguration) : base(azureBlobClientConfiguration)
        {
        }

        public Task<bool> ExistsInFolder(string fileName, string folderName)
        {
            return base.Exists($"{ModelingBlobContainerName}/{folderName}/{fileName}");
        }
        
        public Task<Stream> GetFromFolder(string fileName, string folderName)
        {
            return base.Get($"{ModelingBlobContainerName}/{folderName}/{fileName}");
        }
        
        public Task<BlobClient> InsertWithFolder(string fileName, string folderName, Stream stream)
        {
            return base.Insert($"{ModelingBlobContainerName}/{folderName}/{fileName}", stream);
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