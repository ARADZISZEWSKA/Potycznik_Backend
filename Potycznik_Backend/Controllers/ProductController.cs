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

            product.Image = product.Image != null
        ? $"/images/{Path.GetFileName(product.Image)}"
        : "/assets/placeholder.jpg";

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

                // Obsługa obrazu
                string relativeImagePath = null;
                if (productDto.Image != null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + productDto.Image.FileName;
                    var absoluteImagePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Kopiowanie pliku na serwer
                    using (var fileStream = new FileStream(absoluteImagePath, FileMode.Create))
                    {
                        await productDto.Image.CopyToAsync(fileStream);
                    }

                    // Ustal relatywną ścieżkę do obrazu
                    relativeImagePath = Path.Combine("images", uniqueFileName).Replace("\\", "/");
                }

                // Tworzymy nowy produkt
                var product = new Product
                {
                    Name = productDto.Name,
                    CategoryId = productDto.CategoryId,
                    Quantity = productDto.Quantity,
                    Unit = productDto.Unit,
                    Image = relativeImagePath,
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
        [HttpPost("update-inventory")]
        public async Task<IActionResult> UpdateInventory([FromBody] UpdateInventoryRequestDTo request)
        {
            if (request.InventoryRecords == null || !request.InventoryRecords.Any())
            {
                return BadRequest("Brak rekordów do aktualizacji.");
            }

            foreach (var record in request.InventoryRecords)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == record.ProductId);

                if (product == null)
                {
                    return NotFound($"Produkt o ID {record.ProductId} nie istnieje.");
                }

                // Aktualizacja ilości w produkcie
                product.Quantity = record.Quantity;

                // Dodanie rekordu inwentaryzacyjnego
                var inventoryRecord = new InventoryRecord
                {
                    ProductId = record.ProductId,
                    Date = DateTime.Now,
                    Quantity = record.Quantity
                };

                _context.InventoryRecords.Add(inventoryRecord);
            }

            await _context.SaveChangesAsync();
            return Ok("Zaktualizowano inwentaryzację.");
        }


        // Endpoint do usuwania produktu
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteProducts([FromBody] DeleteProductRequestDTo request)
        {
            if (request.ProductsToDelete == null || !request.ProductsToDelete.Any())
            {
                return BadRequest("Brak produktów do usunięcia.");
            }

            var productsToDelete = await _context.Products
                .Where(p => request.ProductsToDelete.Contains(p.Id))
                .ToListAsync();

            if (!productsToDelete.Any())
            {
                return NotFound("Żadne produkty nie zostały znalezione do usunięcia.");
            }

            foreach (var product in productsToDelete)
            {
                // Dodanie rekordu inwentaryzacyjnego przed usunięciem
                var inventoryRecord = new InventoryRecord
                {
                    ProductId = product.Id,
                    Date = DateTime.Now,
                    Quantity = 0 // Ilość 0, bo produkt jest usunięty
                };
                _context.InventoryRecords.Add(inventoryRecord);
            }

            _context.Products.RemoveRange(productsToDelete);
            await _context.SaveChangesAsync();
            return Ok("Produkty zostały usunięte.");
        }

        // Pomocnicza funkcja do sprawdzenia, czy produkt istnieje
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        [HttpPost("end-inventory")]
        public async Task<IActionResult> EndInventory([FromBody] EndInventoryRequestDto request)
        {
            if (request == null || (request.InventoryRecords == null && request.ProductsToDelete == null))
            {
                return BadRequest("Invalid data.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Aktualizacja produktów
                if (request.InventoryRecords != null && request.InventoryRecords.Any())
                {
                    var productIds = request.InventoryRecords.Select(r => r.ProductId).ToHashSet();
                    var products = await _context.Products
                        .Where(p => productIds.Contains(p.Id))
                        .ToListAsync();

                    foreach (var record in request.InventoryRecords)
                    {
                        var product = products.FirstOrDefault(p => p.Id == record.ProductId);
                        if (product == null)
                        {
                            return BadRequest($"Product with ID {record.ProductId} not found.");
                        }

                        // Zaktualizuj ilość w produkcie
                        product.Quantity = record.Quantity;

                        // Dodaj rekord do InventoryRecords
                        var inventoryRecord = new InventoryRecord
                        {
                            ProductId = product.Id,
                            Quantity = record.Quantity,
                            PreviousQuantity = record.PreviousQuantity,
                            Date = DateTime.UtcNow,
                            ProductName = product.Name,
                            CategoryId = product.CategoryId
                        };
                        _context.InventoryRecords.Add(inventoryRecord);
                    }

                    // Zapisz zmiany w produktach
                    await _context.SaveChangesAsync();
                }

                // Usuwanie produktów
                if (request.ProductsToDelete != null && request.ProductsToDelete.Any())
                {
                    var productsToDelete = await _context.Products
                        .Where(p => request.ProductsToDelete.Contains(p.Id))
                        .ToListAsync();

                    foreach (var product in productsToDelete)
                    {
                        // Dodaj rekord do InventoryRecords z ilością ujemną
                        var inventoryRecord = new InventoryRecord
                        {
                            ProductId = product.Id,
                            Quantity = 0,
                            PreviousQuantity = product.Quantity,
                            Date = DateTime.UtcNow,
                            ProductName = product.Name,
                        };
                        _context.InventoryRecords.Add(inventoryRecord);
                    }

                    // Usuń produkty
                    _context.Products.RemoveRange(productsToDelete);

                    // Zapisz zmiany w usuniętych produktach
                    await _context.SaveChangesAsync();
                }

                // Zatwierdź transakcję
                await transaction.CommitAsync();

                return Ok(new { message = "Inwentaryzacja zakończona pomyślnie" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }



    }

}
