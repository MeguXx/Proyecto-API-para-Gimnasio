using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GYMAPI.Models;

namespace GYMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsistenciasController : ControllerBase
    {
        private readonly GymDbContext _context;
        public AsistenciasController(GymDbContext context) { _context = context; }

        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] Asistencia asistencia)
        {
            asistencia.FechaHoraEntrada = DateTime.Now;
            _context.Asistencias.Add(asistencia);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Asistencia grabada" });
        }

        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        [HttpGet("socio/{id}")]
        public async Task<IActionResult> GetHistorialSocio(int id) => 
            Ok(await _context.Asistencias.Where(a => a.SocioId == id).ToListAsync());

        [Authorize(Roles = "SOCIO")]
        [HttpGet("mi-historial")]
        public async Task<IActionResult> GetMiHistorial()
        {
            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null) return NotFound();

            var socio = await _context.Socios.FirstOrDefaultAsync(s => s.UserId == user.UserId);
            if (socio == null) return NotFound();

            return Ok(await _context.Asistencias.Where(a => a.SocioId == socio.SocioId).ToListAsync());
        }

        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AsistenciaUpdateRequest request)
        {
            var asistencia = await _context.Asistencias.FindAsync(id);
            if (asistencia == null) return NotFound("Asistencia no encontrada");

            asistencia.FechaHoraSalida = request.FechaHoraSalida ?? asistencia.FechaHoraSalida;
            asistencia.Observaciones = request.Observaciones ?? asistencia.Observaciones;

            _context.Asistencias.Update(asistencia);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Asistencia actualizada" });
        }

        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        [HttpGet("reporte")]
        public async Task<IActionResult> GetReporte() => 
            Ok(await _context.Asistencias.Include(a => a.Socio).ThenInclude(s => s.User).ToListAsync());
    }

    public class AsistenciaUpdateRequest
    {
        public DateTime? FechaHoraSalida { get; set; }
        public string? Observaciones { get; set; }
    }
}