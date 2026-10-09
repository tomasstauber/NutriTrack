using System;
using System.Collections.Generic;

namespace NutriTrack.API.DTOs
{
    public class ReporteEvolucionPesoResponseDTO
    {
        public List<AnimalEvolucionPesoItemDTO> Animales { get; set; } = new();
        public DetallePesajesDTO? Detalle { get; set; }
    }

    public class AnimalEvolucionPesoItemDTO
    {
        public string Caravana { get; set; }
        // "Activo" / "Inactivo", igual que el reporte de inventario
        public string EstadoActual { get; set; }
        public DateTime FechaInicial { get; set; }
        public decimal PesoInicial { get; set; }
        public DateTime FechaFinal { get; set; }
        public decimal PesoFinal { get; set; }
        public decimal? VariacionKg { get; set; }
        public int CantidadRegistros { get; set; }
    }

    public class DetallePesajesDTO
    {
        public List<PesajeDetalleItemDTO> Pesajes { get; set; } = new();
    }

    public class PesajeDetalleItemDTO
    {
        public DateTime FechaPesaje { get; set; }
        public decimal PesoKg { get; set; }
    }
}