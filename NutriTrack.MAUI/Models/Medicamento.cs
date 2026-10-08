namespace NutriTrack.MAUI.Models
{
    // Medicamento del catálogo, tal como lo devuelve GET api/Medicamento
    // (NutriTrack.API.DTOs.MedicamentoResponseDTO).
    public class Medicamento
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }

        // Texto de la columna Estado de la lista
        public string Estado => Activo ? "Activo" : "Inactivo";

        // No vienen de la API: acciones de la fila según el rol y el estado.
        // Los asigna MedicamentosViewModel al cargar la lista

        // CU30 y CU31: Asesor y Administrador, solo en filas activas
        public bool PuedeEditarYDesactivar { get; set; }

        // CU32: solo Administrador, en filas inactivas
        public bool PuedeReactivar { get; set; }

        // Sin acciones (Encargado) la columna no ocupa lugar
        public bool TieneAcciones => PuedeEditarYDesactivar || PuedeReactivar;
    }
}
