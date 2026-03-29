namespace APW.Web.Models;

public class SourceItemExportModel
{
    public string SchemaVersion { get; set; } = "edu.univ.ingest.v1";
    public DateTime ExportedAt { get; set; } = DateTime.UtcNow;
    public SourceExportModel Source { get; set; } = new();
    public NormalizedExportModel Normalized { get; set; } = new();
    public AiEnrichmentExportModel? AiEnrichment { get; set; }
    public RawExportModel Raw { get; set; } = new();
}

public class SourceExportModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public string Url { get; set; } = "";
    public bool RequiresSecret { get; set; }
}

public class NormalizedExportModel
{
    public int Id { get; set; }
    public string ExternalId { get; set; } = "";
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string? Summary { get; set; }
    public DateTime PublishedAt { get; set; }
    public string Url { get; set; } = "";
    public string? Author { get; set; }
    public string Language { get; set; } = "es";
}

public class AiEnrichmentExportModel
{
    public string Provider { get; set; } = "claude";
    public string Model { get; set; } = "";
    public string Summary { get; set; } = "";
    public string Sentiment { get; set; } = "";
    public List<string> Keywords { get; set; } = new();
    public int ReadingTimeMinutes { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class RawExportModel
{
    public string Format { get; set; } = "json";
    public object Data { get; set; } = new();
}