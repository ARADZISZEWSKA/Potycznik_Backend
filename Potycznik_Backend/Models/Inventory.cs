namespace Potycznik_Backend.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public ICollection<InventoryRecord> InventoryRecords { get; set; } 
    }
}
