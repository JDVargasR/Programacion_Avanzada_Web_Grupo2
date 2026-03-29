using APW.Architecture.Services;
using APW.Data.Models.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace APW.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SourceItemsController : ControllerBase
{
    private readonly ProyectoWebGrupo2Context _db;
    private readonly IAiEnrichmentService _ai;

    public SourceItemsController(ProyectoWebGrupo2Context db, IAiEnrichmentService ai)
    {
        _db = db;
        _ai = ai;
    }

    // GET /api/SourceItems
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.SourceItems
            .Include(x => x.Source)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        var result = items.Select(x => new
        {
            x.Id,
            x.Title,
            x.ExternalId,
            x.Category,
            x.IsPinned,
            x.CreatedAt,
            Summary = ExtractSummary(x.Json),
            SourceName = x.Source?.Name ?? ""
        });

        return Ok(result);
    }

    // GET /api/SourceItems/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _db.SourceItems
            .Include(x => x.Source)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (item is null) return NotFound();

        return Ok(new
        {
            item.Id,
            item.Title,
            item.ExternalId,
            item.Category,
            item.IsPinned,
            item.CreatedAt,
            item.Json,
            SourceName = item.Source?.Name ?? ""
        });
    }

    // POST /api/SourceItems/import
    [HttpPost("import")]
    public async Task<IActionResult> Import([FromBody] ImportRequest request)
    {
        // Guardia: el body debe ser un objeto JSON válido
        if (request.Data.ValueKind != JsonValueKind.Object)
            return BadRequest(new { error = "El cuerpo debe ser un objeto JSON." });

        var root = request.Data;

        // normalized es requerido
        if (!root.TryGetProperty("normalized", out var norm))
            return BadRequest(new { error = "Falta la sección requerida 'normalized'." });

        // normalized.title es requerido
        var title = norm.TryGetProperty("title", out var t) && t.ValueKind == JsonValueKind.String
            ? t.GetString() ?? "" : "";
        if (string.IsNullOrWhiteSpace(title))
            return BadRequest(new { error = "normalized.title es requerido y no puede estar vacío." });

        try
        {
            var externalId = norm.TryGetProperty("externalId", out var eid) && eid.ValueKind == JsonValueKind.String
                ? eid.GetString() ?? "" : "";
            var content = norm.TryGetProperty("content", out var c) && c.ValueKind == JsonValueKind.String
                ? c.GetString() ?? "" : "";

            var category = "";
            if (norm.TryGetProperty("category", out var cat) &&
                cat.ValueKind == JsonValueKind.Object &&
                cat.TryGetProperty("primary", out var primary) &&
                primary.ValueKind == JsonValueKind.String)
                category = primary.GetString() ?? "";

            var sourceName = "Importados";
            if (root.TryGetProperty("source", out var src) &&
                src.TryGetProperty("name", out var n) && n.ValueKind == JsonValueKind.String)
                sourceName = n.GetString() ?? "Importados";

            // Enriquecer categoría con IA si no viene en el JSON (con fallback seguro)
            if (string.IsNullOrWhiteSpace(category) && !string.IsNullOrWhiteSpace(content))
            {
                try
                {
                    var aiResult = await _ai.EnrichAsync(title, content);
                    category = MapToCategory(aiResult.Keywords);
                }
                catch { /* IA no disponible, continuar sin categoría */ }
            }

            // Buscar o crear Source
            var source = await _db.Sources.FirstOrDefaultAsync(x => x.Name == sourceName)
                      ?? await _db.Sources.FirstOrDefaultAsync(x => x.Name == "Importados");
            if (source is null)
            {
                source = new Source
                {
                    Name = "Importados",
                    Url = "internal://import",
                    ComponentType = "import",
                    RequiresSecret = false
                };
                _db.Sources.Add(source);
                await _db.SaveChangesAsync();
            }

            // Evitar duplicados por externalId (para importados y fijados por separado)
            if (!string.IsNullOrWhiteSpace(externalId))
            {
                var exists = await _db.SourceItems.AnyAsync(
                    x => x.ExternalId == externalId && x.IsPinned == request.IsPinned);
                if (exists)
                    return Ok(new { id = 0, duplicate = true });
            }

            var item = new SourceItem
            {
                SourceId   = source.Id,
                Title      = title,
                ExternalId = externalId,
                IsPinned   = request.IsPinned,
                Category   = category,
                Json       = root.GetRawText(),
                CreatedAt  = DateTime.UtcNow
            };

            _db.SourceItems.Add(item);
            await _db.SaveChangesAsync();

            return Ok(new { id = item.Id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private static string ExtractSummary(string json)
    {
        try
        {
            var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("normalized", out var norm))
            {
                if (norm.TryGetProperty("summary", out var s) && s.ValueKind != JsonValueKind.Null)
                {
                    var summary = s.GetString() ?? "";
                    if (!string.IsNullOrEmpty(summary)) return summary;
                }
                if (norm.TryGetProperty("content", out var c) && c.ValueKind != JsonValueKind.Null)
                {
                    var content = c.GetString() ?? "";
                    return content.Length > 200 ? content[..200] + "…" : content;
                }
            }
        }
        catch { }
        return "";
    }

    private static readonly string[] KnownCategories =
        ["Política", "Economía", "Cultura", "Deportes", "Tecnología", "Salud", "Ciencia", "Internacional"];

    private static string MapToCategory(List<string> keywords)
    {
        foreach (var kw in keywords)
        {
            var match = KnownCategories.FirstOrDefault(c =>
                kw.Contains(c, StringComparison.OrdinalIgnoreCase) ||
                c.Contains(kw, StringComparison.OrdinalIgnoreCase));
            if (match is not null) return match;
        }
        return keywords.FirstOrDefault() ?? "";
    }
}

public class ImportRequest
{
    public JsonElement Data { get; set; }
    public bool IsPinned { get; set; }
}
