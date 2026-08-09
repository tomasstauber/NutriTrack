using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Core.Entities
{
    public class Medicamento
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;
    }
}
