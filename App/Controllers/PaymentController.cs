using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Internal;
using PaymentProcessingSystem.Abstractions.Models;
using PaymentProcessingSystem.Models;
using PaymentProcessingSystem.Requests;

namespace PaymentProcessingSystem.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/payments")]
//[Route("api/v1.0/payments")]
[ApiController]
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

    [HttpPost("cancel")]
    public async Task<IActionResult> CancelPaymentAsync([FromBody] CancelPayment model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var request = new CancelPaymentRequest
            {
                PaymentId = model.PaymentId,
                Reason = model.Reason,
                ProcessedOn = _systemClock.UtcNow
            };

            var result = await _mediator.Send(request);

            return Ok(new { Message = "Payment canceled successfully", PaymentId = model.PaymentId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while canceling the payment.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("refund")]
    public async Task<IActionResult> RefundPaymentAsync([FromBody] RefundPayment model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var request = new RefundPaymentRequest
            {
                PaymentId = model.PaymentId,
                Reason = model.Reason,
                ProcessedOn = _systemClock.UtcNow
            };

            var result = await _mediator.Send(request);

            return Ok(new { Message = "Refund processed successfully", RefundId = result.RefundId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the refund.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("send")]
    public async Task<IActionResult> MakePaymentAsync([FromBody] ProcessPayment model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
         catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the payment for user {UserId}", model.UserId);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
