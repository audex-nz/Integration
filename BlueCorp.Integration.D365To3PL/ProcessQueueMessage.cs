using System.Text.Json;
using Azure.Data.Tables;
using BlueCorp.Integration.D365To3PL.Configurations;
using BlueCorp.Integration.D365To3PL.Models;
using BlueCorp.Integration.D365To3PL.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlueCorp.Integration.D365To3PL
{
    public class ProcessQueueMessage
    {
        private readonly ILogger<ProcessQueueMessage> _logger;
        private readonly IDataProcessingService _processingService;
        private readonly StorageSettings _settings;


        public ProcessQueueMessage(
            ILogger<ProcessQueueMessage> logger,
            IDataProcessingService processingService,
            IOptions<StorageSettings> settings)
        {
            _logger = logger;
            _processingService = processingService;
            _settings = settings.Value;
        }

        [Function("ProcessQueueMessage")]
        public async Task Run(
            [QueueTrigger("%Storage:QueueName%", Connection = "")] string queueItem,
            FunctionContext executionContext)
        {
            var retryCount = 0;
            try
            {
                
                var bindingData = executionContext.BindingContext.BindingData;

                if (bindingData.TryGetValue("DequeueCount", out var dequeueCount))
                    retryCount = Convert.ToInt32(dequeueCount);

                var dispatchEvent = JsonSerializer.Deserialize<DispatchEvent>(queueItem);
                if (dispatchEvent == null)
                {
                    _logger.LogError("Failed to deserialize queue item.");
                    return;
                }


                // Check if controlNumber exists in Table Storage
                var tableClient = new TableClient(_settings.ConnectionString, _settings.TableName);
                await tableClient.CreateIfNotExistsAsync();

                var existingRecord = await tableClient.GetEntityIfExistsAsync<TableEntity>("partitionKey", dispatchEvent.ControlNumber.ToString());

                if (existingRecord.HasValue)
                {
                    _logger.LogInformation("Duplicate controlNumber {ControlNumber}, ignoring message.", dispatchEvent.ControlNumber);
                    return;
                }

                // Process the message (convert to CSV and send to SFTP)
                await _processingService.ProcessD365EventAsync(dispatchEvent);

                var entity = new TableEntity("partitionKey", dispatchEvent.ControlNumber.ToString())
                {
                    { "Timestamp", DateTime.UtcNow }
                };
                await tableClient.AddEntityAsync(entity);

                _logger.LogInformation("Successfully processed controlNumber {ControlNumber}.", dispatchEvent.ControlNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error processing queue message {RetryCount} of 5 ", retryCount);
                throw;
            }
        }
    }
}
