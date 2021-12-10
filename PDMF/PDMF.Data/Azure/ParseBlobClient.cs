using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using PDMF.Data.Azure.Base;
using PDMF.Data.Azure.Models;
using PDMF.Data.Entities;

namespace PDMF.Data.Azure
{
    public class ParseBlobClient : AzureBlobClient<AzureBlobClientConfiguration<ParseTask>, ParseTask>
    {
        private const string  ParseBlobContainerName = "parse";
        
        public ParseBlobClient(AzureBlobClientConfiguration<ParseTask> azureBlobClientConfiguration) : base(azureBlobClientConfiguration)
        {
        }

        public override Task<BlobClient> Insert(string fileName, Stream stream)
        {
            return base.Insert($"{ParseBlobContainerName}/{fileName}", stream);
        }
        
        public override Task DeleteBlobIfExists(string fileName)
        {
            return base.DeleteBlobIfExists($"{ParseBlobContainerName}/{fileName}");
        }
        
        public override Task<Stream> Get(string fileName)
        {
            return base.Get($"{ParseBlobContainerName}/{fileName}");
        }
        
        public override Task<bool> Exists(string fileName)
        {
            return base.Exists($"{ParseBlobContainerName}/{fileName}");
        }
    }
}