using System;
using System.Collections.Generic;

namespace GYMAPI.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string NormalizedUserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string NormalizedEmail { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Entrenador? Entrenador { get; set; }

    public virtual Socio? Socio { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
