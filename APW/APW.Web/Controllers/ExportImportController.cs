using APW.Architecture.Services;
using APW.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace APW.Web.Controllers;

public class ExportImportController : Controller
{
    private readonly IExportImportService _exportImportService;
    private readonly IAiEnrichmentService _aiService;

    public ExportImportController(
        IExportImportService exportImportService,
        IAiEnrichmentService aiService)
    {
        _exportImportService = exportImportService;
        _aiService = aiService;
    }

    // Descargar JSON
    [HttpGet]
    public async Task<IActionResult> Download(int id)
    {
        var json = await _exportImportService.ExportToJsonAsync(id);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        return File(bytes, "application/json", $"sourceitem-{id}.json");
    }

    // Subir JSON
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return RedirectToAction("Noticias", "Home");

        using var reader = new StreamReader(file.OpenReadStream());
        var json = await reader.ReadToEndAsync();

        try
        {
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var title = "";
            var content = "";

            if (root.TryGetProperty("normalized", out var normalized))
            {
                if (normalized.TryGetProperty("title", out var titleProp))
                    title = titleProp.GetString() ?? "Sin título";

                if (normalized.TryGetProperty("content", out var contentProp))
                    content = contentProp.GetString() ?? "";
            }

            // Llamar a la IA
            var aiResult = await _aiService.EnrichAsync(title, content);

            // Guardar en Session (más confiable que TempData en redirects)
            HttpContext.Session.SetString("AiSummary", aiResult.Summary ?? "");
            HttpContext.Session.SetString("AiSentiment", aiResult.Sentiment ?? "");
            HttpContext.Session.SetString("AiReadingTime", aiResult.ReadingTimeMinutes.ToString());
            HttpContext.Session.SetString("AiKeywords", string.Join(",", aiResult.Keywords));
            HttpContext.Session.SetString("AiTitle", title);
        }
        catch (Exception ex)
        {
            HttpContext.Session.SetString("AiSummary", "Error al analizar: " + ex.Message);
        }

        // Importar la noticia normalmente
        await _exportImportService.ImportFromJsonAsync(json);

        return RedirectToAction("Noticias", "Home");
    }
}