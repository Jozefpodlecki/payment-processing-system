using MassTransit;
using PaymentProcessingSystem.RequestHandlers;
using PaymentProcessingSystem.Requests;

namespace PaymentProcessingSystem.Consumers
{
    public class ProcessPaymentConsumer : IConsumer<ProcessPaymentRequest>
    {
        private readonly ILogger<ProcessPaymentRequestHandler> _logger;

        public ProcessPaymentConsumer(ILogger<ProcessPaymentRequestHandler> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ProcessPaymentRequest> context)
        {
            _logger.LogInformation("Processing payment for user {} with amount {} using {}", context.Message.UserId, context.Message.Amount, context.Message.PaymentMethod);

            return Task.CompletedTask;
        }
    }
}
