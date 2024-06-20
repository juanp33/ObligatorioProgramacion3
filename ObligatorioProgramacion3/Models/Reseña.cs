using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3.Models;

public partial class Reseña
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El ID del cliente es obligatorio.")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "El ID del restaurante es obligatorio.")]
    public int RestauranteId { get; set; }

    [Required(ErrorMessage = "El puntaje es obligatorio.")]
    [Range(1, 5, ErrorMessage = "El puntaje debe ser un número entre 1 y 5.")]
    public int Puntaje { get; set; }

    [StringLength(500, ErrorMessage = "El comentario no puede exceder los 500 caracteres.")]
    public string Comentario { get; set; }

    [Required(ErrorMessage = "La fecha de la reseña es obligatoria.")]
    public DateTime FechaReseña { get; set; }

    public virtual Cliente? Cliente { get; set; }

    public virtual Restaurante? Restaurante { get; set; }
}
