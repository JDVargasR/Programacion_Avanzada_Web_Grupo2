using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APW.Architecture.Services;

public interface IAiEnrichmentService
{
    Task<AiEnrichmentResult> EnrichAsync(string title, string content);
}
