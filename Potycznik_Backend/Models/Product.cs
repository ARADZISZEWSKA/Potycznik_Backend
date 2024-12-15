using System.Text.Json.Serialization;

namespace Potycznik_Backend.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public int CategoryId { get; set; } // Klucz obcy do kategorii
        [JsonIgnore]
        public Category Category { get; set; } // Nawigacja do kategorii
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public string? Barcode { get; set; }
        public DateTime? ExpiryDate { get; set; }


        public ICollection<InventoryRecord> InventoryRecords { get; set; }
        public ICollection<Loss> Losses { get; set; }
    }

}
