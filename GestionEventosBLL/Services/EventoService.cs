using GestionEventosBLL.DTOs;
using GestionEventosDAL.Models;
using GestionEventosDAL.Repositories;

namespace GestionEventosBLL.Services
{
    public class EventoService : IEventoService
    {
        private readonly IRepository<Evento> _repository;

        public EventoService(IRepository<Evento> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<EventoDto>> GetAllAsync()
        {
            var eventos = await _repository.GetAllAsync();
            return eventos.Select(e => new EventoDto
            {
                IdEvento = e.IdEvento,
                NombreEvento = e.NombreEvento,
                Fecha = e.Fecha,
                HoraInicio = e.HoraInicio,
                HoraFin = e.HoraFin,
                Descripcion = e.Descripcion,
                Estado = e.Estado,
                Recordatorio = e.Recordatorio,
                IdUsuario = e.IdUsuario,
                IdCategoria = e.IdCategoria,
                IdPrioridad = e.IdPrioridad,
                IdEstado = e.IdEstado
            });
        }

        public async Task<EventoDto?> GetByIdAsync(int id)
        {
            var e = await _repository.GetByIdAsync(id);
            if (e == null) return null;

            return new EventoDto
            {
                IdEvento = e.IdEvento,
                NombreEvento = e.NombreEvento,
                Fecha = e.Fecha,
                HoraInicio = e.HoraInicio,
                HoraFin = e.HoraFin,
                Descripcion = e.Descripcion,
                Estado = e.Estado,
                Recordatorio = e.Recordatorio,
                IdUsuario = e.IdUsuario,
                IdCategoria = e.IdCategoria,
                IdPrioridad = e.IdPrioridad,
                IdEstado = e.IdEstado
            };
        }

        public async Task<EventoDto> CreateAsync(EventoDto dto)
        {
            var entity = new Evento
            {
                NombreEvento = dto.NombreEvento,
                Fecha = dto.Fecha,
                HoraInicio = dto.HoraInicio,
                HoraFin = dto.HoraFin,
                Descripcion = dto.Descripcion,
                Estado = dto.Estado,
                Recordatorio = dto.Recordatorio,
                IdUsuario = dto.IdUsuario,
                IdCategoria = dto.IdCategoria,
                IdPrioridad = dto.IdPrioridad,
                IdEstado = dto.IdEstado
            };

            await _repository.AddAsync(entity);
            await _repository.SaveAsync();

            dto.IdEvento = entity.IdEvento;
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, EventoDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            entity.NombreEvento = dto.NombreEvento;
            entity.Fecha = dto.Fecha;
            entity.HoraInicio = dto.HoraInicio;
            entity.HoraFin = dto.HoraFin;
            entity.Descripcion = dto.Descripcion;
            entity.Estado = dto.Estado;
            entity.Recordatorio = dto.Recordatorio;
            entity.IdUsuario = dto.IdUsuario;
            entity.IdCategoria = dto.IdCategoria;
            entity.IdPrioridad = dto.IdPrioridad;
            entity.IdEstado = dto.IdEstado;

            _repository.Update(entity);
            await _repository.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            _repository.Delete(entity);
            await _repository.SaveAsync();
            return true;
        }
    }
}