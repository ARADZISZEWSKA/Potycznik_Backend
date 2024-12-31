using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Potycznik_Backend.Data;
using Potycznik_Backend.Models;
using Potycznik_Backend.DTo;

namespace Potycznik_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint do pobierania wszystkich produktów
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return Ok(products);
        }

        // Endpoint do pobierania produktu po id
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // Endpoint do pobierania produktów po ID kategorii
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int categoryId)
        {
            var products = await _context.Products
                .Include(p => p.Category) 
                .Where(p => p.CategoryId == categoryId) 
                .ToListAsync();

            if (!products.Any())
            {
                return NotFound();
            }

            return Ok(products);
        }

        // Endpoint do tworzenia produktu
        [HttpPost("add-product")]
        public async Task<IActionResult> AddProductToTemporaryList([FromForm] ProductDTo productDto)
        {
            try
            {
                Console.WriteLine($"Product Name: {productDto.Name}");
                Console.WriteLine($"Category ID: {productDto.CategoryId}");
                Console.WriteLine($"Quantity: {productDto.Quantity}");
                Console.WriteLine($"Unit: {productDto.Unit}");

                // Znalezienie kategorii
                var category = await _context.Categories.FindAsync(productDto.CategoryId);
                if (category == null)
                {
                    return BadRequest("Invalid CategoryId.");
                }

                // Tworzymy nowy produkt
                var product = new Product
                {
                    Name = productDto.Name,
                    CategoryId = productDto.CategoryId,
                    Quantity = 0, 
                    Unit = productDto.Unit
                };

                // Dodajemy produkt do bazy danych
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Produkt został dodany do bazy." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { error = ex.Message, innerError = ex.InnerException?.Message });
            }
        }




        // Endpoint do aktualizacji produktu
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Endpoint do usuwania produktu
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Pomocnicza funkcja do sprawdzenia, czy produkt istnieje
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        [HttpPost("end-inventory")]
        public async Task<IActionResult> EndInventory([FromBody] List<InventoryRecordRequest> inventoryRecords)
        {
            if (inventoryRecords == null || !inventoryRecords.Any())
            {
                return BadRequest("Brak danych do zapisania.");
            }

            // Przetwarzamy rekordy inwentaryzacyjne
            foreach (var record in inventoryRecords)
            {
                var product = await _context.Products
                    .Where(p => p.Id == record.ProductId)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    return NotFound($"Produkt o ID {record.ProductId} nie istnieje.");
                }

                // Aktualizujemy ilość produktu w tabeli Products
                product.Quantity = record.Quantity;

                // Dodajemy rekord inwentaryzacyjny (tylko zapisujemy zmiany ilości)
                var inventoryRecord = new InventoryRecord
                {
                    ProductId = record.ProductId,
                    Date = DateTime.Now,
                    Quantity = record.Quantity, // Ilość z formularza
                };

                _context.InventoryRecords.Add(inventoryRecord);
            }

            // Zapisujemy zmiany w tabeli InventoryRecords
            await _context.SaveChangesAsync();

            return Ok("Inwentaryzacja zakończona.");
        }


    }


}
