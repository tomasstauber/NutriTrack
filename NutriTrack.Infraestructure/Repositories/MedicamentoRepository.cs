using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace NutriTrack.Infraestructure.Repositories
{
    public class MedicamentoRepository
    {
        private readonly AppDbContext _context;

        public MedicamentoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Medicamento> AgregarAsync(Medicamento medicamento)
        {
            await _context.Medicamentos.AddAsync(medicamento);
            await _context.SaveChangesAsync();
            return medicamento;
        }

        public async Task<List<Medicamento>> ObtenerTodosAsync(bool incluirInactivos = false, string? nombreMedicamento = null)
        {
            var query = _context.Medicamentos.AsQueryable();

            if (!incluirInactivos)
                query = query.Where(m => m.Activo);

            if (!string.IsNullOrWhiteSpace(nombreMedicamento))
                query = query.Where(m => m.Nombre.ToLower().Contains(nombreMedicamento.ToLower()));

            return await query.ToListAsync();
        }

        public async Task<Medicamento?> ObtenerPorIdAsync(int id)
        {
            return await _context.Medicamentos
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> VerificarNombreUnicoExcluyendo(string nombre, int id)
        {
            return await _context.Medicamentos
                .AnyAsync(m => m.Nombre.ToLower() == nombre.ToLower() && m.Id != id);
        }

        public async Task<bool> VerificarNombreUnico(string nombre)
        {
            return await _context.Medicamentos
                .AnyAsync(m => m.Nombre.ToLower() == nombre.ToLower());
        }

        public async Task ActualizarAsync(Medicamento medicamento)
        {
            _context.Medicamentos.Update(medicamento);
            await _context.SaveChangesAsync();
        }

        public async Task DesactivarAsync(int id)
        {
            var medicamento = await _context.Medicamentos.FindAsync(id);
            if (medicamento is not null)
            {
                medicamento.Activo = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ActivarAsync(int id)
        {
            var medicamento = await _context.Medicamentos.FindAsync(id);
            if (medicamento is not null)
            {
                medicamento.Activo = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}