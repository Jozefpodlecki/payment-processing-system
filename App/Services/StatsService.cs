using MassTransit;
using PaymentProcessingSystem.Abstractions;
using PaymentProcessingSystem.Abstractions.Models;
using PaymentProcessingSystem.Models.Events;

namespace PaymentProcessingSystem.Services
{
    public class StatsService :
         IConsumer<PaymentProcessedEvent>,
         IConsumer<PaymentFailedEvent>,
         IConsumer<PaymentCancelledEvent>,
         IConsumer<PaymentRefundedEvent>,
         IConsumer<PaymentCompletedEvent>
    {
        private static PaymentProcessingStats _stats = new();
        private readonly ISystemClock _systemClock;

        public StatsService(ISystemClock systemClock)
        {
            _systemClock = systemClock;
        }

        public Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            _stats.CompletedCount++;
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<PaymentProcessedEvent> context)
        {
            _stats.ProcessedCount++;
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            _stats.FailedCount++;
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<PaymentCancelledEvent> context)
        {
            _stats.CancelledCount++;
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<PaymentRefundedEvent> context)
        {
            _stats.RefundedCount++;
            return Task.CompletedTask;
        }

        public PaymentProcessingStats GetStats()
        {
            _stats.UpdatedOn = _systemClock.UtcNow;
            return _stats;
        }
    }
}
