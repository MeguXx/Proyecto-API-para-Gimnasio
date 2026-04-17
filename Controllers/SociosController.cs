using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GYMAPI.Models;
using BCrypt.Net;

namespace GYMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SociosController : ControllerBase
    {
        private readonly GymDbContext _context;
        public SociosController(GymDbContext context) { _context = context; }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.Socios.ToListAsync());

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SocioCreateRequest request)
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

            var socio = new Socio
            {
                UserId = user.UserId,
                FechaNacimiento = request.FechaNacimiento,
                Genero = request.Genero,
                AlturaCm = request.AlturaCm,
                PesoKg = request.PesoKg,
                EmergenciaNombre = request.EmergenciaNombre,
                EmergenciaTelefono = request.EmergenciaTelefono
            };
            _context.Socios.Add(socio);
            await _context.SaveChangesAsync();

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "SOCIO");
            if (role != null)
            {
                _context.UserRoles.Add(new UserRole { UserId = user.UserId, RoleId = role.RoleId });
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Socio creado", socioId = socio.SocioId });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("{socioId}/membresia")]
        public async Task<IActionResult> AsignarMembresia(int socioId, [FromBody] AsignarMembresiaRequest request)
        {
            var socio = await _context.Socios.FindAsync(socioId);
            if (socio == null) return NotFound("Socio no encontrado");

            var membresia = await _context.Membresias.FindAsync(request.MembresiaId);
            if (membresia == null) return NotFound("Membresía no encontrada");

            var socioMembresia = new SocioMembresia
            {
                SocioId = socioId,
                MembresiaId = request.MembresiaId,
                FechaInicio = DateOnly.FromDateTime(DateTime.Now),
                FechaFin = DateOnly.FromDateTime(DateTime.Now.AddDays(membresia.DuracionDias)),
                Estado = "ACTIVA",
                MontoPagado = membresia.Precio,
                Notas = request.Notas
            };
            _context.SocioMembresias.Add(socioMembresia);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Membresía asignada" });
        }

        [Authorize(Roles = "SOCIO")]
        [HttpGet("{socioId}/rutinas")]
        public async Task<IActionResult> GetRutinas(int socioId)
        {
            var rutinas = await _context.Rutinas
                .Where(r => r.SocioId == socioId)
                .Include(r => r.RutinaEjercicios)
                .ThenInclude(re => re.Ejercicio)
                .ToListAsync();
            return Ok(rutinas);
        }

    public class SocioCreateRequest
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateOnly? FechaNacimiento { get; set; }
        public string? Genero { get; set; }
        public decimal? AlturaCm { get; set; }
        public decimal? PesoKg { get; set; }
        public string? EmergenciaNombre { get; set; }
        public string? EmergenciaTelefono { get; set; }
    }

    public class AsignarMembresiaRequest
    {
        public int MembresiaId { get; set; }
        public string? Notas { get; set; }
    }
}
}