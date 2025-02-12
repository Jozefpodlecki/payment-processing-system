using PaymentProcessingSystem.Abstractions.Models;
using System.Net.Http.Json;

namespace Simulator
{
    internal class Runner
    {
        private readonly HttpClient _httpClient;

        public Runner(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Api");

        }

        public async Task RunAsync()
        {
            var request = new ProcessPayment
            {
                UserId = "123",
                Amount = 100,
                PaymentMethod = "CreditCard"
            };
            await _httpClient.PostAsJsonAsync("Payment/Send", request);
        }
    }
}
