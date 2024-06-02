using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public string Rol {  get; set; } = null!;
}

 
