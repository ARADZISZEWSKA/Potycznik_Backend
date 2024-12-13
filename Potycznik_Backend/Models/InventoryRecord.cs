namespace Potycznik_Backend.Models
{
    public class InventoryRecord
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }
        public int ProductId { get; set; } // Klucz obcy do produktu
        public Product Product { get; set; } // Nawigacja do produktu
        public DateTime Date { get; set; } 
        public int Quantity { get; set; }

        

    }

}
