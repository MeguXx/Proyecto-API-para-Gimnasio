using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GYMAPI.Models;

public partial class GymDbContext : DbContext
{
    public GymDbContext()
    {
    }

    public GymDbContext(DbContextOptions<GymDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Asistencia> Asistencias { get; set; }

    public virtual DbSet<Ejercicio> Ejercicios { get; set; }

    public virtual DbSet<Entrenador> Entrenadores { get; set; }

    public virtual DbSet<Membresia> Membresias { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Rutina> Rutinas { get; set; }

    public virtual DbSet<RutinaEjercicio> RutinaEjercicios { get; set; }

    public virtual DbSet<Socio> Socios { get; set; }

    public virtual DbSet<SocioEntrenador> SocioEntrenadors { get; set; }

    public virtual DbSet<SocioMembresia> SocioMembresias { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asistencia>(entity =>
        {
            entity.HasKey(e => e.AsistenciaId).HasName("PK__Asistenc__123456");

            entity.Property(e => e.FechaHoraEntrada).HasPrecision(0);
            entity.Property(e => e.FechaHoraSalida).HasPrecision(0);
            entity.Property(e => e.Observaciones).HasMaxLength(300);

            entity.HasOne(d => d.Socio).WithMany(p => p.Asistencias)
                .HasForeignKey(d => d.SocioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Asistencias_Socio");

            entity.HasOne(d => d.RegistradaPorUser).WithMany()
                .HasForeignKey(d => d.RegistradaPorUserId)
                .HasConstraintName("FK_Asistencias_UsuarioReg");
        });

        modelBuilder.Entity<Entrenador>(entity =>
        {
            entity.HasKey(e => e.EntrenadorId).HasName("PK__Entrenad__D0EE8565D6C99A52");

            entity.HasIndex(e => e.UserId, "UQ__Entrenad__1788CC4DFBA76F3F").IsUnique();

            entity.Property(e => e.Especialidad).HasMaxLength(120);
            entity.Property(e => e.Certificaciones).HasMaxLength(250);
            entity.Property(e => e.FechaIngreso)
                .HasColumnType("date")
                .HasDefaultValueSql("(CONVERT(date,sysdatetime()))");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.User).WithOne(p => p.Entrenador)
                .HasForeignKey<Entrenador>(d => d.UserId)
                .HasConstraintName("FK__Entrenado__UserI__45F365D3");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AA84E6BD8");

            entity.HasIndex(e => e.Name, "UQ__Roles__737584F6576F42A7").IsUnique();

            entity.HasIndex(e => e.NormalizedName, "UQ__Roles__A93C97B99CE43A2A").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.NormalizedName).HasMaxLength(50);
        });

        modelBuilder.Entity<Socio>(entity =>
        {
            entity.HasKey(e => e.SocioId).HasName("PK__Socios__165D08BA1126B50A");

            entity.HasIndex(e => e.UserId, "UQ__Socios__1788CC4D9DBB4C3A").IsUnique();

            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(CONVERT([date],sysdatetime()))");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.User).WithOne(p => p.Socio)
                .HasForeignKey<Socio>(d => d.UserId)
                .HasConstraintName("FK__Socios__UserId__4AB81AF0");
        });

        modelBuilder.Entity<SocioEntrenador>(entity =>
        {
            entity.HasKey(e => new { e.SocioId, e.EntrenadorId }).HasName("PK__SocioEnt__5B53E0ECF20B6073");

            entity.ToTable("SocioEntrenador");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaAsignacion).HasDefaultValueSql("(CONVERT([date],sysdatetime()))");

            entity.HasOne(d => d.Entrenador).WithMany(p => p.SocioEntrenadors)
                .HasForeignKey(d => d.EntrenadorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocioEntrenador_Entr");

            entity.HasOne(d => d.Socio).WithMany(p => p.SocioEntrenadors)
                .HasForeignKey(d => d.SocioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocioEntrenador_Socio");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CB793452A");

            entity.HasIndex(e => e.NormalizedEmail, "UQ__Users__368B291A23A17CCA").IsUnique();

            entity.HasIndex(e => e.NormalizedUserName, "UQ__Users__54E8BE222AC3D38A").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534A3C000E7").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ__Users__C9F284564B0AD990").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLoginAt).HasPrecision(0);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(512);
            entity.Property(e => e.PhoneNumber).HasMaxLength(25);
            entity.Property(e => e.UpdatedAt).HasPrecision(0);
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId }).HasName("PK__UserRole__123456");

            entity.Property(e => e.AssignedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_User");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Role");
        });

        modelBuilder.Entity<Ejercicio>(entity =>
        {
            entity.HasKey(e => e.EjercicioId).HasName("PK__Ejercicios__123456");

            entity.Property(e => e.Nombre).HasMaxLength(120);
            entity.Property(e => e.Descripcion).HasMaxLength(400);
            entity.Property(e => e.GrupoMuscular).HasMaxLength(60);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Membresia>(entity =>
        {
            entity.HasKey(e => e.MembresiaId).HasName("PK__Membresi__123456");

            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(300);
            entity.Property(e => e.Precio).HasPrecision(10, 2);
            entity.Property(e => e.EsRenovable).HasDefaultValue(true);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
        });

        modelBuilder.Entity<Rutina>(entity =>
        {
            entity.HasKey(e => e.RutinaId).HasName("PK__Rutinas__123456");

            entity.Property(e => e.Nombre).HasMaxLength(120);
            entity.Property(e => e.Objetivo).HasMaxLength(300);
            entity.Property(e => e.Activa).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Socio).WithMany(p => p.Rutinas)
                .HasForeignKey(d => d.SocioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Rutinas_Socio");

            entity.HasOne(d => d.Entrenador).WithMany(p => p.Rutinas)
                .HasForeignKey(d => d.EntrenadorId)
                .HasConstraintName("FK_Rutinas_Entrenador");
        });

        modelBuilder.Entity<RutinaEjercicio>(entity =>
        {
            entity.HasKey(e => new { e.RutinaId, e.EjercicioId }).HasName("PK__RutinaEj__123456");

            entity.ToTable("RutinaEjercicios");

            entity.Property(e => e.PesoObjetivoKg).HasPrecision(6, 2);
            entity.Property(e => e.Notas).HasMaxLength(250);

            entity.HasOne(d => d.Rutina).WithMany(p => p.RutinaEjercicios)
                .HasForeignKey(d => d.RutinaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RutinaEjercicios_Rutina");

            entity.HasOne(d => d.Ejercicio).WithMany(p => p.RutinaEjercicios)
                .HasForeignKey(d => d.EjercicioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RutinaEjercicios_Ejercicio");
        });

        modelBuilder.Entity<SocioMembresia>(entity =>
        {
            entity.HasKey(e => e.SocioMembresiaId).HasName("PK__SocioMem__123456");

            entity.Property(e => e.Estado).HasMaxLength(20);
            entity.Property(e => e.MontoPagado).HasPrecision(10, 2);
            entity.Property(e => e.Notas).HasMaxLength(300);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Socio).WithMany(p => p.SocioMembresias)
                .HasForeignKey(d => d.SocioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocioMembresia_Socio");

            entity.HasOne(d => d.Membresia).WithMany(p => p.SocioMembresias)
                .HasForeignKey(d => d.MembresiaId)
                .HasConstraintName("FK_SocioMembresia_Membresia");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
