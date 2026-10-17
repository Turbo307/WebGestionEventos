namespace GestionEventosBLL.DTOs
{
    public class EventoDto
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
    }
}