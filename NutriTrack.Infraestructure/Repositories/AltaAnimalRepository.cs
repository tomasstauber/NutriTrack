using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Infraestructure.Repositories
{
    public class AltaAnimalRepository
    {
        private readonly AppDbContext _context;

        public AltaAnimalRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool ExisteCaravana(string cuig, string nroManejo)
        {
            return _context.Animales
                .Any(a => a.CaravanaCuig == cuig && a.CaravanaNroManejo == nroManejo);
        }

        public Animal? BuscarPorCaravana (string cuig, string nroManejo)
        {  return _context.Animales
                .FirstOrDefault(a => a.CaravanaCuig == cuig && a.CaravanaNroManejo == nroManejo);
        }
   
        public Animal Crear(Animal animal)
        {
            _context.Animales.Add(animal);
            _context.SaveChanges();
            return animal;
        }

    }
}
