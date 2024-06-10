﻿using System;
using System.Collections.Generic;

namespace ObligatorioProgramacion3.Models;

public partial class Role
{
    public int RolId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();

    public virtual ICollection<UsuarioRole> UsuarioRoles { get; set; } = new List<UsuarioRole>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
