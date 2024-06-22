using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3.Models;

public partial class Mesa
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El número de mesa es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El número de mesa debe ser un número positivo.")]
    public int NumeroMesa { get; set; }

    [Required(ErrorMessage = "La capacidad es obligatoria.")]
    [Range(1, 15, ErrorMessage = "La capacidad debe ser un número positivo y no puede exceder las 15 personas.")]
    public int Capacidad { get; set; }

    [Required(ErrorMessage = "El estado es obligatorio.")]
    [StringLength(50, ErrorMessage = "El estado no puede exceder los 50 caracteres.")]
    public string Estado { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}