using System;
using System.Collections.Generic;

namespace ObligatorioProgramacion3.Models;

public partial class Cotizacion
{
    public int Id { get; set; }

    public double? Cotizacion1 { get; set; }

    public DateTime? FechaCotizacion { get; set; }

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
