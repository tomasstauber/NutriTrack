using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.ViewModels
{
    // Fila de la grilla de componentes del plan (#114).
    // Se arma ya validada al tocar "Agregar" y no se edita: para cambiarla, se quita y se vuelve a agregar
    public class ComponentePlanFila
    {
        // CU11: componente que ya tenía el plan. CU27 R5: si el ingrediente no está
        // en el catálogo activo, la fila queda marcada como inactiva
        public ComponentePlanFila(PlanAlimenticioDetalle detalle, bool ingredienteInactivo)
        {
            IdIngrediente = detalle.IdIngrediente;
            NombreIngrediente = detalle.NombreIngrediente ?? $"Ingrediente {detalle.IdIngrediente}";
            PorcentajeInclusionMs = detalle.PorcentajeInclusionMs;
            Observaciones = detalle.Observaciones;
            IngredienteInactivo = ingredienteInactivo;
        }

        public ComponentePlanFila(Ingrediente ingrediente, decimal porcentajeInclusionMs, string? observaciones)
        {
            IdIngrediente = ingrediente.Id;
            NombreIngrediente = ingrediente.NombreIngrediente;
            PorcentajeInclusionMs = porcentajeInclusionMs;
            Observaciones = observaciones;
        }

        public int IdIngrediente { get; }
        public string NombreIngrediente { get; }
        public decimal PorcentajeInclusionMs { get; }
        public string? Observaciones { get; }
        public bool IngredienteInactivo { get; }

        public PlanAlimenticioDetalleRequest ArmarPedido() => new()
        {
            IdIngrediente = IdIngrediente,
            PorcentajeInclusionMs = PorcentajeInclusionMs,
            Observaciones = Observaciones
        };
    }
}
