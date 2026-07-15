using CorporateEvents.Abstractions.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CorporateEvents.SampleApi.Controllers;

[ApiController]
[Route("api/metrics")]
public class MetricsController : ControllerBase
{
    private readonly IMetricsService _metricsService;

    public MetricsController(IMetricsService metricsService)
    {
        _metricsService = metricsService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _metricsService.GetMetricsAsync());
    }
}