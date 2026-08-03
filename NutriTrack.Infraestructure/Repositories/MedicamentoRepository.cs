using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

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


    }
}
