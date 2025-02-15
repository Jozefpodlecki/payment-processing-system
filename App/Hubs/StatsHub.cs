using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.RecordIO;

namespace PaymentProcessingSystem.Hubs
{
    public class PaymentStats
    {
        public int TotalPayments { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class StatsHub : Hub<PaymentStats>
    {
    }
}
