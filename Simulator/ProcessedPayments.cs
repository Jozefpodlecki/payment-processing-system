using PaymentProcessingSystem.Abstractions.Models;
using System.ComponentModel.DataAnnotations;

namespace Simulator
{
    internal class ProcessedPayments
    {
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
    }
}
