using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Potycznik_Backend.DTo
{
    public class ProductDTo 
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int CategoryId { get; set; } 

        [Required]
        public decimal Quantity { get; set; }
        public decimal MinimalQuantity { get; set; }

        [Required]
        public string Unit { get; set; }

        public string? Barcode { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public IFormFile? Image { get; set; } 
    }
}