namespace Potycznik_Backend.DTo
{
    public class LossDTo
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public int Quantity { get; set; } 
        public string Reason { get; set; }
        public DateTime Date { get; set; }
    }
}
