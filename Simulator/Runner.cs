using Client;
using PaymentProcessingSystem.Abstractions.Models;
using System.Net.Http.Json;

namespace Simulator
{
    internal class Runner
    {
        private readonly PaymentProcessingApi _apiClient;

        public Runner(PaymentProcessingApi apiClient)
        {
            apiClient = apiClient;
        }

        public async Task RunAsync()
        {
            var request = new ProcessPayment
            {
                UserId = Guid.NewGuid(),
                Amount = 100,
                PaymentMethod = "CreditCard"
            };

            await _apiClient.ProcessPaymentAsync(request);
        }
    }
}
