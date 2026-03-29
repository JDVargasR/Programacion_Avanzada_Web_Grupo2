namespace APW.Web.Models;

public class NoticiaItemViewModel
{
    public int? LocalId { get; set; }
    public string ExternalId { get; set; } = "";
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public string Category { get; set; } = "";
    public string Url { get; set; } = "";
    public string? Author { get; set; }
    public DateTime PublishedAt { get; set; }
    public bool IsPinned { get; set; }
    public bool IsImported { get; set; }
    public bool IsLive { get; set; }
    public string SourceName { get; set; } = "";
}

public class NoticiasViewModel
{
    public List<NoticiaItemViewModel> ImportedItems { get; set; } = new();
    public List<NoticiaItemViewModel> PinnedItems { get; set; } = new();
    public List<NoticiaItemViewModel> LiveItems { get; set; } = new();

    public IEnumerable<NoticiaItemViewModel> AllItems =>
        ImportedItems.Concat(PinnedItems).Concat(LiveItems);

    public int TotalLocales => ImportedItems.Count + PinnedItems.Count;
    public int TotalLive => LiveItems.Count;
}

// DTO que mapea la respuesta de GET /api/SourceItems
public record LocalItemDto(
    int Id,
    string? Title,
    string? ExternalId,
    string? Category,
    string? Summary,
    bool IsPinned,
    DateTime CreatedAt,
    string? SourceName);
