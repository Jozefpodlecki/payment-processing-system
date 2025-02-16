using MassTransit;
using PaymentProcessingSystem.Models.Events;

namespace PaymentProcessingSystem.Consumers
{
    public class ProcessPaymentConsumer : IConsumer<PaymentProcessedEvent>
    {
        private readonly ILogger<ProcessPaymentConsumer> _logger;

        public ProcessPaymentConsumer(ILogger<ProcessPaymentConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<PaymentProcessedEvent> context)
        {
            _logger.LogInformation("Processing payment for user {} with amount {} using {}",
                context.Message.UserId,
                context.Message.Amount,
                context.Message.PaymentMethod);

            return Task.CompletedTask;
        }
    }
}
