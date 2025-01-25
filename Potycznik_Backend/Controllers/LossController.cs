using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Potycznik_Backend.Data;
using Potycznik_Backend.DTo;
using Potycznik_Backend.Models;
using System;
using System.Linq;

namespace Potycznik_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LossesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LossesController(AppDbContext context)
        {
            _context = context;
        }

        // Pobieranie wszystkich strat
        [HttpGet]
        public IActionResult GetLosses()
        {
            var losses = _context.Losses.ToList();
            return Ok(losses);
        }

        // Pobieranie straty po LossId
        [HttpGet("{id}")]
        public IActionResult GetLoss(int id)
        {
            var loss = _context.Losses.FirstOrDefault(l => l.Id == id);

            if (loss == null)
            {
                return NotFound();
            }

            return Ok(loss);
        }

        // Dodawanie nowej straty
        [HttpPost]
        public async Task<IActionResult> CreateLoss([FromBody] LossDTo lossDto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Name == lossDto.ProductName);

            if (product == null)
            {
                return BadRequest($"Product with name '{lossDto.ProductName}' does not exist.");
            }

            if (product.Quantity < lossDto.Quantity)
            {
                return BadRequest($"Insufficient quantity. Available quantity: {product.Quantity}, requested loss quantity: {lossDto.Quantity}");
            }

            int categoryId = lossDto.CategoryId > 0 ? lossDto.CategoryId : (product.Category?.Id ?? 0);

            // Stwórz nową stratę na podstawie danych z DTO i znalezionego produktu
            var loss = new Loss
            {
                ProductId = product.Id,
                ProductName = product.Name,
                CategoryId = categoryId,  // Jeśli categoryId jest null, ustawiamy domyślną wartość 0
                Quantity = lossDto.Quantity,
                Reason = lossDto.Reason,
                Date = DateTime.UtcNow 
            };

            product.Quantity -= lossDto.Quantity;

            _context.Losses.Add(loss);
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLoss), new { id = loss.Id }, loss);
        }





        // Edytowanie straty
        [HttpPut("{id}")]
        public IActionResult EditLoss(int id, [FromBody] Loss updatedLoss)
        {
            var existingLoss = _context.Losses.FirstOrDefault(l => l.Id == id);

            if (existingLoss == null)
            {
                return NotFound();
            }

            existingLoss.ProductId = updatedLoss.ProductId;
            existingLoss.Quantity = updatedLoss.Quantity;
            existingLoss.Reason = updatedLoss.Reason;
            existingLoss.Date = updatedLoss.Date;

            _context.Losses.Update(existingLoss);
            _context.SaveChanges();

            return NoContent();
        }

        // Usuwanie straty
        [HttpDelete("{id}")]
        public IActionResult DeleteLoss(int id)
        {
            var loss = _context.Losses.FirstOrDefault(l => l.Id == id);

            if (loss == null)
            {
                return NotFound();
            }

            _context.Losses.Remove(loss);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
