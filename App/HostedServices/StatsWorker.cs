
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using PaymentProcessingSystem.Abstractions.Models;
using PaymentProcessingSystem.Hubs;
using PaymentProcessingSystem.Models.Events;
using Stripe.Reporting;

namespace PaymentProcessingSystem.HostedServices
{
    public class StatsWorker : BackgroundService, IConsumer<PaymentCompletedEvent>
    {
        private readonly IHubContext<StatsHub> _statsHub;
        private readonly IServiceProvider _serviceProvider;
        private PaymentProcessingStats _stats;

        public StatsWorker(IHubContext<StatsHub> statsHub, IServiceProvider serviceProvider)
        {
            _statsHub = statsHub;
            _serviceProvider = serviceProvider;

            _stats = new PaymentProcessingStats
            {
                CompletedCount = 10,
                FailedCount = 2,
                ProcessedCount = 12
            };
        }

        public Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            _stats.CompletedCount++;
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //using var scope = _serviceProvider.CreateScope();

                var methodName = "GetStats";

                await _statsHub.Clients.All.SendAsync(
                    methodName,
                    _stats,
                    stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
