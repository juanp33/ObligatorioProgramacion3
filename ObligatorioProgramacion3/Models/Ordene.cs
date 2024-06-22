using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3.Models;

public partial class Ordene
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El ID de la reserva es obligatorio.")]
    public int? ReservaId { get; set; }

    [Required(ErrorMessage = "El total es obligatorio.")]
    [Range(0.01, 9999999999.99, ErrorMessage = "El total debe ser un valor positivo.")]
    public decimal Total { get; set; }
    public virtual ICollection<OrdenDetalle> OrdenDetalles { get; set; } = new List<OrdenDetalle>();

    public virtual Reserva? Reserva { get; set; }
}