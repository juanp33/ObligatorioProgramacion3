using System;
using System.Collections.Generic;

namespace ObligatorioProgramacion3.Models;

public partial class RolPermiso
{
    public int RolPermisoId { get; set; }

    public int RolId { get; set; }

    public int PermisoId { get; set; }

    public virtual Permiso Permiso { get; set; } = null!;

    public virtual Role Rol { get; set; } = null!;
}
