using System.Net;
using BlueCorp.Integration.D365To3PL.Models;
using System.Text.Json;
using Azure.Storage.Queues;
using BlueCorp.Integration.D365To3PL.Authentication;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text;
using BlueCorp.Integration.D365To3PL.Configurations;
using Microsoft.Extensions.Options;

namespace BlueCorp.Integration.D365To3PL
{
    public class DispatchEventTrigger
    {
        private readonly ILogger<DispatchEventTrigger> _logger;
        private readonly AuthenticationHelper _authHelper;
        private readonly StorageSettings _settings;
        // private static readonly string ConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        // private const string QueueName = "dispatch-queue";

        public DispatchEventTrigger(
            ILogger<DispatchEventTrigger> logger,
            AuthenticationHelper authHelper,
            IOptions<StorageSettings> settings)
        {
            _logger = logger;
            _authHelper = authHelper;
            _settings = settings.Value;
        }

        [Function("DispatchEventTrigger")]
        public async Task<HttpResponseData> RunAsync ([HttpTrigger(AuthorizationLevel.Function, "post", Route = "dispatch")] HttpRequestData req)
        {
            _logger.LogInformation("Received dispatch event request");

            try
            {
                // Using the IsAuthenticated method to check if the request is authenticated
                if (!_authHelper.IsAuthenticated(req))
                {
                    _logger.LogWarning("Unauthenticated request received");
                    var unauthorizedResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                    return unauthorizedResponse;
                }

                // Read the request body
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                if (string.IsNullOrEmpty(requestBody))
                {
                    _logger.LogWarning("Empty request body received");
                    var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badRequestResponse.WriteStringAsync("Request body cannot be empty");
                    return badRequestResponse;
                }

                // Deserialize the request body into a DispatchEvent object
                try
                {
                    var dispatchEvent = JsonSerializer.Deserialize<DispatchEvent>(requestBody);

                    if (dispatchEvent == null)
                    {
                        _logger.LogWarning("Failed to deserialize request body");
                        var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                        await badRequestResponse.WriteStringAsync("Invalid request format");
                        return badRequestResponse;
                    }

                    // Process the dispatch event
                    var queueClient = new QueueClient(_settings.ConnectionString, _settings.QueueName);
                    await queueClient.CreateIfNotExistsAsync();

                    // Add message to queue
                    await queueClient.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(requestBody)));

                    // Return a success response
                    var successResponse = req.CreateResponse(HttpStatusCode.OK);
                    var responsePayload = new
                    {
                        success = true,
                        message = $"Event with control number {dispatchEvent.ControlNumber} processed successfully",
                        timestamp = DateTime.UtcNow
                    };
                    await successResponse.WriteAsJsonAsync(responsePayload);

                    return successResponse;
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error deserializing JSON request");
                    var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badRequestResponse.WriteStringAsync("Invalid JSON format");
                    return badRequestResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing dispatch event");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("An error occurred while processing the request");
                return errorResponse;
            }
        }
    }
}
