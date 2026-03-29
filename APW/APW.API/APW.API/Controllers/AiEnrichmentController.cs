using APW.Architecture.Services;
using Microsoft.AspNetCore.Mvc;

namespace APW.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiEnrichmentController : ControllerBase
{
    private readonly IAiEnrichmentService _aiService;

    public AiEnrichmentController(IAiEnrichmentService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("enrich")]
    public async Task<IActionResult> Enrich([FromBody] EnrichRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest("El contenido es requerido");

        var result = await _aiService.EnrichAsync(request.Title, request.Content);
        return Ok(result);
    }
}

public record EnrichRequest(string Title, string Content);