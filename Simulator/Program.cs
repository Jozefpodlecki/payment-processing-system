using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Simulator;

internal class Program
{
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("appsettings.json", optional: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddHttpClient("Api", client =>
                {
                    var configuration = context.Configuration;
                    var baseUrl = configuration["ApiSettings:BaseUrl"];
                    client.BaseAddress = new Uri(baseUrl!);

                });
                //.ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
                //{
                //    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                //});
                services.AddSingleton<Runner>();
                services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true)
                    .Build());
            });

    private static async Task Main(string[] args)
    {
        using IHost host = CreateHostBuilder(args).Build();
        var runner = host.Services.GetRequiredService<Runner>();
        await runner.RunAsync();
    }
}