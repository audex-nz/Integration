using BlueCorp.Integration.D365To3PL.Authentication;
using BlueCorp.Integration.D365To3PL.Configurations;
using BlueCorp.Integration.D365To3PL.Services;
using BlueCorp.Integration.D365To3PL.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        // Register configurations
        var configuration = context.Configuration;
        services.Configure<SftpSettings>(configuration.GetSection("Sftp"));
        services.Configure<OAuthSettings>(configuration.GetSection("OAuth"));
        services.Configure<StorageSettings>(configuration.GetSection("Storage"));
        services.Configure<KeyVaultSettings>(configuration.GetSection("KeyVault"));

        // Register services
        services.AddSingleton<AuthenticationHelper>();
        services.AddScoped<ISftpService, SftpService>();
        services.AddScoped<IDataProcessingService, DataProcessingService>();

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();
