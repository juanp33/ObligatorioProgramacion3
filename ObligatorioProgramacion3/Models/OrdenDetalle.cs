using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3.Models;

public partial class OrdenDetalle
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El ID de la orden es obligatorio.")]
    public int? OrdenId { get; set; }

    [Required(ErrorMessage = "El ID del plato es obligatorio.")]
    public int? PlatoId { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser un número positivo.")]
    public int Cantidad { get; set; }

    public virtual Ordene? Orden { get; set; }

    public virtual Plato? Plato { get; set; }
}
