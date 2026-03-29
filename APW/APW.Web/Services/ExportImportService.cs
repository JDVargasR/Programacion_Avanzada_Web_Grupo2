using System.Text;
using System.Text.Json;
using APW.Web.Models;
using Microsoft.Extensions.Configuration;

namespace APW.Web.Services;

public interface IExportImportService
{
    Task<string> ExportToJsonAsync(int sourceItemId);
    Task<bool> ImportFromJsonAsync(string json);
}

public class ExportImportService : IExportImportService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _http;
    private readonly string _apiBase;

    public ExportImportService(IConfiguration config, HttpClient http)
    {
        _config = config;
        _http = http;
        _apiBase = config["ApiSettings:BaseUrl"] ?? "https://localhost:7000";
    }

    public async Task<string> ExportToJsonAsync(int sourceItemId)
    {
        // Obtener el SourceItem del API
        var response = await _http.GetAsync(
            $"{_apiBase}/api/SourceItems/{sourceItemId}");
        response.EnsureSuccessStatusCode();

        var sourceItem = await response.Content
            .ReadFromJsonAsync<dynamic>();

        // Llamar al servicio de IA para enriquecer
        var enrichRequest = new
        {
            title = sourceItem?.GetProperty("title").GetString() ?? "",
            content = sourceItem?.GetProperty("json").GetString() ?? ""
        };

        var enrichResponse = await _http.PostAsJsonAsync(
            $"{_apiBase}/api/AiEnrichment/enrich", enrichRequest);

        AiEnrichmentExportModel? aiEnrichment = null;
        if (enrichResponse.IsSuccessStatusCode)
        {
            aiEnrichment = await enrichResponse.Content
                .ReadFromJsonAsync<AiEnrichmentExportModel>();
        }

        // Construir el JSON final
        var export = new SourceItemExportModel
        {
            ExportedAt = DateTime.UtcNow,
            AiEnrichment = aiEnrichment
        };

        return JsonSerializer.Serialize(export, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    public async Task<bool> ImportFromJsonAsync(string json)
    {
        try
        {
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Importar al API — el aiEnrichment se ignora si el otro grupo no lo tiene
            var response = await _http.PostAsJsonAsync(
                $"{_apiBase}/api/SourceItems/import", root);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}