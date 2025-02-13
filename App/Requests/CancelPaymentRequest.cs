using MediatR;
using PaymentProcessingSystem.Models.Response;

namespace PaymentProcessingSystem.Requests
{
    public class CancelPaymentRequest : IRequest<CancelPaymentResponse>
    {
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
        public string Reason { get; set; }
        public DateTimeOffset ProcessedOn { get; set; }
    }
}
