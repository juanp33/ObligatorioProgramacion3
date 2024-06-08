using System;
using System.Collections.Generic;

namespace ObligatorioProgramacion3.Models;

public partial class Permiso
{
    public int PermisoId { get; set; }

    public string NombrePagina { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
}
