namespace Potycznik_Backend.DTo
{
    public class ReportItemDto
    {
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public decimal PreviousQuantity { get; set; }
        public decimal CurrentQuantity { get; set; }
        public decimal Change { get; set; }
        public decimal Loss { get; set; }
        public decimal MinimalQuantity { get; set; }
        public decimal ToBuy { get; set; }
        public string Status { get; set; }
    }
}
