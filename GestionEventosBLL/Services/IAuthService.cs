using System.Threading.Tasks;
using GestionEventosBLL.DTOs;

namespace GestionEventosBLL.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    }
}