using System;
using System.IO;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PDMF.Data.Azure;
using PDMF.Data.Azure.Abstract;
using PDMF.Data.Azure.Base;
using PDMF.Data.Azure.Models;
using PDMF.Data.Azure.Models.QueueMessages;
using PDMF.Data.Contexts;
using PDMF.Data.Entities;
using PDMF.Data.Repositories;
using PDMF.Data.Utilities;
using PDMF.Data.Utilities.Configuration;
using PDMF.Data.Utilities.Configuration.Managers;
using PDMF.Parsing.Services;
using PDMF.Parsing.Services.Abstract;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace PDMF.Functions.Modeling.Function
{
    public class Program
    {
        static void Main(string[] args)
        {
            Configurations.InitializeConfigurationManager(new EnvironmentConfigurationManager());
            
            Serilog.Debugging.SelfLog.Enable(Console.WriteLine);
            SetSerilogOptions();
            
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(RegisterServices)
                .Build();

            var rootPath = Environment.GetEnvironmentVariable("HOME") == null ? 
                $"{Directory.GetCurrentDirectory()}\\..\\..\\..\\" : 
                $"{Environment.GetEnvironmentVariable("HOME")}\\site\\wwwroot";
            
            VirtualPathMapper.Initialize(rootPath);

            host.Run();
        }

        private static void SetSerilogOptions()
        {
            var telemetryConfiguration = TelemetryConfiguration.CreateDefault();

            telemetryConfiguration.InstrumentationKey = "27527666-09fc-411a-bfe0-ce82587842df";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                    theme: SystemConsoleTheme.Literate)
                .WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .CreateLogger();
        }
        
        private static void RegisterServices(IServiceCollection container)
        {
            container.AddSingleton(new AzureBlobClientConfiguration<ParseTask>
            {
                ConnectionString = Configurations.GetConnectionString("AzureStorageConnection")
            });
            
            container.AddSingleton(new AzureBlobClientConfiguration<ModelingTask>
            {
                ConnectionString = Configurations.GetConnectionString("AzureStorageConnection")
            });

            container.AddSingleton(new AzureQueueClientConfiguration<ModelingMonitoringQueueMessage>
            {
                ConnectionString = Configurations.GetConnectionString("AzureStorageConnection"),
                QueueName = "modeling-monitoring"
            });
            
            container.AddSingleton(new AzureQueueClientConfiguration<ModelingCleanupQueueMessage>
            {
                ConnectionString = Configurations.GetConnectionString("AzureStorageConnection"),
                QueueName = "modeling-cleanup"
            });
            
            container.AddScoped<IDatasetParser, DatasetParser>();
            
            container.AddScoped<IAzureQueueClient<ModelingMonitoringQueueMessage>, AzureQueueClient<ModelingMonitoringQueueMessage>>();
            container.AddScoped<IAzureQueueClient<ModelingCleanupQueueMessage>, AzureQueueClient<ModelingCleanupQueueMessage>>();
            container.AddScoped<DatasetBlobClient>();
            container.AddScoped<ParseBlobClient>();
            container.AddScoped<PDMFDatabaseContext>();
            container.AddScoped<DatasetRepository>();
            container.AddScoped<ParseTaskRepository>();
            container.AddScoped<ModelingTaskRepository>();
            container.AddScoped<ModelingResultRepository>();
        }
    }
}