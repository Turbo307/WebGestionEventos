using System.Threading.Tasks;
using GestionEventosBLL.DTOs;
using GestionEventosBLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebGestionEventos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var response = await _authService.LoginAsync(dto);
            if (response == null)
            {
                return Unauthorized(new { mensaje = "Credenciales invalidas" });
            }
            return Ok(response);
        }
    }
}