using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PaymentProcessingSystem.Abstractions;
using PaymentProcessingSystem.Abstractions.Models;
using PaymentProcessingSystem.Requests;

namespace PaymentProcessingSystem.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/webhook")]
[ApiController]
public class WebhookController : ControllerBase
{    
    private readonly ILogger<PaymentController> _logger;
    private readonly IMediator _mediator;
    private readonly ISystemClock _systemClock;

    public WebhookController(
        ILogger<PaymentController> logger,
        IMediator mediator,
        ISystemClock systemClock)
    {
        _logger = logger;
        _mediator = mediator;
        _systemClock = systemClock;
    }

    [HttpPost]
    public async Task<IActionResult> Post()
    {
        try
        {
            var json = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            var paymentResult = JsonConvert.DeserializeObject<PaymentGatewayResponse>(json);

            if(paymentResult.IsSuccess)
            {
                var request = new ProcessPaymentCompletedRequest
                {
                    PaymentId = paymentResult.PaymentId,
                    UserId = paymentResult.UserId
                };

                await _mediator.Send(request);
            }
            else
            {
                var request = new ProcessPaymentFailedRequest
                {
                    PaymentId = paymentResult.PaymentId,
                    UserId = paymentResult.UserId
                };

                await _mediator.Send(request);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the webhook.");
        }

        return Ok();
    }
}
