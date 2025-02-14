using MediatR;
using PaymentProcessingSystem.Abstractions.Models;
using PaymentProcessingSystem.Models.Response;

namespace PaymentProcessingSystem.Requests
{
    public class ProcessPaymentRequest : IRequest<ProcessPaymentResponse>
    {
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public string PaymentMethod { get; set; }
        public DateTimeOffset ProcessedOn { get; set; }
    }
}
