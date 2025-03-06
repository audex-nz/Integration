using System.Text.Json;
using BlueCorp.Integration.D365To3PL.Authentication;
using BlueCorp.Integration.D365To3PL.Configurations;
using BlueCorp.Integration.D365To3PL.Models;
using BlueCorp.Integration.D365To3PL.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlueCorp.Integration.D365To3PL
{
    public class ProcessQueuePoisonMessage
    {
        private readonly ILogger<ProcessQueuePoisonMessage> _logger;
        private readonly IDataProcessingService _processingService;
        private readonly StorageSettings _settings;


        public ProcessQueuePoisonMessage(
            ILogger<ProcessQueuePoisonMessage> logger,
            IDataProcessingService processingService,
            AuthenticationHelper authHelper,
            IOptions<StorageSettings> settings)
        {
            _logger = logger;
            _processingService = processingService;
            _settings = settings.Value;
        }

        [Function("ProcessQueuePoisonMessage")]
        public async Task Run(
            [QueueTrigger("%Storage:QueuePoisonName%", Connection = "")] string poisonMessage,
            FunctionContext executionContext)
        {
            try
            {

                var dispatchEvent = JsonSerializer.Deserialize<DispatchEvent>(poisonMessage);
                if (dispatchEvent == null)
                {
                    _logger.LogError("Failed to deserialize queue item.");
                    return;
                }

                // TODO: Send notification (email/Teams) about the failed message, after all retries

                _logger.LogInformation("Poison message controlNumber {ControlNumber}, delete.", dispatchEvent.ControlNumber);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error processing poison message: {Message}", ex.Message);
            }
        }
    }
}
