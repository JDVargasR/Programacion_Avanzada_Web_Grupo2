using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace APW.Architecture.Services;

public class AiEnrichmentService : IAiEnrichmentService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public AiEnrichmentService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["Claude:ApiKey"] ??
            throw new Exception("Claude API Key no configurada");
    }

    public async Task<AiEnrichmentResult> EnrichAsync(string title, string content)
    {
        var contentPreview = content.Length > 3000
            ? content[..3000]
            : content;

        var prompt = "Analiza este artículo y responde ÚNICAMENTE con JSON válido, " +
                     "sin texto adicional, sin comentarios, sin bloques de código. " +
                     "El JSON debe tener exactamente esta estructura: " +
                     "{\"summary\": \"resumen en 2-3 oraciones en español\", " +
                     "\"sentiment\": \"positive, neutral o negative\", " +
                     "\"keywords\": [\"palabra1\", \"palabra2\", \"palabra3\"], " +
                     "\"readingTimeMinutes\": 2} " +
                     "Título: " + title + " " +
                     "Contenido: " + contentPreview;

        var requestBody = new
        {
            model = "claude-haiku-4-5",
            max_tokens = 500,
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post,
            "https://api.anthropic.com/v1/messages");
        request.Headers.Add("x-api-key", _apiKey);
        request.Headers.Add("anthropic-version", "2023-06-01");
        request.Content = JsonContent.Create(requestBody);

        var response = await _http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new Exception($"Claude API error {(int)response.StatusCode}: {errorBody}");
        }

        var json = await response.Content
            .ReadFromJsonAsync<JsonDocument>();

        var text = json!.RootElement
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString()!
            .Trim();

        text = text
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();

        var result = JsonSerializer.Deserialize<AiEnrichmentResult>(text,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new AiEnrichmentResult();

        result.Provider = "claude";
        result.Model = "claude-haiku-4-5";
        result.GeneratedAt = DateTime.UtcNow;

        return result;
    }
}