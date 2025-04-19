using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Potycznik_Backend.Models
{
    public class Product
    {

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int CategoryId { get; set; } 
        [JsonIgnore]
        public Category? Category { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public string Unit { get; set; }
        public string? Barcode { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public string? Image { get; set; }
        public decimal MinimalQuantity { get; set; }

        public ICollection<InventoryRecord> InventoryRecords { get; set; } = new List<InventoryRecord>();
        public ICollection<Loss> Losses { get; set; } = new List<Loss>();
    }

}
