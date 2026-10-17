using GestionEventosBLL.DTOs;

namespace GestionEventosBLL.Services
{
    public interface IEventoService
    {
        Task<IEnumerable<EventoDto>> GetAllAsync();
        Task<EventoDto?> GetByIdAsync(int id);
        Task<EventoDto> CreateAsync(EventoDto dto);
        Task<bool> UpdateAsync(int id, EventoDto dto);
        Task<bool> DeleteAsync(int id);
    }
}