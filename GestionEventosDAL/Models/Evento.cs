using System;
using System.Collections.Generic;

namespace GestionEventosDAL.Models;

public partial class Evento
{
    public int IdEvento { get; set; }

    public string NombreEvento { get; set; } = null!;

    public DateOnly Fecha { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public string? Descripcion { get; set; }

    public string? Estado { get; set; }

    public bool? Recordatorio { get; set; }

    public int IdUsuario { get; set; }

    public int IdCategoria { get; set; }

    public int IdPrioridad { get; set; }

    public int IdEstado { get; set; }

    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;

    public virtual Estado IdEstadoNavigation { get; set; } = null!;

    public virtual Prioridad IdPrioridadNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
