using System;
using System.Collections.Generic;

namespace GYMAPI.Models;

public partial class Socio
{
    public int SocioId { get; set; }

    public int UserId { get; set; }

    public DateOnly FechaRegistro { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    public string? Genero { get; set; }

    public decimal? AlturaCm { get; set; }

    public decimal? PesoKg { get; set; }

    public string? EmergenciaNombre { get; set; }

    public string? EmergenciaTelefono { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<SocioEntrenador> SocioEntrenadors { get; set; } = new List<SocioEntrenador>();

    public virtual ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();

    public virtual ICollection<Rutina> Rutinas { get; set; } = new List<Rutina>();

    public virtual ICollection<SocioMembresia> SocioMembresias { get; set; } = new List<SocioMembresia>();

    public virtual User User { get; set; } = null!;
}
