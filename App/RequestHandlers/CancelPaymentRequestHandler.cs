using MassTransit;
using MediatR;
using PaymentProcessingSystem.Abstractions;
using PaymentProcessingSystem.Models;
using PaymentProcessingSystem.Models.Events;
using PaymentProcessingSystem.Models.Response;
using PaymentProcessingSystem.Repositories;
using PaymentProcessingSystem.Requests;

namespace PaymentProcessingSystem.RequestHandlers
{
    public class CancelPaymentRequestHandler : IRequestHandler<CancelPaymentRequest, CancelPaymentResponse>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ISystemClock _systemClock;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<ProcessPaymentRequestHandler> _logger;

        public CancelPaymentRequestHandler(
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

        public async Task<CancelPaymentResponse> Handle(CancelPaymentRequest request, CancellationToken cancellationToken)
        {
            var response = new CancelPaymentResponse();
            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, request.UserId);

            if (payment == null)
            {
                _logger.LogWarning($"Payment with ID {request.PaymentId} and UserId {request.UserId} not found.");
                response.IsSuccess = false;
                response.Message = "Payment not found.";

                return response;
            }

            if (payment.Status != PaymentStatus.Pending)
            {
                _logger.LogWarning($"Payment with ID {request.PaymentId} cannot be canceled because it is in {payment.Status} status.");
                response.IsSuccess = false;
                response.Message = $"Payment cannot be canceled because it is in {payment.Status} status.";

                return response;
            }

            payment.Status = PaymentStatus.Cancelled;
            payment.CancelledOn = _systemClock.UtcNow;

            try
            {
                await _paymentRepository.UpdateAsync(payment, cancellationToken);
                _logger.LogInformation($"Payment with ID {request.PaymentId} has been canceled.");

                var eventObj = new PaymentCancelledEvent
                {
                    PaymentId = payment.Id,
                    UserId = payment.UserId,
                    CancelledOn = payment.CancelledOn.Value
                };

                await _publishEndpoint.Publish(eventObj, cancellationToken);

                response.IsSuccess = true;
                response.Message = "Payment canceled successfully.";
                response.UpdatedStatus = payment.Status;
                response.CancelledOn = payment.CancelledOn;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while canceling payment with ID {request.PaymentId}.");
                response.IsSuccess = false;
                response.Message = "An error occurred while canceling the payment.";

                return response;
            }
        }
    }
}
