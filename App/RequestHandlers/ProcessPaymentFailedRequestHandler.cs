using MassTransit;
using MediatR;
using PaymentProcessingSystem.Abstractions;
using PaymentProcessingSystem.Gateways;
using PaymentProcessingSystem.Models;
using PaymentProcessingSystem.Models.Events;
using PaymentProcessingSystem.Repositories;
using PaymentProcessingSystem.Requests;
using PaymentProcessingSystem.Services;

namespace PaymentProcessingSystem.RequestHandlers
{
    public class ProcessPaymentFailedRequestHandler : IRequestHandler<ProcessPaymentFailedRequest, bool>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ISystemClock _systemClock;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IFraudDetectionService _fraudDetectionService;
        private readonly IPaymentGateway _paymentGateway;
        private readonly ILogger<ProcessPaymentRequestHandler> _logger;

        public ProcessPaymentFailedRequestHandler(
            IPaymentRepository paymentRepository,
            ISystemClock systemClock,
            IPublishEndpoint publishEndpoint,
            IFraudDetectionService fraudDetectionService,
            IPaymentGateway paymentGateway,
            ILogger<ProcessPaymentRequestHandler> logger)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _fraudDetectionService = fraudDetectionService ?? throw new ArgumentNullException(nameof(fraudDetectionService));
            _paymentGateway = paymentGateway ?? throw new ArgumentNullException(nameof(paymentGateway));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessPaymentFailedRequest request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, request.UserId, cancellationToken);

            payment.Status = PaymentStatus.Failed;
            payment.FaileddOn = _systemClock.UtcNow;

            var eventObj = new PaymentFailedEvent
            {
                PaymentId = request.PaymentId,
                UserId = request.UserId,
            };

            await _publishEndpoint.Publish(eventObj, cancellationToken);

            return true;
        }
    }
}
