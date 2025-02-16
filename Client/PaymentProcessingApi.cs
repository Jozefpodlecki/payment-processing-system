using Client.Models;
using PaymentProcessingSystem.Abstractions.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;

namespace Client
{
    public class PaymentProcessingApi : IPaymentProcessingApi
    {
        private readonly HttpClient _httpClient;

        public PaymentProcessingApi(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Api");
        }

        public async Task<ApiResponse> CancelPaymentAsync(CancelPayment model, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1.0/payments/cancel", model, cancellationToken).ConfigureAwait(false);
            return await HandleResponseAsync(response, cancellationToken);
        }

        public async Task<ApiResponse> RefundPaymentAsync(RefundPayment model, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1.0/payments/refund", model, cancellationToken).ConfigureAwait(false);
            return await HandleResponseAsync(response, cancellationToken);
        }

        public async Task<ApiResponse> ProcessPaymentAsync(ProcessPayment model, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1.0/payments/send", model, cancellationToken).ConfigureAwait(false);
            return await HandleResponseAsync(response, cancellationToken);
        }

        private async Task<ApiResponse> HandleResponseAsync(HttpResponseMessage httpResponse, CancellationToken cancellationToken)
        {
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = await httpResponse.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken);
                return content ?? new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Operation completed successfully."
                };
            }
            else
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = $"API request failed with status code: {httpResponse.StatusCode}",
                };
            }
        }
    }
}
