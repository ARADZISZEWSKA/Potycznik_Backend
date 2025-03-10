namespace Potycznik_Backend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } 
        public int? AdminId { get; set; } 
        public virtual User? Admin { get; set; }
    }

}
