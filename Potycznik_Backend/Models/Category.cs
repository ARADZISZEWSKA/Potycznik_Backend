using System.Text.Json.Serialization;

namespace Potycznik_Backend.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; } // ID nadrzędnej kategorii 
        public Category ParentCategory { get; set; } // Nawigacja do nadrzędnej kategorii
        public List<Product> Products { get; set; }

    }
}
