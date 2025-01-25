namespace Potycznik_Backend.Models
{
    public class Loss
    {
        public int Id { get; set; } 
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public DateTime Date { get; set; } 
        public int Quantity { get; set; } 
        public string Reason { get; set; } 
    }

}
