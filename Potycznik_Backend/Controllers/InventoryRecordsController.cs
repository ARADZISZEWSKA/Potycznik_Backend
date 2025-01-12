/*using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Potycznik_Backend.Data;
using Potycznik_Backend.Models;

namespace Potycznik_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryRecordsController : Controller
    {
        private readonly AppDbContext _context;

        public InventoryRecordsController(AppDbContext context)
        {
            _context = context;
        }

        // Grupowanie rekordów InventoryRecords po dacie i tworzenie grup Inventory
       // [HttpPost("group-records")]
       *//* public async Task<IActionResult> GroupRecordsByDate()
        {
            try
            {
                // Pobierz wszystkie rekordy bez przypisanej inwentaryzacji
                var ungroupedRecords = await _context.InventoryRecords
                    .Where(ir => ir.InventoryId == null)
                    .ToListAsync();

                if (!ungroupedRecords.Any())
                {
                    return Ok("Brak rekordów do grupowania.");
                }

                // Grupowanie rekordów po dacie (tylko dzień)
                var groupedByDate = ungroupedRecords
                    .GroupBy(ir => ir.Date.Date)
                    .ToList();

                // Lista nowych grup Inventory
                var inventoriesToAdd = groupedByDate.Select(group => new Inventory
                {
                    Date = group.Key,
                    InventoryRecords = group.ToList() // Przypisanie rekordów do grupy
                }).ToList();

                // Dodanie nowych grup Inventory do kontekstu
                _context.Inventories.AddRange(inventoriesToAdd);

                // Zapisanie zmian w bazie danych
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Rekordy zostały pogrupowane.",
                    groupedDates = groupedByDate.Select(g => g.Key).ToList() // Zwrot pogrupowanych dat
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }*//*

        // Pobieranie szczegółów grupy Inventory na podstawie ID
        [HttpGet("inventory/{id}")]
        public async Task<IActionResult> GetInventoryWithRecords(int id)
        {
            var inventory = await _context.Inventories
                .Include(i => i.InventoryRecords) // Załaduj powiązane rekordy
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
            {
                return NotFound(new { error = "Nie znaleziono grupy Inventory." });
            }

            return Ok(inventory);
        }

        // Pobieranie rekordów InventoryRecords na podstawie daty
        [HttpGet("records-by-date/{date}")]
        public async Task<IActionResult> GetRecordsByDate(DateTime date)
        {
            var inventory = await _context.Inventories
                .Include(i => i.InventoryRecords) // Załaduj powiązane rekordy
                .FirstOrDefaultAsync(i => i.Date.Date == date.Date);

            if (inventory == null)
            {
                return NotFound(new { error = "Nie znaleziono rekordów dla podanej daty." });
            }

            return Ok(inventory.InventoryRecords); // Zwróć rekordy z grupy
        }

        // Pobieranie wszystkich grup Inventory
        [HttpGet("all-inventories")]
        public async Task<IActionResult> GetAllInventories()
        {
            var inventories = await _context.Inventories
                .Include(i => i.InventoryRecords) // Załaduj powiązane rekordy
                .ToListAsync();

            return Ok(inventories);
        }

        // Backend - metoda do pobierania dostępnych dat
        [HttpGet("available-dates")]
        public async Task<IActionResult> GetAvailableDates()
        {
            try
            {
                var availableDates = await _context.Inventories
                    .Select(i => i.Date.Date)  // Pobieramy tylko datę (bez godziny)
                    .Distinct()                 // Unikalne daty
                    .ToListAsync();             // Tu jest ważne, żeby to była lista

                return Ok(availableDates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


    }
}
*/