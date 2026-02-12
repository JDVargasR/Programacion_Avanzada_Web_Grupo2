using System;
using System.Collections.Generic;

namespace APW.Data.Models.DB;

public partial class Role
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int EstadoId { get; set; }

    public virtual Estado Estado { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
