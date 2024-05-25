using System;
using System.Collections.Generic;

namespace ObligatorioProgramacion3.Models;

public partial class Clima
{
    public int Id { get; set; }

    public DateOnly Fecha { get; set; }

    public decimal Temperatura { get; set; }

    public string DescripciónClima { get; set; } = null!;
}
