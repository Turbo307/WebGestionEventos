using System;
using System.Collections.Generic;

namespace GestionEventosDAL.Models;

public partial class Categoria
{
    public int IdCategoria { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Evento> Eventos { get; set; } = new List<Evento>();
}
