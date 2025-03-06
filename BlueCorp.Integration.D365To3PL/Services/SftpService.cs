using System.Text;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BlueCorp.Integration.D365To3PL.Configurations;
using BlueCorp.Integration.D365To3PL.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace BlueCorp.Integration.D365To3PL.Services;

public class SftpService : ISftpService
{
    private readonly ILogger<SftpService> _logger;
    private readonly SecretClient _secretClient;
    private readonly SftpSettings _sftpSettings;

    public SftpService(ILogger<SftpService> logger, IOptions<SftpSettings> sftpSettings, IOptions<KeyVaultSettings> keyVaultSettings)
    {
        _logger = logger;
        _sftpSettings = sftpSettings.Value;
        _secretClient = new SecretClient(new Uri(keyVaultSettings.Value.Uri), new DefaultAzureCredential());
    }

    public async Task UploadFileAsync(byte[] fileData, string fileName)
    {
        _logger.LogInformation("Initiating SFTP upload for file: {FileName}", fileName);

        // Obtaining the private key from Azure Key Vault
        var privateKeySecret = await _secretClient.GetSecretAsync(_sftpSettings.PrivateKeySecretName);
        var privateKeyData = privateKeySecret.Value.Value;

        using var privateKeyStream = new MemoryStream(Encoding.UTF8.GetBytes(privateKeyData));
        var privateKey = new PrivateKeyFile(privateKeyStream);

        // Create an SFTP client
        using var client = new SftpClient(_sftpSettings.Host, _sftpSettings.Port, _sftpSettings.Username, new[] { privateKey });

        _logger.LogInformation("Connecting to SFTP server: {Host}", _sftpSettings.Host);
        client.Connect();

        // Verify that the target directory exists
        if (!client.Exists(_sftpSettings.IncomingDirectory))
        {
            _logger.LogWarning("Target directory does not exist: {Directory}", _sftpSettings.IncomingDirectory);
            throw new DirectoryNotFoundException($"Directory not found: {_sftpSettings.IncomingDirectory}");
        }

        // Upload the file
        var remoteFilePath = $"{_sftpSettings.IncomingDirectory}/{fileName}";
        _logger.LogInformation("Uploading file to: {Path}", remoteFilePath);

        using var memoryStream = new MemoryStream(fileData);
        client.UploadFile(memoryStream, remoteFilePath);

        client.Disconnect();
        _logger.LogInformation("File uploaded successfully: {FileName}", fileName);
    }
}