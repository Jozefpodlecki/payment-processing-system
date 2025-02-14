using Microsoft.Extensions.DependencyInjection;
using PaymentProcessingSystem.Gateways;
using Stripe;

namespace Abstractions
{
    public static class Extensions
    {
        public static void AddPaymentGateway(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IPaymentGateway, VoidPaymentGateway>();
            //services.AddScoped<IPaymentGateway, StripePaymentGateway>((sp) =>
            //{
            //    var apiKey = configuration["Stripe:ApiKey"];
            //    StripeConfiguration.ApiKey = apiKey;

            //    return new StripePaymentGateway();
            //});
        }
    }
}
