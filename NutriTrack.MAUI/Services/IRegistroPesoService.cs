using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public interface IRegistroPesoService
    {
        // POST api/RegistroPeso: registra un pesaje de un animal activo.
        Task<ResultadoApi<bool>> RegistrarAsync(RegistroPesoRequest registro);
    }
}
