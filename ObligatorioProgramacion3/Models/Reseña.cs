using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3.Models;

public partial class Reseña
{
    public int Id { get; set; }

    public int? ClienteId { get; set; }

    public int? RestauranteId { get; set; }
    [Range(1, 5, ErrorMessage = "El Puntaje tiene que ser entre 1 y 5")]
    public int Puntaje { get; set; }

    public string? Comentario { get; set; }

    public DateTime FechaReseña { get; set; }

    public virtual Cliente? Cliente { get; set; }

    public virtual Restaurante? Restaurante { get; set; }
}
