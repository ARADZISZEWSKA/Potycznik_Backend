namespace Potycznik_Backend.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public int CategoryId { get; set; } // Klucz obcy do kategorii
        public Category Category { get; set; } // Nawigacja do kategorii
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public string? Barcode { get; set; }
        public DateTime? ExpiryDate { get; set; } 
    }

}
