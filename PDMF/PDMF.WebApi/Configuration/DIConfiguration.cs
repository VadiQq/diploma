using Microsoft.Extensions.DependencyInjection;
using PDMF.Data.Azure;
using PDMF.Data.Azure.Abstract;
using PDMF.Data.Azure.Base;
using PDMF.Data.Azure.Models;
using PDMF.Data.Azure.Models.QueueMessages;
using PDMF.Data.Contexts;
using PDMF.Data.Entities;
using PDMF.Data.Repositories;
using PDMF.Data.Utilities.Configuration;
using PDMF.Parsing.Services;
using PDMF.Parsing.Services.Abstract;

namespace PDMF.WebApi.Configuration
{
    public static class DIConfiguration
    {
        public static void Register(IServiceCollection container)
        {
            container.AddSingleton(new AzureBlobClientConfiguration<Dataset>
            {
                ConnectionString = Configurations.GetConnectionString("AzureStorageConnection")
            });
            
            container.AddSingleton(new AzureBlobClientConfiguration<ParseTask>
            {
                ConnectionString = Configurations.GetConnectionString("AzureStorageConnection")
            });
            
            container.AddSingleton(new AzureBlobClientConfiguration<ForecastTask>
            {
                ConnectionString = Configurations.GetConnectionString("AzureStorageConnection")
            });
            
            container.AddSingleton(new AzureBlobClientConfiguration<ForecastResult>
            {
                ConnectionString = Configurations.GetConnectionString("AzureStorageConnection")
            });
            
            container.AddSingleton(new AzureQueueClientConfiguration<ParseQueueMessage>
            {
                ConnectionString = Configurations.GetConnectionString("AzureStorageConnection"),
                QueueName = "parse"
            });
            
            container.AddSingleton(new AzureQueueClientConfiguration<ModelingQueueMessage>
            {
                ConnectionString = Configurations.GetConnectionString("AzureStorageConnection"),
                QueueName = "modeling"
            });

            container.AddSingleton(new AzureQueueClientConfiguration<ForecastingQueueMessage>
            {
                ConnectionString = Configurations.GetConnectionString("AzureStorageConnection"),
                QueueName = "forecasting"
            });

            container.AddSingleton(new AzureBlobClientConfiguration<ModelingResult>()
            {
                ConnectionString = Configurations.GetSetting("AzureStorageConnection")
            });
            
            container.AddSingleton(new AzureBlobClientConfiguration<ModelingTask>()
            {
                ConnectionString = Configurations.GetSetting("AzureStorageConnection")
            });
            
            container.AddScoped<IDatasetParser, DatasetParser>();
            
            container.AddScoped<IAzureQueueClient<ParseQueueMessage>, AzureQueueClient<ParseQueueMessage>>();
            container.AddScoped<IAzureQueueClient<ModelingQueueMessage>, AzureQueueClient<ModelingQueueMessage>>();
            container.AddScoped<IAzureQueueClient<ForecastingQueueMessage>, AzureQueueClient<ForecastingQueueMessage>>();
            container.AddScoped<DatasetBlobClient>();
            container.AddScoped<ForecastBlobClient>();
            container.AddScoped<ForecastResultBlobClient>();
            container.AddScoped<ModelingResultBlobClient>();
            container.AddScoped<ModelingBlobClient>();
            container.AddScoped<ParseBlobClient>();
            container.AddScoped<PDMFDatabaseContext>();
            container.AddScoped<DatasetRepository>();
            container.AddScoped<ParseTaskRepository>();
            container.AddScoped<ModelingTaskRepository>();
            container.AddScoped<ModelingResultRepository>();
            container.AddScoped<ForecastTaskRepository>();
            container.AddScoped<ForecastResultRepository>();
            container.AddScoped<UserRepository>();
        }
    }
}