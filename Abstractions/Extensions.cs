using Microsoft.Extensions.DependencyInjection;

namespace PaymentProcessingSystem.Abstractions
{
    public static class Extensions
    {
        public static void AddAbstractions(this IServiceCollection services)
        {
            services.AddSingleton<ISystemClock, SystemClock>();
            //builder.Services.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
            services.AddSingleton<IGuidGenerator, SequentialGuidGenerator>();
        }
    }
}
