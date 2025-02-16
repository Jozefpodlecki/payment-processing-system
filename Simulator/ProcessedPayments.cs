namespace Simulator
{
    internal class ProcessedPayment
    {
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
    }
}
