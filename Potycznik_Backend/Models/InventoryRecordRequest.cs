namespace Potycznik_Backend.Models
{
    public class InventoryRecordRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PreviousQuantity { get; set; }
    }
}
