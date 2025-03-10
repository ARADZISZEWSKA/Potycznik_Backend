using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using Potycznik_Backend.Data;
using Potycznik_Backend.DTo;
using Potycznik_Backend.Models;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthController(AppDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }


    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto dto)
    {
        try
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { error = "Email już istnieje" });

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { error = "Hasło nie może być puste" });

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Role = "Admin",
                PasswordHash = _passwordHasher.HashPassword(null, dto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Rejestracja admina udana", userId = user.Id });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd rejestracji admina: {ex.Message}");
            return StatusCode(500, new { error = "Wewnętrzny błąd serwera", details = ex.Message });
        }
    }


    /// Rejestracja użytkownika przez administratora
    [HttpPost("register-user")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterDto dto, [FromQuery] int adminId)
    {
        var admin = await _context.Users.FindAsync(adminId);
        if (admin == null || admin.Role != "Admin")
            return BadRequest("Nieprawidłowy admin");

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email już istnieje");

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Role = "User",
            AdminId = adminId,
            PasswordHash = _passwordHasher.HashPassword(null, dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok("Użytkownik dodany");
    }

    /// Logowanie użytkownika
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTo dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) != PasswordVerificationResult.Success)
            return Unauthorized("Błędny email lub hasło");

        return Ok(new
        {
            user.Id,
            user.Email,
            user.Role,
            user.AdminId
        });
    }
}
