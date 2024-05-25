using System;
using System.Collections.Generic;

namespace ObligatorioProgramacion3.Models;

public partial class Plato
{
    public int Id { get; set; }

    public string NombrePlato { get; set; } = null!;

    public string? Descripción { get; set; }

    public decimal Precio { get; set; }

    public string Imagen { get; set; } = null!;

    public virtual ICollection<OrdenDetalle> OrdenDetalles { get; set; } = new List<OrdenDetalle>();
}
