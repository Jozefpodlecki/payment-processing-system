using Client;
using Client.Models;
using PaymentProcessingSystem.Abstractions.Models;
using System;

namespace Simulator
{
    internal class Runner
    {
        private readonly IPaymentProcessingApi _apiClient;
        private readonly Random _random;

        public Runner(
            Random random,
            IPaymentProcessingApi apiClient)
        {
            _random = random;
            _apiClient = apiClient;
        }

        decimal GenerateRandomDecimal(decimal minValue, decimal maxValue)
        {
            var range = (double)(maxValue - minValue);
            var randomDouble = _random.NextDouble() * range;
            return minValue + (decimal)randomDouble;
        }

        public async Task RunAsync()
        {
            var userIds = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            };

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

            var paymentIds = new List<(Guid, Guid)>();
            

            while (true)
            {
                Guid paymentId;
                Guid userId;
                var chance = _random.NextDouble();

                if(chance <= .5 && paymentIds.Count > 2)
                {
                    var tuple = paymentIds[_random.Next(0, paymentIds.Count)];
                    (paymentId, userId) = tuple;

                    var request = new CancelPayment
                    {
                        PaymentId = paymentId,
                        UserId = userId,
                    };

                    var response = await _apiClient.CancelPaymentAsync(request);

                    paymentIds.Remove(tuple);

                    await Task.Delay(TimeSpan.FromSeconds(2));
                    continue;
                }

                {
                    userId = userIds[_random.Next(0, userIds.Count)];
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

                    paymentId = response.PaymentId.Value;
                    paymentIds.Add((paymentId, userId));

                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
               
            }
        }
    }
}
