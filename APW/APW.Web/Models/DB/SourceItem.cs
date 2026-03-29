using System;
using System.Collections.Generic;

namespace APW.Web.Models.DB;

public partial class SourceItem
{
    public int Id { get; set; }

    public int SourceId { get; set; }

    public string Json { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? Title { get; set; }

    public string? ExternalId { get; set; }

    public bool IsPinned { get; set; }

    public string? Category { get; set; }

    public virtual Source Source { get; set; } = null!;
}
