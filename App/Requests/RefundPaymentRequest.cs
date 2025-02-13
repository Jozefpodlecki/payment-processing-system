using MediatR;
using PaymentProcessingSystem.Models.Reponse;

namespace PaymentProcessingSystem.Requests
{
    public class RefundPaymentRequest : IRequest<RefundPaymentResponse>
    {
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
        public string Reason { get; set; }
        public DateTimeOffset ProcessedOn { get; set; }
        public decimal Amount { get; set; }
    }
}
