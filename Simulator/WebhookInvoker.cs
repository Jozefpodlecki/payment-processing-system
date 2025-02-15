using PaymentProcessingSystem.Abstractions.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Simulator
{
    public class WebhookInvoker
    {
        private readonly HttpClient _httpClient;

        public WebhookInvoker(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Api");
        }

        public async Task InvokeAsync(PaymentGatewayResponse model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));

            var response = await _httpClient.PostAsync("api/v1.0/webhook", content);
            response.EnsureSuccessStatusCode();
        }
    }
}
