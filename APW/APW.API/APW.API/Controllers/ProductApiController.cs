using APW.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace APW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductApiController : ControllerBase
    {
        private readonly ILogger<ProductApiController> _logger;

        public ProductApiController(ILogger<ProductApiController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetProducts")]
        public IEnumerable<Product> Get()
        {            
            return [.. Enumerable.Range(1, 100).Select(index => new Product
            {
                Id = index+1,
                Name = $"Product {index+1}",
                Price = Math.Round((decimal)(new Random().NextDouble() * 100), 2)
            })];
        }
    }
}
