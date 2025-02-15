using MediatR;

namespace PaymentProcessingSystem.Requests
{
    public class ProcessPaymentCompletedRequest : IRequest<bool>
    {
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset ProcessedOn { get; set; }
    }
}
