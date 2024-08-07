﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3.Models;

public partial class Restaurante
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres.")]
    public string Dirección { get; set; } = null!;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "El teléfono solo puede contener números.")]
    public string Teléfono { get; set; } = null!;

    [StringLength(200, ErrorMessage = "La descripción no puede exceder los 200 caracteres.")]
    public string? Descripcion { get; set; }

    public string? Imagen { get; set; }

    public string? Moneda { get; set; }

    public virtual ICollection<Mesa> Mesas { get; set; } = new List<Mesa>();

    public virtual ICollection<Plato> Platos { get; set; } = new List<Plato>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual ICollection<Reseña> Reseñas { get; set; } = new List<Reseña>();
}
