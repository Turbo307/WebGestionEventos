using System.Security.Claims;
using GestionEventosBLL.DTOs;
using GestionEventosBLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebGestionEventos.Controllers
{
    [Authorize] // Requiere Token JWT
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {
        private readonly IEventoService _eventoService;

        public EventosController(IEventoService eventoService)
        {
            _eventoService = eventoService;
        }

        /*[HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _eventoService.GetAllAsync();
            return Ok(result);
        }*/

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Extraer el ID del usuario directamente desde las Claims del Token JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized(new { mensaje = "Token inválido o expirado" });
            }

            // Retorna ÚNICAMENTE los eventos del usuario autenticado
            var result = await _eventoService.GetByUsuarioIdAsync(usuarioId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _eventoService.GetByIdAsync(id);
            if (result == null) return NotFound();

            // Opcional: Validar que el evento pertenezca al usuario que lo consulta
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int usuarioId) && result.IdUsuario != usuarioId)
            {
                return Forbid(); // 403 No tiene permiso para ver eventos de otros usuarios
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EventoDto dto)
        {
            // Asignar automáticamente el ID del usuario del Token para mayor seguridad
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int usuarioId))
            {
                dto.IdUsuario = usuarioId;
            }

            var created = await _eventoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.IdEvento }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EventoDto dto)
        {
            var updated = await _eventoService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _eventoService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}