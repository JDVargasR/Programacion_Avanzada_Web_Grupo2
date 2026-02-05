using System;
using System.Collections.Generic;

namespace APW.Web.Models.DB;

public partial class Source
{
    public int Id { get; set; }

    public string Url { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string ComponentType { get; set; } = null!;

    public bool RequiresSecret { get; set; }

    public virtual ICollection<SourceItem> SourceItems { get; set; } = new List<SourceItem>();
}
