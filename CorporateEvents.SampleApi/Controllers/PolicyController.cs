using CorporateEvents.Abstractions.Contracts;
using CorporateEvents.SampleApi.Events;
using Microsoft.AspNetCore.Mvc;

namespace CorporateEvents.SampleApi.Controllers;

[ApiController]
[Route("api/policies")]
public class PolicyController : ControllerBase
{
    private readonly IEventBus _eventBus;

    public PolicyController(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    [HttpPost]
    public async Task<IActionResult> IssuePolicy()
    {
        var policyEvent = new PolicyIssuedEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            PolicyNumber = "POL-10001",
            CustomerName = "Nilson Benitez",
            Email = "cliente@correo.com",
            Phone = "1234567890"
        };

        await _eventBus.Publish(policyEvent);

        return Ok("Evento publicado correctamente.");
    }
}