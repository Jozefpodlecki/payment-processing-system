using Newtonsoft.Json;

namespace PaymentProcessingSystem.Models
{
    public class Payment
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
