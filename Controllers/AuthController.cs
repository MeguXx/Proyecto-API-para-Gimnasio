using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using GYMAPI.Models;
using BCrypt.Net;

namespace GYMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase 
    {
        private readonly GymDbContext _context;
        public AuthController(GymDbContext context) { _context = context; }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == login.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash)) 
                return Unauthorized(new { message = "Credenciales incorrectas" });

            var roleName = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "SOCIO";
            var key = Encoding.ASCII.GetBytes("clave-secreta-gym-api-2025-muy-larga-para-que-funcione");
            
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[] { 
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, roleName) 
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { token = tokenHandler.WriteToken(token), role = roleName });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest register)
        {
            if (await _context.Users.AnyAsync(u => u.UserName == register.Username))
                return BadRequest(new { message = "Usuario ya existe" });

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(register.Password);
            var user = new User { UserName = register.Username, PasswordHash = hashedPassword, Email = register.Email, IsActive = true };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == register.Role);
            if (role != null)
            {
                _context.UserRoles.Add(new UserRole { UserId = user.UserId, RoleId = role.RoleId });
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Usuario registrado", userId = user.UserId });
        }

        [Authorize]
        [HttpGet("perfil")]
        public async Task<IActionResult> GetPerfil()
        {
            var username = User.Identity?.Name;
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null) return NotFound();

            var role = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "SOCIO";
            return Ok(new { user.UserId, user.UserName, user.Email, role });
        }
    }

    public class LoginRequest { 
        public string? Username { get; set; } 
        public string? Password { get; set; }
    }

    public class RegisterRequest {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; } // ADMIN, ENTRENADOR, SOCIO
    }
}