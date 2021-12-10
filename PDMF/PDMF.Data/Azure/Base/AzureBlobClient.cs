using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using PDMF.Data.Azure.Models;
using PDMF.Data.Entities.Abstract;

namespace PDMF.Data.Azure.Base
{
    public class AzureBlobClient<T, TF> where T : AzureBlobClientConfiguration<TF>  where TF : DataEntity
    {
        private const int SasExpirationTimeout = 15;

        private readonly AzureBlobClientConfiguration<TF> _azureBlobClientConfiguration;

        protected string ConnectionString => _azureBlobClientConfiguration.ConnectionString;

        protected virtual RetryOptions RetryOptions => null;

        protected virtual PublicAccessType PublicAccessType => PublicAccessType.None;

        protected AzureBlobClient(T azureBlobClientConfiguration)
        {
            _azureBlobClientConfiguration = azureBlobClientConfiguration;
        }
        
        public virtual async Task<Stream> Get(string filePath)
        {
            BlobClient blob = await GetCloudBlob(filePath);

            var memoryStream = new MemoryStream();
            await blob.DownloadToAsync(memoryStream);
            memoryStream.Position = 0;
            
            return memoryStream;
        }

        public virtual async Task<bool> Exists(string filePath)
        {
            BlobClient blob = await GetCloudBlob(filePath);
            return await blob.ExistsAsync();
        }
        
        public async Task<string> GetBlobSas(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return string.Empty;
            }

            BlobClient blob = await GetCloudBlob(filePath);
            
            var sasToken = blob.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(SasExpirationTimeout));
            return $"{blob.Uri}{sasToken}";
        }

        public virtual async Task<BlobClient> Insert(string filePath, Stream stream)
        {
            BlobClient blob = await GetCloudBlob(filePath);
            await blob.UploadAsync(stream);

            return blob;
        }

        public async Task<BlobClient> Insert(string filePath, Stream stream, string contentType)
        {
            BlobClient blob = await Insert(filePath, stream);
            await blob.SetHttpHeadersAsync(new BlobHttpHeaders
            {
                ContentType = contentType
            });
            return blob;
        }

        protected virtual async Task<BlobClient> GetCloudBlob(string filePath)
        {
            GetFilePathParts(filePath, out var containerName, out var blobPath);

            var blobClientOptions = new BlobClientOptions();
            if(RetryOptions != null)
            {
                blobClientOptions.Retry.MaxRetries = RetryOptions.MaxRetries;
                blobClientOptions.Retry.Delay = RetryOptions.Delay;
                blobClientOptions.Retry.NetworkTimeout = RetryOptions.NetworkTimeout;
                blobClientOptions.Retry.Mode = RetryOptions.Mode;
                blobClientOptions.Retry.MaxDelay = RetryOptions.MaxDelay;
            }

            var blobServiceClient = new BlobServiceClient(ConnectionString, blobClientOptions);
            var container = blobServiceClient.GetBlobContainerClient(containerName);
            
            if (!await container.ExistsAsync())
            {
                await container.CreateIfNotExistsAsync(PublicAccessType, null, null);
            }
            
            return container.GetBlobClient(blobPath);
        }

        protected void GetFilePathParts(string filePath, out string containerName, out string blobPath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var containerNameLength = filePath.IndexOf('/');
            if (containerNameLength < 0)
            {
                throw new ArgumentException("filePath should contain at least 2 parts - {container}/{blob file path}");
            }

            containerName = filePath.Substring(0, containerNameLength);
            blobPath = filePath.Substring(containerNameLength + 1);
        }

        public virtual async Task DeleteContainerIfExists(string containerName)
        {
            if (string.IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException(nameof(containerName));
            }
            
            var blobClient = new BlobServiceClient(ConnectionString);
            BlobContainerClient container = blobClient.GetBlobContainerClient(containerName);
            await container.DeleteIfExistsAsync();
        }

        public virtual async Task DeleteBlobIfExists(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            GetFilePathParts(filePath, out var containerName, out var blobPath);

            var blobClient = new BlobServiceClient(ConnectionString);
            BlobContainerClient container = blobClient.GetBlobContainerClient(containerName);
            BlobClient blob = container.GetBlobClient(blobPath);
            await blob.DeleteIfExistsAsync();
        }
    }
}