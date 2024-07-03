using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3.Models;

public partial class Plato
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del plato es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre del plato no puede exceder los 100 caracteres.")]
    public string NombrePlato { get; set; } = null!;

    [StringLength(300, ErrorMessage = "La descripción no puede exceder los 300 caracteres.")]
    public string? Descripción { get; set; }

    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Range(0.01, 9999999999.99, ErrorMessage = "El precio debe ser un valor positivo.")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "La imagen es obligatoria.")]
    [StringLength(300, ErrorMessage = "La URL de la imagen no puede exceder los 300 caracteres.")]
    [Url(ErrorMessage = "La URL de la imagen no es válida.")]
    public string Imagen { get; set; } = null!;

    public int? IdRestaurante { get; set; }

    public virtual Restaurante? IdRestauranteNavigation { get; set; }

    public virtual ICollection<OrdenDetalle> OrdenDetalles { get; set; } = new List<OrdenDetalle>();
}
