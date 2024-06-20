using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3.Models;

public partial class Usuario
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El email es obligatorio.")]
    [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, ErrorMessage = "La contraseña no puede exceder los 100 caracteres.")]
    public string Contraseña { get; set; }

    [Required(ErrorMessage = "El ID del rol es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del rol debe ser un número positivo.")]
    public int RolId { get; set; }
    public virtual Cliente? Cliente { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual Role? Rol { get; set; }

    public virtual ICollection<UsuarioRole> UsuarioRoles { get; set; } = new List<UsuarioRole>();
}
