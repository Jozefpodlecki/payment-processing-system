using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using PaymentProcessingSystem.Gateways;
using PaymentProcessingSystem.Repositories;
using PaymentProcessingSystem.Services;
using Stripe;
using static PaymentProcessingSystem.Services.StubFraudDetectionService;

namespace Abstractions
{
    public static class Extensions
    {
        public static void AddCosmosDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(serviceProvider =>
            {
                var section = configuration.GetSection("CosmosDb");
                var connectionString = section["ConnectionString"];

                var client = new CosmosClient(connectionString);

                Task.Run(async () =>
                {
                    var databaseResponse = await client.CreateDatabaseIfNotExistsAsync("PaymentDB");
                    var database = databaseResponse.Database;

                    await database.CreateContainerIfNotExistsAsync("Payments", "/UserId");
                    await database.CreateContainerIfNotExistsAsync("Refunds", "/UserId");
                }).Wait();

                return client;
            });
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IRefundRepository, RefundRepository>();
        }

        public static void AddFraudDetectionService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IFraudDetectionService, StubFraudDetectionService>((sp) =>
            {
                var options = new StubFraudDetectionServiceOptions
                {
                    FraudulentUserIds = new HashSet<Guid>
                    {
                        Guid.Parse("a519e7a8-fb86-4aea-91a6-feeb20b4556e"),
                        Guid.Parse("3d8165d4-ff42-4b0d-bdea-43e0e090f2be"),
                        Guid.Parse("a10ea72d-bf9a-45be-829a-62012a0f3662"),
                    }
                };

                return new StubFraudDetectionService(options);
            });
        }

        public static void AddPaymentGateway(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IPaymentGateway, StubPaymentGateway>((sp) =>
            {
                var options = new StubPaymentGateway.StubPaymentGatewayOptions
                {
                    UserIdsToFail = new HashSet<Guid> 
                    {
                        Guid.Parse("d09366f4-79bb-4e97-aa2d-966cb9800fb7"),
                        Guid.Parse("38d7a1d5-7bcc-4a8c-b319-e76195390556"),
                    }
                };

                return new StubPaymentGateway(options);
            });
            //services.AddScoped<IPaymentGateway, StripePaymentGateway>((sp) =>
            //{
            //    var apiKey = configuration["Stripe:ApiKey"];
            //    StripeConfiguration.ApiKey = apiKey;

            //    return new StripePaymentGateway();
            //});
        }
    }
}
