using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3.Models;

public partial class Cliente
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
    public string Nombre { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres.")]
    [EmailAddress(ErrorMessage = "Formato de email no válido.")]
    public string Email { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "El tipo de cliente no puede exceder los 50 caracteres.")]
    public string TipoCliente { get; set; }

    public int IdUsuarios { get; set; }

    public Usuario Usuario { get; set; }
    public virtual Usuario IdUsuariosNavigation { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual ICollection<Reseña> Reseñas { get; set; } = new List<Reseña>();
}
