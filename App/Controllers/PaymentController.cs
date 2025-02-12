using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Internal;
using PaymentProcessingSystem.Abstractions.Models;
using PaymentProcessingSystem.Requests;

namespace PaymentProcessingSystem.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentController : ControllerBase
{    
    private readonly ILogger<PaymentController> _logger;
    private readonly IMediator _mediator;
    private readonly ISystemClock _systemClock;

    public PaymentController(
        ILogger<PaymentController> logger,
        IMediator mediator,
        ISystemClock systemClock)
    {
        _logger = logger;
        _mediator = mediator;
        _systemClock = systemClock;
    }

    [HttpPost("Send")]
    public async Task<StatusCodeResult> MakePaymentAsync(ProcessPayment model)
    {
        var request = new ProcessPaymentRequest
        {
            Amount = model.Amount,
            PaymentMethod = model.PaymentMethod,
            UserId = model.UserId,
            ProcessedOn = _systemClock.UtcNow
        };

        await _mediator.Send(request);

        return Ok();
    }
}
