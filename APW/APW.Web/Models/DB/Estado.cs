using System;
using System.Collections.Generic;

namespace APW.Web.Models.DB;

public partial class Estado
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
