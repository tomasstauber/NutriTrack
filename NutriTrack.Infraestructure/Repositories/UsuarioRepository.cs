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

        public async Task<List<Usuario>> BuscarAsync(string? busqueda, string? rol)
        {
            var query = _context.Usuarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                busqueda = busqueda.ToLower();

                query = query.Where(u =>
                    u.Nombre.ToLower().Contains(busqueda) ||
                    u.Correo.ToLower().Contains(busqueda));
            }

            if (!string.IsNullOrWhiteSpace(rol))
            {
                query = query.Where(u => u.Rol == rol);
            }

            return await query.ToListAsync();
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