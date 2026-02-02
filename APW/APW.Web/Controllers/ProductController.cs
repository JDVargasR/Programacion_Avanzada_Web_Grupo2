using APW.Architecture;
using APW.Web.Models;
using APW.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;

namespace APW.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IWrapperServiceProvider _serviceProvider;

        public ProductController(ILogger<ProductController> logger, IWrapperServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _serviceProvider.GetDataAsync<IEnumerable<ProductViewModel>>("https://localhost:7159/ProductApi");
            return View(data);
        }
    }
}
