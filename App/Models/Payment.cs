using Newtonsoft.Json;
using PaymentProcessingSystem.Abstractions.Models;

namespace PaymentProcessingSystem.Models
{
    public class Payment
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public string PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? FaileddOn { get; set; }
        public DateTimeOffset? CompletedOn { get; set; }
        public DateTimeOffset? CancelledOn { get; set; }
        public DateTimeOffset? RefundedOn { get; set; }
    }
}
