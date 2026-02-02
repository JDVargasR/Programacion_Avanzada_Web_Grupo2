using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Transactions;

namespace APW.Web.Models
{
    public class ProductViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }

    }
}