using System.Globalization;
using System.Text;
using BlueCorp.Integration.D365To3PL.Mappings;
using BlueCorp.Integration.D365To3PL.Models;
using BlueCorp.Integration.D365To3PL.Services.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;

namespace BlueCorp.Integration.D365To3PL.Services;

public class DataProcessingService : IDataProcessingService
{
    private readonly ILogger<DataProcessingService> _logger;
    private readonly ISftpService _sftpService;

    public DataProcessingService(ILogger<DataProcessingService> logger, ISftpService sftpService)
    {
        _logger = logger;
        _sftpService = sftpService;
    }

    public async Task ProcessD365EventAsync(DispatchEvent dispatchEvent)
    {
        _logger.LogInformation("Processing event with control number: {ControlNumber}", dispatchEvent.ControlNumber);

        try
        {
            // Convert to CSV format
            var thirdPartyCsvRecords = DispatchEventToThirdPartyCsvRecord.ConvertToCsvFormat(dispatchEvent);

            // Create the CSV file using CsvHelper
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
            using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));

            await csv.WriteRecordsAsync(thirdPartyCsvRecords);
            await writer.FlushAsync();

            var csvData = memoryStream.ToArray();

            // Create a unique filename
            var filename = $"bluecorp-dispatch-{dispatchEvent.ControlNumber}-{DateTime.UtcNow:yyyyMMddHHmmss}.csv";

            // Send the CSV file to the 3PL system
            await _sftpService.UploadFileAsync(csvData, filename);

            _logger.LogInformation("Successfully processed and uploaded file {FileName} with control number: {ControlNumber}", filename, dispatchEvent.ControlNumber);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event with control number: {ControlNumber}", dispatchEvent.ControlNumber);
            throw;
        }
    }
}