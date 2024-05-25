﻿using System;
using System.Collections.Generic;

namespace ObligatorioProgramacion3.Models;

public partial class Restaurante
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Dirección { get; set; } = null!;

    public string Teléfono { get; set; } = null!;

    public virtual ICollection<Reseña> Reseñas { get; set; } = new List<Reseña>();
}
