using Abstractions;
using MassTransit;
using MediatR;
using PaymentProcessingSystem.Models;
using PaymentProcessingSystem.Repositories;
using PaymentProcessingSystem.Requests;

namespace PaymentProcessingSystem.RequestHandlers
{
    public class ProcessPaymentRequestHandler : IRequestHandler<ProcessPaymentRequest, Guid>
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ISystemClock _systemClock;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<ProcessPaymentRequestHandler> _logger;

        public ProcessPaymentRequestHandler(
            IGuidGenerator guidGenerator,
            IPaymentRepository paymentRepository,
            ISystemClock systemClock,
            IPublishEndpoint publishEndpoint,
            ILogger<ProcessPaymentRequestHandler> logger)
        {
            _guidGenerator = guidGenerator;
            _paymentRepository = paymentRepository;
            _systemClock = systemClock;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<Guid> Handle(ProcessPaymentRequest request, CancellationToken cancellationToken)
        {
            var id = _guidGenerator.NewGuid();

            var payment = new Payment
            {
                Id = id,
                UserId = request.UserId,
                Amount = request.Amount,
                PaymentMethod = request.PaymentMethod,
                Status = PaymentStatus.Pending,
                CreatedOn = _systemClock.UtcNow
            };

            await _paymentRepository.SaveAsync(payment, cancellationToken);

            await _publishEndpoint.Publish(request, cancellationToken);
            return id;
        }
    }
}
