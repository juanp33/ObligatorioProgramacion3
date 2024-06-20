using System;
using System.Collections.Generic;

namespace ObligatorioProgramacion3.Models;

public partial class OrdenDetalle
{
    public int Id { get; set; }

    public int? OrdenId { get; set; }

    public int? PlatoId { get; set; }

    public int Cantidad { get; set; }

    public virtual Ordene? Orden { get; set; }

    public virtual Plato? Plato { get; set; }
}
