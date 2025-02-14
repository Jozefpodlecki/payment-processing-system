using Abstractions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using PaymentProcessingSystem.Abstractions;
using PaymentProcessingSystem.Consumers;
using PaymentProcessingSystem.Gateways;
using PaymentProcessingSystem.Repositories;
using PaymentProcessingSystem.Services;
using Stripe;

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
            x.UsingRabbitMq();
        });

        builder.Services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
        });

        //var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        //{
        //    cfg.ReceiveEndpoint("make-payment-event", ev =>
        //    {
        //        ev.Consumer<ProcessPaymentConsumer>();
        //    });
        //});

        builder.Services.AddMediatR(cf => cf.RegisterServicesFromAssembly(typeof(Program).Assembly));
        builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
        builder.Services.AddScoped<IFraudDetectionService, VoidFraudDetectionService>();
        builder.Services.AddAbstractions();
        builder.Services.AddPaymentGateway(builder.Configuration);
        builder.Services.AddSingleton(serviceProvider =>
        {
            var section = builder.Configuration.GetSection("CosmosDb");
            var connectionString = section["ConnectionString"];

            var client = new CosmosClient(connectionString);
            
            Task.Run(async () =>
            {
                var database = await client.CreateDatabaseIfNotExistsAsync("PaymentDB");
                await database.Database.CreateContainerIfNotExistsAsync("Payments", "/UserId");
            }).Wait();

            return client;
        });

        builder.Services.AddControllers();

        builder.Services.AddOpenApi();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.MapGet("/", () => "Test");

        app.Run();
    }
}