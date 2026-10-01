using Microsoft.IdentityModel.Tokens;
using NutriTrack.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NutriTrack.API.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public (string Token, DateTime Expiracion) GenerarToken(Usuario usuario)
        {
            // 1. Leer la configuración de Jwt.
            var key = _config["Jwt:Key"]
                ?? throw new InvalidOperationException("Falta Jwt:Key en la configuración.");
            var issuer = _config["Jwt:Issuer"]
                ?? throw new InvalidOperationException("Falta Jwt:Issuer en la configuración.");
            var audience = _config["Jwt:Audience"]
                ?? throw new InvalidOperationException("Falta Jwt:Audience en la configuración.");

            // 2. Claims: lo que el token afirma sobre el usuario.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString())
            };

            // 3. y 4. Clave y credenciales de firma
            var claveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credenciales = new SigningCredentials(claveSimetrica, SecurityAlgorithms.HmacSha256);

            // 5. Calcular expiración
            var expiracion = DateTime.UtcNow.AddHours(8);

            // 6. Armar el objeto token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiracion,
                signingCredentials: credenciales);

            // 7. Convertirlo al string
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // 8. Devolver la tupla: TODO: se haría mejor con clase o record.
            return (tokenString, expiracion);
        }
    }
}
