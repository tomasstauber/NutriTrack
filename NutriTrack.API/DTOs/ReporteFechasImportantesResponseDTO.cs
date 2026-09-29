using System;
using System.Collections.Generic;

namespace NutriTrack.API.DTOs
{
    public class ReporteFechasImportantesResponseDTO
    {
        public List<ProximaAplicacionItemDTO> ProximasAplicaciones { get; set; } = new();
        public List<VencimientoEventoItemDTO> VencimientosEventos { get; set; } = new();
    }

    public class ProximaAplicacionItemDTO
    {
        public string Destino { get; set; }          
        public string TipoEvento { get; set; }       
        public string Producto { get; set; }          
        public DateTime FechaProximaAplicacion { get; set; } 
        public string Responsable { get; set; }      
    
    }

  
    public class VencimientoEventoItemDTO
    {
        public string Destino { get; set; }          
        public string TipoEvento { get; set; }        
        public string Producto { get; set; }          
        public DateTime VigenciaHasta { get; set; }    
        public string Responsable { get; set; }       
        public string? Observaciones { get; set; }     
    }
}