using Abstractions;
using MassTransit;
using MediatR;
using PaymentProcessingSystem.Events;
using PaymentProcessingSystem.Models;
using PaymentProcessingSystem.Models.Reponse;
using PaymentProcessingSystem.Repositories;
using PaymentProcessingSystem.Requests;

namespace PaymentProcessingSystem.RequestHandlers
{
    public class RefundPaymentRequestHandler : IRequestHandler<RefundPaymentRequest, RefundPaymentResponse>
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ISystemClock _systemClock;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<ProcessPaymentRequestHandler> _logger;

        public RefundPaymentRequestHandler(
            IGuidGenerator guidGenerator,
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

        public async Task<RefundPaymentResponse> Handle(RefundPaymentRequest request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, request.UserId);

            if (payment == null)
            {
                _logger.LogWarning($"Payment with ID {request.PaymentId} and UserId {request.UserId} not found.");
                return new RefundPaymentResponse
                {
                    IsSuccess = false,
                    Message = "Payment not found."
                };
            }

            if (payment.Status != PaymentStatus.Completed)
            {
                _logger.LogWarning($"Payment with ID {request.PaymentId} cannot be refunded because it is in {payment.Status} status.");
                return new RefundPaymentResponse
                {
                    IsSuccess = false,
                    Message = $"Payment cannot be refunded because it is in {payment.Status} status."
                };
            }

            var refund = new Refund
            {
                Id = _guidGenerator.NewGuid(),
                PaymentId = payment.Id,
                UserId = payment.UserId,
                Amount = request.Amount,
                Reason = request.Reason,
                ProcessedOn = _systemClock.UtcNow
            };

            payment.Status = PaymentStatus.Refunded;
            payment.RefundedOn = _systemClock.UtcNow;

            try
            {
                await _paymentRepository.SaveRefundAsync(refund, cancellationToken);
                await _paymentRepository.UpdateAsync(payment, cancellationToken);

                _logger.LogInformation($"Payment with ID {request.PaymentId} has been refunded.");

                var eventObj = new PaymentRefundedEvent
                {
                    PaymentId = payment.Id,
                    UserId = payment.UserId,
                    RefundId = refund.Id,
                    Amount = refund.Amount,
                    RefundedOn = refund.ProcessedOn
                };

                await _publishEndpoint.Publish(eventObj, cancellationToken);

                return new RefundPaymentResponse
                {
                    IsSuccess = true,
                    Message = "Payment refunded successfully.",
                    RefundId = refund.Id,
                    RefundAmount = refund.Amount,
                    RefundedOn = refund.ProcessedOn
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while refunding payment with ID {request.PaymentId}.");
                return new RefundPaymentResponse
                {
                    IsSuccess = false,
                    Message = "An error occurred while processing the refund."
                };
            }
        }
    }
}
