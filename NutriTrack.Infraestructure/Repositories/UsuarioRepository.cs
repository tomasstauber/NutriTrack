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

        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<bool> ExisteCorreoAsync(string correo, int? idExcluir = null)
        {
            return await _context.Usuarios
                .AnyAsync(u =>
                    u.Correo.ToLower() == correo.ToLower() &&
                    (!idExcluir.HasValue || u.Id != idExcluir.Value));
        }

        public async Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int? idExcluir = null)
        {
            return await _context.Usuarios
                .AnyAsync(u =>
                    u.NombreUsuario.ToLower() == nombreUsuario.ToLower() &&
                    (!idExcluir.HasValue || u.Id != idExcluir.Value));
        }

        public async Task<bool> ActualizarAsync(Usuario usuario)
        {
            var existente = await _context.Usuarios.FindAsync(usuario.Id);

            if (existente == null)
                return false;

            existente.Nombre = usuario.Nombre;
            existente.Correo = usuario.Correo;
            existente.NombreUsuario = usuario.NombreUsuario;
            existente.Rol = usuario.Rol;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}