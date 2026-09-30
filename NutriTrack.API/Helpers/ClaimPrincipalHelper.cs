using System.Security.Claims;

namespace NutriTrack.API.Helpers
{
    public static class ClaimPrincipalHelper
    {
        public static int ObtenerId(this ClaimsPrincipal user)
        {
            //busca el claim del id, si no está devuelve null
            var valor = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //TryParse convierte texto a int, si funciona deja el nro en id y devuelve true.
            if (!int.TryParse(valor, out var id))
                throw new InvalidOperationException("El token no contiene un id de usuario válido.");

            return id;
        }
    }
}
