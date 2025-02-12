using MediatR;

namespace PaymentProcessingSystem.Requests
{
    public class ProcessPaymentRequest : IRequest<bool>
    {
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTimeOffset ProcessedOn { get; set; }
    }
}
