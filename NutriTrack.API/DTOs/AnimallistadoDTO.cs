using System;
using System.Collections.Generic;

namespace NutriTrack.API.DTOs
{
    // Fila del listado de animales.
    public class AnimalListadoDTO
    {
        public int Id { get; set; }
        public string CaravanaCuig { get; set; } = string.Empty;
        public string CaravanaNroManejo { get; set; } = string.Empty;
        public string? Raza { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public bool Estado { get; set; }
        public int? RodeoId { get; set; }
        public string? RodeoNombre { get; set; }
    }

    // Envoltorio genérico para cualquier listado paginado
    // solo para este porqeu son muchos animales
    public class ListadoPaginadoDTO<T>
    {
        public List<T> Items { get; set; } = new();
        public int Total { get; set; }
        public int Pagina { get; set; }
        public int TamanioPagina { get; set; }
    }
}