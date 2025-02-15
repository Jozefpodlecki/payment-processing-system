using MediatR;

namespace PaymentProcessingSystem.Requests
{
    public class ProcessPaymentFailedRequest : IRequest<bool>
    {
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset ProcessedOn { get; set; }
    }
}
