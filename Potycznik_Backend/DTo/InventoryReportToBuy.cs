namespace Potycznik_Backend.DTo
{
    public class InventoryReportToBuy
    {
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public decimal PreviousQuantity { get; set; }
        public decimal CurrentQuantity { get; set; }
        public decimal Change => CurrentQuantity - PreviousQuantity;
        public decimal MinimalQuantity { get; set; }
        public decimal? ToBuy => CurrentQuantity < MinimalQuantity ? MinimalQuantity - CurrentQuantity : (decimal?)null;
        public string Status => ToBuy.HasValue ? "Do zakupu" : "";
    }
}
