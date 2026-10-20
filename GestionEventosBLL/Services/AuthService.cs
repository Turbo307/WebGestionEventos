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

            var usuarios = await _usuarioRepository.GetAllAsync();
            var correoBuscar = dto.Correo.Trim().ToLower();
            var usuario = usuarios.FirstOrDefault(u => u.Correo != null && u.Correo.Trim().ToLower() == correoBuscar);

            if (usuario == null)
            {
                return null;
            }

            string inputHash = ConvertToMD5(dto.Password.Trim());
            string dbHash = (usuario.PasswordHash ?? string.Empty).Trim().ToLower();

            if (dbHash != inputHash)
            {
                return null;
            }

            var token = GenerarTokenJwt(usuario);

            return new AuthResponseDto
            {
                Token = token,
                Correo = usuario.Correo,
                Nombre = $"{usuario.Nombre} {usuario.Apellido}".Trim()
            };
        }

        // Conversion MD5
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