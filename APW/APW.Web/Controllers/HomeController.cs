using APW.Web.Models;
using APW.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace APW.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INewsService _newsService;
        private readonly IExportImportService _exportImportService;

        public HomeController(
            ILogger<HomeController> logger,
            INewsService newsService,
            IExportImportService exportImportService)
        {
            _logger = logger;
            _newsService = newsService;
            _exportImportService = exportImportService;
        }

        public IActionResult Index()
        {
            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre") ?? "lector";
            return View();
        }

        public IActionResult Privacy() => View();

        public async Task<IActionResult> Noticias()
        {
            var vm = new NoticiasViewModel();

            // Items locales (importados + fijados) desde la API
            var localItems = await _exportImportService.GetLocalItemsAsync();
            foreach (var item in localItems)
            {
                var noticia = new NoticiaItemViewModel
                {
                    LocalId      = item.Id,
                    ExternalId   = item.ExternalId ?? "",
                    Title        = item.Title ?? "Sin título",
                    Summary      = item.Summary ?? "",
                    Category     = item.Category ?? "",
                    PublishedAt  = item.CreatedAt,
                    IsPinned     = item.IsPinned,
                    IsImported   = !item.IsPinned,
                    SourceName   = item.SourceName ?? ""
                };
                if (item.IsPinned) vm.PinnedItems.Add(noticia);
                else vm.ImportedItems.Add(noticia);
            }

            // Items en vivo desde la API externa
            vm.LiveItems = await _newsService.GetLatestAsync();

            return View(vm);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Pin(
            string externalId, string title, string summary,
            string url, string? author, string? sourceName)
        {
            var export = new
            {
                schemaVersion = "edu.univ.ingest.v1",
                exportedAt    = DateTime.UtcNow,
                source = new
                {
                    name           = sourceName ?? "NewsAPI",
                    type           = "api",
                    url            = url,
                    requiresSecret = false
                },
                normalized = new
                {
                    externalId,
                    title,
                    content     = summary,
                    summary,
                    publishedAt = DateTime.UtcNow,
                    url,
                    author,
                    language    = "es"
                },
                raw = new { format = "json", data = new { original = summary } }
            };

            var json = JsonSerializer.Serialize(export,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await _exportImportService.ImportFromJsonAsync(json, isPinned: true);
            return RedirectToAction("Noticias");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
