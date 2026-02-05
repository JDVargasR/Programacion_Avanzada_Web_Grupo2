using System;
using System.Collections.Generic;

namespace APW.Web.Models.DB;

public partial class SourceItem
{
    public int Id { get; set; }

    public int SourceId { get; set; }

    public string Json { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Source Source { get; set; } = null!;
}
