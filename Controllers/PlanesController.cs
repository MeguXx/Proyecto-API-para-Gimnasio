using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GYMAPI.Models;

namespace GYMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanesController : ControllerBase
    {
        private readonly GymDbContext _context;
        public PlanesController(GymDbContext context) { _context = context; }

        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.Rutinas.ToListAsync());

        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Rutina rutina)
        {
            _context.Rutinas.Add(rutina);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = rutina.RutinaId }, rutina);
        }

        [Authorize(Roles = "SOCIO")]
        [HttpGet("mi-plan")]
        public async Task<IActionResult> GetMiPlan()
        {
            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null) return NotFound();

            var socio = await _context.Socios.FirstOrDefaultAsync(s => s.UserId == user.UserId);
            if (socio == null) return NotFound();

            var rutinas = await _context.Rutinas
                .Where(r => r.EntrenadorId == (from se in _context.SocioEntrenadors where se.SocioId == socio.SocioId select se.EntrenadorId).FirstOrDefault())
                .Include(r => r.RutinaEjercicios)
                .ThenInclude(re => re.Ejercicio)
                .ToListAsync();

            return Ok(rutinas);
        }

        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rutina = await _context.Rutinas
                .Include(r => r.RutinaEjercicios)
                .ThenInclude(re => re.Ejercicio)
                .FirstOrDefaultAsync(r => r.RutinaId == id);
            if (rutina == null) return NotFound();
            return Ok(rutina);
        }

        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RutinaUpdateRequest request)
        {
            var rutina = await _context.Rutinas.FindAsync(id);
            if (rutina == null) return NotFound();

            rutina.Nombre = request.Nombre ?? rutina.Nombre;
            rutina.Objetivo = request.Objetivo ?? rutina.Objetivo;
            rutina.Activa = request.Activa ?? rutina.Activa;

            _context.Rutinas.Update(rutina);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Plan actualizado" });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var rutina = await _context.Rutinas.FindAsync(id);
            if (rutina == null) return NotFound();

            _context.Rutinas.Remove(rutina);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Plan eliminado" });
        }
    }

    public class RutinaUpdateRequest
    {
        public string? Nombre { get; set; }
        public string? Objetivo { get; set; }
        public bool? Activa { get; set; }
    }
}