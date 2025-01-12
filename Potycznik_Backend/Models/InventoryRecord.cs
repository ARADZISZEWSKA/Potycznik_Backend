using System.Text.Json.Serialization;

namespace Potycznik_Backend.Models
{
    public class InventoryRecord
    {
        public int Id { get; set; }
        public int? ProductId { get; set; } // Klucz obcy do produktu
        public Product Product { get; set; } // Nawigacja do produktu
        public DateTime Date { get; set; } 
        public decimal Quantity { get; set; }
        public decimal PreviousQuantity { get; set; }
        public string ProductName { get; set; }

    }

}
