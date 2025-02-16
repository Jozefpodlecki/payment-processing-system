
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using PaymentProcessingSystem.Abstractions;
using PaymentProcessingSystem.Abstractions.Models;
using PaymentProcessingSystem.Hubs;
using PaymentProcessingSystem.Models.Events;
using PaymentProcessingSystem.Services;
using Stripe.Reporting;

namespace PaymentProcessingSystem.HostedServices
{
    /// <summary>
    /// Worker that sends payment processing stats to the clients.
    /// </summary>
    public class StatsWorker : BackgroundService
    {
        private readonly IHubContext<StatsHub> _statsHub;
        private readonly IServiceProvider _serviceProvider;

        public StatsWorker(
            IHubContext<StatsHub> statsHub,
            IServiceProvider serviceProvider)
        {
            _statsHub = statsHub;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var serviceScope = _serviceProvider.CreateScope();
                var statsService = serviceScope.ServiceProvider.GetRequiredService<StatsService>();
                var methodName = "GetStats";

                await _statsHub.Clients.All.SendAsync(
                    methodName,
                    statsService.GetStats(),
                    stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
