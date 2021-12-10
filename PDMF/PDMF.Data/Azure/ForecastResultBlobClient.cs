using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using PDMF.Data.Azure.Base;
using PDMF.Data.Azure.Models;
using PDMF.Data.Entities;

namespace PDMF.Data.Azure
{
    public class ForecastResultBlobClient : AzureBlobClient<AzureBlobClientConfiguration<ForecastResult>, ForecastResult>
    {
        private const string  ForecastBlobContainerName = "forecast-result-data";
        
        public ForecastResultBlobClient(AzureBlobClientConfiguration<ForecastResult> azureBlobClientConfiguration) : base(azureBlobClientConfiguration)
        {
        }

        public override Task<BlobClient> Insert(string fileName, Stream stream)
        {
            return base.Insert($"{ForecastBlobContainerName}/{fileName}", stream);
        }
        
        public override Task DeleteBlobIfExists(string fileName)
        {
            return base.DeleteBlobIfExists($"{ForecastBlobContainerName}/{fileName}");
        }
        
        public override Task<Stream> Get(string fileName)
        {
            return base.Get($"{ForecastBlobContainerName}/{fileName}");
        }
    }
}