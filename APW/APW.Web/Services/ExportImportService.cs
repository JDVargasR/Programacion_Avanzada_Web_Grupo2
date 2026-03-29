using System.Text.Json;
using System.Text.Json.Nodes;
using APW.Web.Models;
using Microsoft.Extensions.Configuration;

namespace APW.Web.Services;

public interface IExportImportService
{
    Task<string> ExportToJsonAsync(int sourceItemId);
    Task<bool> ImportFromJsonAsync(string json, bool isPinned = false);
    Task<List<LocalItemDto>> GetLocalItemsAsync();
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
        var response = await _http.GetAsync($"{_apiBase}/api/SourceItems/{sourceItemId}");
        response.EnsureSuccessStatusCode();

        var doc = await response.Content.ReadFromJsonAsync<JsonDocument>();
        if (doc is null) throw new Exception("Item no encontrado");

        // Si el Json almacenado ya es un templateV2 válido, devolverlo
        if (doc.RootElement.TryGetProperty("json", out var jsonProp) &&
            jsonProp.ValueKind != JsonValueKind.Null)
        {
            var stored = jsonProp.GetString() ?? "";
            if (!string.IsNullOrWhiteSpace(stored))
            {
                try
                {
                    var storedDoc = JsonDocument.Parse(stored);
                    if (storedDoc.RootElement.TryGetProperty("schemaVersion", out _))
                    {
                        // Inyectar normalized.category desde la columna DB si el blob no lo tiene
                        var dbCategory = doc.RootElement.TryGetProperty("category", out var cat) &&
                                         cat.ValueKind == JsonValueKind.String
                            ? cat.GetString() : null;

                        var node = JsonNode.Parse(stored);
                        if (!string.IsNullOrWhiteSpace(dbCategory) &&
                            node?["normalized"] is JsonObject normObj &&
                            normObj["category"] is null)
                        {
                            normObj["category"] = new JsonObject
                            {
                                ["primary"]   = dbCategory,
                                ["secondary"] = new JsonArray()
                            };
                        }

                        return node?.ToJsonString(new JsonSerializerOptions { WriteIndented = true })
                               ?? stored;
                    }
                }
                catch { }
            }
        }

        // Fallback: construir export mínimo desde metadatos
        var title = doc.RootElement.TryGetProperty("title", out var t) ? t.GetString() ?? "" : "";
        var category = doc.RootElement.TryGetProperty("category", out var c) ? c.GetString() ?? "" : "";

        var export = new SourceItemExportModel
        {
            ExportedAt = DateTime.UtcNow,
            Source = new SourceExportModel { Name = "APW.Grupo2", Type = "local", Url = "" },
            Normalized = new NormalizedExportModel
            {
                Id = sourceItemId,
                Title = title,
                Content = "",
                Category = string.IsNullOrEmpty(category)
                    ? null
                    : new CategoryExportModel { Primary = category }
            }
        };

        return JsonSerializer.Serialize(export, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    public async Task<bool> ImportFromJsonAsync(string json, bool isPinned = false)
    {
        try
        {
            var doc = JsonDocument.Parse(json);
            var request = new { data = doc.RootElement, isPinned };
            var response = await _http.PostAsJsonAsync(
                $"{_apiBase}/api/SourceItems/import", request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<LocalItemDto>> GetLocalItemsAsync()
    {
        try
        {
            var items = await _http.GetFromJsonAsync<List<LocalItemDto>>(
                $"{_apiBase}/api/SourceItems");
            return items ?? new();
        }
        catch
        {
            return new();
        }
    }
}
