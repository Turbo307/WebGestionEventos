using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GestionEventosBLL.DTOs;
using GestionEventosDAL.Models;
using GestionEventosDAL.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GestionEventosBLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<Usuario> _usuarioRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IRepository<Usuario> usuarioRepository, IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Correo) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return null;
            }

            // 1. Obtener todos los usuarios de la base de datos
            var usuarios = await _usuarioRepository.GetAllAsync();

            // 2. Normalizar el correo a minúsculas y buscar en la lista (compatible con EF Core y C#)
            var correoBuscar = dto.Correo.Trim().ToLower();
            var usuario = usuarios.FirstOrDefault(u => u.Correo != null && u.Correo.Trim().ToLower() == correoBuscar);

            if (usuario == null)
            {
                // El correo no existe en la base de datos
                return null;
            }

            // 3. Generar el Hash MD5 de la contraseña ingresada
            string inputHash = ConvertToMD5(dto.Password.Trim());

            // 4. Limpiar el Hash obtenido de la base de datos (remover espacios)
            string dbHash = (usuario.PasswordHash ?? string.Empty).Trim().ToLower();

            // 5. Verificar coincidencia exacta de Hashes
            if (dbHash != inputHash)
            {
                // La contraseña no coincide
                return null;
            }

            // 6. Generar el Token JWT
            var token = GenerarTokenJwt(usuario);

            return new AuthResponseDto
            {
                Token = token,
                Correo = usuario.Correo,
                Nombre = $"{usuario.Nombre} {usuario.Apellido}".Trim()
            };
        }

        // Convierte la contraseña en texto plano a su equivalente MD5 (32 caracteres hexadecimales en minúscula)
        private static string ConvertToMD5(string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private string GenerarTokenJwt(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Email, usuario.Correo ?? string.Empty),
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}".Trim())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["DurationInMinutes"]!)),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}