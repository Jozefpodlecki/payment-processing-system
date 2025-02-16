using Abstractions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using PaymentProcessingSystem.Abstractions;
using PaymentProcessingSystem.Consumers;
using PaymentProcessingSystem.Gateways;
using PaymentProcessingSystem.HostedServices;
using PaymentProcessingSystem.Hubs;
using PaymentProcessingSystem.Repositories;
using PaymentProcessingSystem.Services;
using Stripe;
using Stripe.Reporting;
using static PaymentProcessingSystem.Services.StubFraudDetectionService;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // TO-DO
        //var builder = DistributedApplication.CreateBuilder(args);
        //var cosmos = builder.AddAzureCosmosDB("cosmos-db")
        //    .RunAsEmulator(
        //        emulator =>
        //        {
        //            emulator.WithGatewayPort(7777);
        //        });

        builder.Configuration.AddUserSecrets<Program>();

        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<ProcessPaymentConsumer>();
            x.AddConsumer<StatsService>();
            x.UsingRabbitMq((context, configuration) =>
            {
                configuration.ConfigureEndpoints(context);
            });
        });

        builder.Services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
        });

        builder.Services.AddSignalR(opt =>
        {
            opt.EnableDetailedErrors = true;
        });
        builder.Services.AddHostedService<StatsWorker>();
        builder.Services.AddMediatR(cf => cf.RegisterServicesFromAssembly(typeof(Program).Assembly));
        builder.Services.AddRepositories();
        builder.Services.AddAbstractions();
        builder.Services.AddFraudDetectionService(builder.Configuration);
        builder.Services.AddPaymentGateway(builder.Configuration);
        builder.Services.AddCosmosDb(builder.Configuration);

        builder.Services.AddControllers();

        builder.Services.AddOpenApi();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseCors(config => config.WithOrigins("https://localhost:7108").AllowAnyHeader().WithMethods("GET"));
        //app.UseCors(config => 
        //    config
        //        .AllowAnyOrigin()
        //        .AllowAnyHeader()
        //        .AllowAnyMethod());

        app.MapControllers();

        app.MapGet("/", () => "Test");
        app.MapHub<StatsHub>("/stats");

        app.Run();
    }
}