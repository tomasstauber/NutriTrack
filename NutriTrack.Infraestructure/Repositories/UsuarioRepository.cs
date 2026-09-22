using NutriTrack.Infraestructure.Data;
using NutriTrack.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Isopoh.Cryptography.Argon2;

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
            usuario.Activo = true;
            usuario.Contrasenia = Argon2.Hash(usuario.Contrasenia);

            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            return await _context.Usuarios
                .Where(u => u.Activo)
                .ToListAsync();
        }

        public async Task<List<Usuario>> BuscarAsync(string? busqueda, RolUsuario? rol)
        {
            var query = _context.Usuarios
                .Where(u => u.Activo)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                busqueda = busqueda.ToLower();

                query = query.Where(u =>
                    u.Nombre.ToLower().Contains(busqueda) ||
                    u.Correo.ToLower().Contains(busqueda));
            }

            if (rol.HasValue)
            {
                query = query.Where(u => u.Rol == rol.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id && u.Activo);
        }

        public async Task<bool> ExisteCorreoAsync(string correo, int? idExcluir = null)
        {
            return await _context.Usuarios
                .AnyAsync(u =>
                    u.Activo &&
                    u.Correo.ToLower() == correo.ToLower() &&
                    (!idExcluir.HasValue || u.Id != idExcluir.Value));
        }

        public async Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int? idExcluir = null)
        {
            return await _context.Usuarios
                .AnyAsync(u =>
                    u.Activo &&
                    u.NombreUsuario.ToLower() == nombreUsuario.ToLower() &&
                    (!idExcluir.HasValue || u.Id != idExcluir.Value));
        }

        public async Task<bool> ActualizarAsync(Usuario usuario)
        {
            var existente = await _context.Usuarios.FindAsync(usuario.Id);

            if (existente == null || !existente.Activo)
                return false;

            existente.Nombre = usuario.Nombre;
            existente.Correo = usuario.Correo;
            existente.NombreUsuario = usuario.NombreUsuario;
            existente.Rol = usuario.Rol;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> ContarAdministradoresActivosAsync()
        {
            return await _context.Usuarios
                .CountAsync(u => u.Activo && u.Rol == RolUsuario.Administrador);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null || !usuario.Activo)
                return false;

            usuario.Activo = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}