using MassTransit;
using MediatR;
using PaymentProcessingSystem.Abstractions;
using PaymentProcessingSystem.Gateways;
using PaymentProcessingSystem.Gateways.Models;
using PaymentProcessingSystem.Models;
using PaymentProcessingSystem.Models.Events;
using PaymentProcessingSystem.Models.Response;
using PaymentProcessingSystem.Repositories;
using PaymentProcessingSystem.Requests;
using PaymentProcessingSystem.Services;

namespace PaymentProcessingSystem.RequestHandlers
{
    public class ProcessPaymentRequestHandler : IRequestHandler<ProcessPaymentRequest, ProcessPaymentResponse>
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ISystemClock _systemClock;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IFraudDetectionService _fraudDetectionService;
        private readonly IPaymentGateway _paymentGateway;
        private readonly ILogger<ProcessPaymentRequestHandler> _logger;

        public ProcessPaymentRequestHandler(
            IGuidGenerator guidGenerator,
            IPaymentRepository paymentRepository,
            ISystemClock systemClock,
            IPublishEndpoint publishEndpoint,
            IFraudDetectionService fraudDetectionService,
            IPaymentGateway paymentGateway,
            ILogger<ProcessPaymentRequestHandler> logger)
        {
            _guidGenerator = guidGenerator ?? throw new ArgumentNullException(nameof(guidGenerator));
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _fraudDetectionService = fraudDetectionService ?? throw new ArgumentNullException(nameof(fraudDetectionService));
            _paymentGateway = paymentGateway ?? throw new ArgumentNullException(nameof(paymentGateway));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ProcessPaymentResponse> Handle(ProcessPaymentRequest request, CancellationToken cancellationToken)
        {
            var response = new ProcessPaymentResponse();
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
            response.PaymentId = id;

            var isFraudulent = await _fraudDetectionService.CheckForFraudAsync(request.UserId, request.Amount, request.PaymentMethod, cancellationToken);
            
            if (isFraudulent)
            {
                payment.Status = PaymentStatus.FraudDetected;
                await _paymentRepository.UpdateAsync(payment, cancellationToken);

                _logger.LogWarning("Fraud detected for Payment ID: {PaymentId}. Payment rejected.", id);
                response.Message = $"Fraud detected for Payment ID: {id}. Payment rejected.";

                return response;
            }

            var paymentGatewayProcessRequest = new PaymentGatewayProcessRequest
            {
                Amount = request.Amount,
                Currency = request.Currency,
                PaymentMethod = request.PaymentMethod
            };

            var paymentResult = await _paymentGateway.ProcessPaymentAsync(paymentGatewayProcessRequest, cancellationToken);

            if (paymentResult.IsSuccess)
            {
                payment.Status = PaymentStatus.Completed;
                response.IsSuccess = true;
                _logger.LogInformation("Payment processed successfully for Payment ID: {PaymentId}", id);
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
                response.Message = $"Payment failed for Payment ID: {id}. Reason: {paymentResult.ErrorMessage}";
                _logger.LogError("Payment failed for Payment ID: {PaymentId}. Reason: {Reason}", id, paymentResult.ErrorMessage);
            }

            await _paymentRepository.UpdateAsync(payment, cancellationToken);

            var eventObj = new PaymentProcessedEvent
            {
                PaymentId = id,
                UserId = request.UserId,
                Amount = request.Amount,
                Status = payment.Status,
                ProcessedOn = _systemClock.UtcNow
            };

            await _publishEndpoint.Publish(eventObj, cancellationToken);

            return response;
        }
    }
}
