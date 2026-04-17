using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GYMAPI.Models;

namespace GYMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembresiasController : ControllerBase
    {
        private readonly GymDbContext _context;
        public MembresiasController(GymDbContext context) { _context = context; }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.Membresias.ToListAsync());

        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var membresia = await _context.Membresias.FindAsync(id);
            return membresia == null ? NotFound() : Ok(membresia);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MembresiaCreateRequest request)
        {
            var membresia = new Membresia
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                DuracionDias = request.DuracionDias,
                Precio = request.Precio,
                EsRenovable = request.EsRenovable,
                IsActive = true
            };
            _context.Membresias.Add(membresia);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Membresía creada", membresiaId = membresia.MembresiaId });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MembresiaUpdateRequest request)
        {
            var membresia = await _context.Membresias.FindAsync(id);
            if (membresia == null) return NotFound();

            membresia.Nombre = request.Nombre ?? membresia.Nombre;
            membresia.Descripcion = request.Descripcion ?? membresia.Descripcion;
            membresia.DuracionDias = request.DuracionDias ?? membresia.DuracionDias;
            membresia.Precio = request.Precio ?? membresia.Precio;
            membresia.EsRenovable = request.EsRenovable ?? membresia.EsRenovable;
            membresia.IsActive = request.IsActive ?? membresia.IsActive;

            _context.Membresias.Update(membresia);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Membresía actualizada" });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var membresia = await _context.Membresias.FindAsync(id);
            if (membresia == null) return NotFound();

            _context.Membresias.Remove(membresia);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Membresía eliminada" });
        }
    }

    public class MembresiaCreateRequest
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public int DuracionDias { get; set; }
        public decimal Precio { get; set; }
        public bool EsRenovable { get; set; }
    }

    public class MembresiaUpdateRequest
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int? DuracionDias { get; set; }
        public decimal? Precio { get; set; }
        public bool? EsRenovable { get; set; }
        public bool? IsActive { get; set; }
    }
}