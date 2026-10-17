using System;
using System.Collections.Generic;

namespace GestionEventosDAL.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string? Telefono { get; set; }

    public string Correo { get; set; } = null!;

    public int? Edad { get; set; }

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<Evento> Eventos { get; set; } = new List<Evento>();
}
