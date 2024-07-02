using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3.Models;

public partial class Pago
{

    public int Id { get; set; }

    [Required(ErrorMessage = "El ID de la reserva es obligatorio.")]
    public int? ReservaId { get; set; }

    [Required(ErrorMessage = "El monto es obligatorio.")]
    [Range(0.01, 9999999999.99, ErrorMessage = "El monto debe ser un valor positivo.")]
    public decimal Monto { get; set; }

    [Required(ErrorMessage = "La fecha de pago es obligatoria.")]
    public DateTime FechaPago { get; set; }

    [Required(ErrorMessage = "El método de pago es obligatorio.")]
    [StringLength(50, ErrorMessage = "El método de pago no puede exceder los 50 caracteres.")]
    public string MetodoPago { get; set; } = null!;

    public int? IdClima { get; set; }

    public int? IdCotizacion { get; set; }

    public virtual Clima? IdClimaNavigation { get; set; }

    public virtual Cotizacion? IdCotizacionNavigation { get; set; }

    public virtual Reserva? Reserva { get; set; }
}
