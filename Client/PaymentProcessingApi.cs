using PaymentProcessingSystem.Abstractions.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace Client
{
    public class PaymentProcessingApi
    {
        private readonly HttpClient _httpClient;

        public PaymentProcessingApi(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Api");
        }

        public async Task CancelPaymentAsync(ProcessPayment model, CancellationToken token = default)
        {
            await _httpClient.PostAsJsonAsync("api/v1.0/payments/cancel", model, token).ConfigureAwait(false);
        }

        public async Task RefundPaymentAsync(ProcessPayment model, CancellationToken token = default)
        {
            await _httpClient.PostAsJsonAsync("api/v1.0/payments/refund", model, token).ConfigureAwait(false);
        }

        public async Task ProcessPaymentAsync(ProcessPayment model, CancellationToken token = default)
        {
            await _httpClient.PostAsJsonAsync("api/v1.0/payments/send", model, token).ConfigureAwait(false);
        }
    }
}
