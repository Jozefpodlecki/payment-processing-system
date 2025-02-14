using Microsoft.Extensions.DependencyInjection;

namespace Client
{
    public static class Extensions
    {
        public static void AddPaymentProcessingApi(this IServiceCollection services)
        {
            services.AddSingleton<IPaymentProcessingApi, PaymentProcessingApi>();
        }
    }
}
