using Client;
using Client.Models;
using Microsoft.Extensions.Logging;
using PaymentProcessingSystem.Abstractions.Models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Simulator
{
    internal class Runner
    {
        private readonly ILogger<Runner> _logger;
        private readonly IPaymentProcessingApi _apiClient;
        private readonly WebhookInvoker _webhookInvoker;
        private readonly Random _random;

        public Runner(
            ILogger<Runner> logger,
            IPaymentProcessingApi apiClient,
            WebhookInvoker webhookInvoker,
            Random random)
        {
            _logger = logger;
            _apiClient = apiClient;
            _webhookInvoker = webhookInvoker;
            _random = random;
        }

        public async Task RunAsync()
        {
            var userIds = new List<Guid>
            {
                Guid.Parse("a519e7a8-fb86-4aea-91a6-feeb20b4556e"),
                Guid.Parse("3d8165d4-ff42-4b0d-bdea-43e0e090f2be"),
                Guid.Parse("a10ea72d-bf9a-45be-829a-62012a0f3662"),
                Guid.Parse("d09366f4-79bb-4e97-aa2d-966cb9800fb7"),
                Guid.Parse("38d7a1d5-7bcc-4a8c-b319-e76195390556"),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            //Console.WriteLine(string.Join("\n", userIds));

            var paymentMethods = new List<string>
            {
                "Card",
                "PayPal",
                "Bitcoin",
                "ApplePay",
                "GooglePay",
                "Venmo",
                "CashApp",
                "Zelle",
                "Stripe"
            };

            var currencies = new List<Currency>
            {
                Currency.USD,
                Currency.EUR,
                Currency.GBP,
                Currency.JPY,
                Currency.CNY
            };

            var processedPayments = new List<ProcessedPayment>();
            var completedPayments = new List<ProcessedPayment>();
            var delay = TimeSpan.FromMilliseconds(500);

            while (true)
            {
                var chance = _random.NextDouble();

                if(processedPayments.Count > 5)
                {
                    await CompleteOrFailPaymentsViaWebHookAsync(processedPayments, completedPayments);
                    await CompleteOrFailPaymentsViaWebHookAsync(processedPayments, completedPayments);
                }

                if (chance <= .15 && completedPayments.Count > 2)
                {
                    var processedPayment = completedPayments[_random.Next(0, completedPayments.Count)];

                    var request = new RefundPayment
                    {
                        PaymentId = processedPayment.PaymentId,
                        UserId = processedPayment.UserId,
                        Amount = GenerateRandomDecimal(10, processedPayment.Amount),
                        Reason = GenerateRandomString(50)
                    };

                    var response = await _apiClient.RefundPaymentAsync(request);

                    completedPayments.Remove(processedPayment);

                    continue;
                }

                if (chance <= .35 && processedPayments.Count > 2)
                {
                    await CompleteOrFailPaymentsViaWebHookAsync(processedPayments, completedPayments);

                    continue;
                }

                if(chance <= .5 && processedPayments.Count > 2)
                {
                    var processedPayment = processedPayments[_random.Next(0, processedPayments.Count)];

                    var request = new CancelPayment
                    {
                        PaymentId = processedPayment.PaymentId,
                        UserId = processedPayment.UserId,
                        Reason = GenerateRandomString(50)
                    };

                    var response = await _apiClient.CancelPaymentAsync(request);

                    processedPayments.Remove(processedPayment);

                    await Task.Delay(delay);
                    continue;
                }

                {
                    var userId = userIds[_random.Next(0, userIds.Count)];
                    var paymentMethod = paymentMethods[_random.Next(0, paymentMethods.Count)];
                    var amount = GenerateRandomDecimal(10, 100);
                    var currency = currencies[_random.Next(0, currencies.Count)];

                    var request = new ProcessPayment
                    {
                        UserId = userId,
                        Amount = amount,
                        Currency = currency,
                        PaymentMethod = paymentMethod
                    };

                    var response = await _apiClient.ProcessPaymentAsync(request);

                    if(response.IsSuccess)
                    {
                        var paymentId = response.PaymentId.Value;
                        var processedPayment = new ProcessedPayment
                        {
                            PaymentId = paymentId,
                            UserId = userId,
                            Amount = amount
                        };

                        processedPayments.Add(processedPayment);
                    }
                    else
                    {
                        _logger.LogError(response.Message);
                    }

                    await Task.Delay(delay);
                }
               
            }
        }

        private async Task CompleteOrFailPaymentsViaWebHookAsync(IList<ProcessedPayment> processedPayments, IList<ProcessedPayment> completedPayments)
        {
            var processedPayment = processedPayments[_random.Next(0, processedPayments.Count)];
            var isSuccess = _random.NextDouble() > .25;

            var payload = new PaymentGatewayResponse
            {
                IsSuccess = isSuccess,
                PaymentId = processedPayment.PaymentId,
                UserId = processedPayment.UserId,
            };

            processedPayments.Remove(processedPayment);

            if (isSuccess)
            {
                completedPayments.Add(processedPayment);
            }

            await _webhookInvoker.InvokeAsync(payload);
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var randomBytes = new byte[length];

            _random.NextBytes(randomBytes);

            var result = new StringBuilder(length);

            foreach (byte b in randomBytes)
            {
                result.Append(chars[b % chars.Length]);
            }

            return result.ToString();
        }

        private decimal GenerateRandomDecimal(decimal minValue, decimal maxValue)
        {
            var range = (double)(maxValue - minValue);
            var randomDouble = _random.NextDouble() * range;
            return minValue + (decimal)randomDouble;
        }
    }
}
