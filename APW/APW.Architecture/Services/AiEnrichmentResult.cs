using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APW.Architecture.Services;

public class AiEnrichmentResult
{
    public string Provider { get; set; } = "claude";
    public string Model { get; set; } = "claude-haiku-4-5-20251001";
    public string Summary { get; set; } = "";
    public string Sentiment { get; set; } = "";
    public List<string> Keywords { get; set; } = new();
    public int ReadingTimeMinutes { get; set; }
    public DateTime GeneratedAt { get; set; }
}
