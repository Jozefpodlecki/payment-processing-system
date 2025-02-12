using MassTransit;
using MediatR;
using Microsoft.Extensions.Internal;
using PaymentProcessingSystem.Models;
using PaymentProcessingSystem.Repositories;
using PaymentProcessingSystem.Requests;

namespace PaymentProcessingSystem.RequestHandlers
{
    public class ProcessPaymentRequestHandler : IRequestHandler<ProcessPaymentRequest, bool>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ISystemClock _systemClock;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<ProcessPaymentRequestHandler> _logger;

        public ProcessPaymentRequestHandler(
            IPaymentRepository paymentRepository,
            ISystemClock systemClock,
            IPublishEndpoint publishEndpoint,
            ILogger<ProcessPaymentRequestHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _systemClock = systemClock;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<bool> Handle(ProcessPaymentRequest request, CancellationToken cancellationToken)
        {
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Amount = request.Amount,
                PaymentMethod = request.PaymentMethod,
                Status = PaymentStatus.Pending,
                CreatedOn = _systemClock.UtcNow
            };

            await _paymentRepository.SaveAsync(payment, cancellationToken);

            await _publishEndpoint.Publish<ProcessPaymentRequest>(request, cancellationToken);
            return true;
        }
    }
}
