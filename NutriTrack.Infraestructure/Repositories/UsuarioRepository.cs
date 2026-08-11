using NutriTrack.Infraestructure.Data;
using NutriTrack.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace NutriTrack.Infraestructure.Repositories
{
    public class UsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CrearAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<bool> ExisteCorreoAsync(string correo)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.Correo.ToLower() == correo.ToLower());
        }

        public async Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.NombreUsuario.ToLower() == nombreUsuario.ToLower());
        }
    }
}