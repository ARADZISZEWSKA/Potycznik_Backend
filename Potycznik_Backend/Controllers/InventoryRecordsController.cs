using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Potycznik_Backend.Data;
using Potycznik_Backend.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        // Grupowanie rekordów InventoryRecords po dacie
        [HttpPost("group-records-by-date")]
        public async Task<IActionResult> GroupRecordsByDate()
        {
            try
            {
                // Pobranie wszystkich rekordów InventoryRecords
                var ungroupedRecords = await _context.InventoryRecords
                    .Where(ir => !_context.Inventories
                        .SelectMany(i => i.InventoryRecords.Select(r => r.Id))
                        .Contains(ir.Id)) // Rekordy bez przypisanej grupy
                    .ToListAsync();

                if (!ungroupedRecords.Any())
                {
                    return Ok("Brak rekordów do grupowania.");
                }

                // Grupowanie rekordów po dacie (tylko dzień)
                var groupedByDate = ungroupedRecords
                    .GroupBy(ir => ir.Date.Date)
                    .Select(group => new
                    {
                        Date = group.Key,
                        Records = group.ToList()  // Powiązanie rekordów
                    })
                    .ToList();

                // Tworzenie nowych grup Inventory
                foreach (var group in groupedByDate)
                {
                    var newInventory = new Inventory
                    {
                        Date = group.Date,
                        InventoryRecords = group.Records // Powiązanie rekordów
                    };

                    _context.Inventories.Add(newInventory);
                }

                // Zapisanie zmian w bazie danych
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Rekordy zostały pogrupowane.",
                    groupedDates = groupedByDate.Select(g => g.Date).ToList() // Zwrot pogrupowanych dat
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Pobieranie szczegółów grupy Inventory na podstawie daty
        [HttpGet("inventory-by-date/{date}")]
        public async Task<IActionResult> GetInventoryWithRecordsByDate(DateTime date)
        {
            try
            {
                // Znalezienie wszystkich grup Inventory na podstawie daty
                var inventories = await _context.Inventories
                    .Include(i => i.InventoryRecords)
                    .Where(i => i.Date.Date == date.Date) // Filtracja po pełnej dacie
                    .ToListAsync();

                if (inventories == null || inventories.Count == 0)
                {
                    return NotFound(new { error = "Nie znaleziono grup Inventory dla podanej daty." });
                }

                return Ok(inventories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
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
                    .Select(i => i.Date.Date) // Pobieranie unikalnych dat
                    .Distinct()
                    .ToListAsync();

                return Ok(availableDates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
