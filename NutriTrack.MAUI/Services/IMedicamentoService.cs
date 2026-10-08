using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public interface IMedicamentoService
    {
        // GET api/Medicamento: catálogo ordenado por nombre.
        // nombre busca en forma parcial y sin distinguir mayúsculas; vacío o null trae todo.
        // incluirInactivos = true suma los inactivos (solo Administrador: al resto el back responde 403).
        Task<ResultadoApi<List<Medicamento>>> ListarAsync(string? nombre = null, bool incluirInactivos = false);

        // POST api/Medicamento: devuelve el medicamento creado.
        Task<ResultadoApi<Medicamento>> CrearAsync(MedicamentoRequest medicamento);

        // PUT api/Medicamento/{id}: solo Administrador y Asesor técnico.
        Task<ResultadoApi<bool>> EditarAsync(int idMedicamento, MedicamentoRequest medicamento);

        // DELETE api/Medicamento/{id}: baja lógica. Solo Administrador y Asesor técnico.
        Task<ResultadoApi<bool>> DesactivarAsync(int idMedicamento);

        // PATCH api/Medicamento/activar/{id}: solo Administrador.
        Task<ResultadoApi<bool>> ReactivarAsync(int idMedicamento);
    }
}
