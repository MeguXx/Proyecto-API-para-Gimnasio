using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GYMAPI.Models;
using BCrypt.Net;

namespace GYMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntrenadoresController : ControllerBase
    {
        private readonly GymDbContext _context;
        public EntrenadoresController(GymDbContext context) { _context = context; }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.Entrenadores.Include(e => e.User).ToListAsync());

        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entrenador = await _context.Entrenadores.Include(e => e.User).FirstOrDefaultAsync(e => e.EntrenadorId == id);
            return entrenador == null ? NotFound() : Ok(entrenador);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EntrenadorCreateRequest request)
        {
            var user = new User
            {
                UserName = request.UserName,
                NormalizedUserName = request.UserName.ToUpper(),
                Email = request.Email,
                NormalizedEmail = request.Email.ToUpper(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsActive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var entrenador = new Entrenador
            {
                UserId = user.UserId,
                Especialidad = request.Especialidad,
                Certificaciones = request.Certificaciones
            };
            _context.Entrenadores.Add(entrenador);
            await _context.SaveChangesAsync();

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "ENTRENADOR");
            if (role != null)
            {
                _context.UserRoles.Add(new UserRole { UserId = user.UserId, RoleId = role.RoleId });
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Entrenador creado", entrenadorId = entrenador.EntrenadorId });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EntrenadorUpdateRequest request)
        {
            var entrenador = await _context.Entrenadores.FindAsync(id);
            if (entrenador == null) return NotFound();

            entrenador.Especialidad = request.Especialidad ?? entrenador.Especialidad;
            entrenador.Certificaciones = request.Certificaciones ?? entrenador.Certificaciones;
            entrenador.IsActive = request.IsActive ?? entrenador.IsActive;

            _context.Entrenadores.Update(entrenador);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Entrenador actualizado" });
        }
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        [HttpGet("{id}/socios")]
        public async Task<IActionResult> GetSociosAsignados(int id)
        {
            var entrenador = await _context.Entrenadores.FindAsync(id);
            if (entrenador == null) return NotFound();

            var socios = await _context.SocioEntrenadors
                .Where(se => se.EntrenadorId == id && se.Activo)
                .Include(se => se.Socio)
                .ThenInclude(s => s.User)
                .Select(se => new { se.Socio.SocioId, se.Socio.User.UserName, se.Socio.User.Email })
                .ToListAsync();

            return Ok(socios);
        }
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entrenador = await _context.Entrenadores.FindAsync(id);
            if (entrenador == null) return NotFound();

            _context.Entrenadores.Remove(entrenador);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Entrenador eliminado" });
        }
    }

    public class EntrenadorCreateRequest
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Especialidad { get; set; }
        public string? Certificaciones { get; set; }
    }

    public class EntrenadorUpdateRequest
    {
        public string? Especialidad { get; set; }
        public string? Certificaciones { get; set; }
        public bool? IsActive { get; set; }
    }
}