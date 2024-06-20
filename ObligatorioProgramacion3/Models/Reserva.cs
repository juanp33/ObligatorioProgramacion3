using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3.Models;

public partial class Reserva
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El ID del cliente es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del cliente debe ser un número positivo.")]
    public int ClienteID { get; set; }

    [Required(ErrorMessage = "El ID de la mesa es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de la mesa debe ser un número positivo.")]
    public int MesaID { get; set; }

    [Required(ErrorMessage = "La fecha de la reserva es obligatoria.")]
    public DateTime FechaReserva { get; set; }

    [Required(ErrorMessage = "El estado es obligatorio.")]
    [StringLength(50, ErrorMessage = "El estado no puede exceder los 50 caracteres.")]
    public string Estado { get; set; }

    [Required(ErrorMessage = "El ID del restaurante es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del restaurante debe ser un número positivo.")]
    public int IdRestaurante { get; set; }

    [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del usuario debe ser un número positivo.")]
    public int UsuarioID { get; set; }

    public virtual Cliente? Cliente { get; set; }

    public virtual Restaurante? IdRestauranteNavigation { get; set; }

    public virtual Mesa? Mesa { get; set; }

    public virtual ICollection<Ordene> Ordenes { get; set; } = new List<Ordene>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual Usuario? Usuario { get; set; }
}
