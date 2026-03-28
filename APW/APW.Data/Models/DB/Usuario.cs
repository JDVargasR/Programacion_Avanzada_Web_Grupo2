using System;
using System.Collections.Generic;

namespace APW.Data.Models.DB;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public int RolId { get; set; }

    public int EstadoId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Estado Estado { get; set; } = null!;

    public virtual Role Rol { get; set; } = null!;
}
