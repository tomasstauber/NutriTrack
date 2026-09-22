using System;
using System.Collections.Generic;

namespace NutriTrack.API.DTOs
{
    public class ReporteInventarioAnimalesResponseDTO
    {
        public int TotalAnimales { get; set; }
        public List<AnimalInventarioItemDTO> Animales { get; set; } = new();
    }

    public class AnimalInventarioItemDTO
    {
        public string Caravana { get; set; }
        public string Raza { get; set; }
        public string Sexo { get; set; }
        public string Edad { get; set; }
        public DateTime FechaAltaSistema { get; set; }
        public string EstadoActual { get; set; }
        public string? RodeoActual { get; set; }
    }
}