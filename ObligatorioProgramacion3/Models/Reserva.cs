using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3.Models;

public partial class Reserva
{
    public int Id { get; set; }

   
    
    public int? ClienteId { get; set; }

    
    
    public int? MesaId { get; set; }

    [Required(ErrorMessage = "La fecha de la reserva es obligatoria.")]
    public DateTime FechaReserva { get; set; }

    [Required(ErrorMessage = "El estado es obligatorio.")]
    [StringLength(50, ErrorMessage = "El estado no puede exceder los 50 caracteres.")]
    public string Estado { get; set; } = null!;

    [Required(ErrorMessage = "El ID del restaurante es obligatorio.")]

    public int? IdRestaurante { get; set; }

    [Required(ErrorMessage = "El ID del usuario es obligatorio.")]

    public int? UsuarioId { get; set; }

    public virtual Cliente? Cliente { get; set; }

    public virtual Restaurante? IdRestauranteNavigation { get; set; }

    public virtual Mesa? Mesa { get; set; }

    public virtual ICollection<Ordene> Ordenes { get; set; } = new List<Ordene>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual Usuario? Usuario { get; set; }
}