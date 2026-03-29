using System.Text.Json;
using APW.Web.Models;

namespace APW.Web.Services;

public interface INewsService
{
    Task<List<NoticiaItemViewModel>> GetLatestAsync();
}

public class NewsService : INewsService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly ILogger<NewsService> _logger;

    public NewsService(HttpClient http, IConfiguration config, ILogger<NewsService> logger)
    {
        _http = http;
        _apiKey = config["NewsApi:ApiKey"] ?? "";
        _baseUrl = config["NewsApi:BaseUrl"] ?? "https://newsapi.org/v2";
        _logger = logger;
    }

    public async Task<List<NoticiaItemViewModel>> GetLatestAsync()
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            _logger.LogWarning("NewsApi:ApiKey no configurado.");
            return new();
        }

        try
        {
            var url = $"{_baseUrl}/top-headlines?country=us&pageSize=20&apiKey={_apiKey}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "APW-Grupo2/1.0");
            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("NewsAPI respondió {status}: {body}", (int)response.StatusCode, body);
                return new();
            }

            var doc = await response.Content.ReadFromJsonAsync<JsonDocument>();
            if (doc is null) return new();

            var result = new List<NoticiaItemViewModel>();
            foreach (var article in doc.RootElement.GetProperty("articles").EnumerateArray())
            {
                var title = article.TryGetProperty("title", out var t) ? t.GetString() ?? "" : "";
                if (string.IsNullOrWhiteSpace(title) || title == "[Removed]") continue;

                var articleUrl = article.TryGetProperty("url", out var u) ? u.GetString() ?? "" : "";

                var publishedAt = DateTime.UtcNow;
                if (article.TryGetProperty("publishedAt", out var p) &&
                    DateTime.TryParse(p.GetString(), out var dt))
                    publishedAt = dt;

                string? author = null;
                if (article.TryGetProperty("author", out var a) && a.ValueKind != JsonValueKind.Null)
                    author = a.GetString();

                var sourceName = "";
                if (article.TryGetProperty("source", out var src) &&
                    src.TryGetProperty("name", out var sn))
                    sourceName = sn.GetString() ?? "";

                result.Add(new NoticiaItemViewModel
                {
                    ExternalId = articleUrl,
                    Title = title,
                    Summary = article.TryGetProperty("description", out var d) ? d.GetString() ?? "" : "",
                    Url = articleUrl,
                    Author = author,
                    PublishedAt = publishedAt,
                    IsLive = true,
                    SourceName = sourceName
                });
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al llamar a NewsAPI.");
            return new();
        }
    }
}
