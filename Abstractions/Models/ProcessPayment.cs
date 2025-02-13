using System.ComponentModel.DataAnnotations;

namespace PaymentProcessingSystem.Abstractions.Models
{
    public class ProcessPayment
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentMethod { get; set; }
    }
}
