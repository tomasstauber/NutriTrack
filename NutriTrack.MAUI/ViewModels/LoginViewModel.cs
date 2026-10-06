using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        // Mismo mínimo que el LoginDTO del back ([MinLength(8)])
        private const int ContraseniaMinima = 8;

        private readonly IAuthService _authService;

        [ObservableProperty]
        public partial string NombreUsuario { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Contrasenia { get; set; } = string.Empty;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private Task Ingresar() => EjecutarAsync(async () =>
        {
            // Validaciones antes de llamar a la API
            if (string.IsNullOrWhiteSpace(NombreUsuario) || string.IsNullOrEmpty(Contrasenia))
            {
                MensajeError = "Ingresá el usuario y la contraseña.";
                return;
            }

            if (Contrasenia.Length < ContraseniaMinima)
            {
                MensajeError = $"La contraseña debe tener al menos {ContraseniaMinima} caracteres.";
                return;
            }

            var resultado = await _authService.IniciarSesionAsync(NombreUsuario.Trim(), Contrasenia);

            if (!resultado.Exito)
            {
                MensajeError = resultado.MensajeError;

                if (resultado.CodigoEstado == HttpStatusCode.Unauthorized)
                    Contrasenia = string.Empty;

                return;
            }

            // Shell reutiliza la LoginPage: se limpian los campos para que no
            // queden escritos al volver después de cerrar sesión
            NombreUsuario = string.Empty;
            Contrasenia = string.Empty;

            await Shell.Current.GoToAsync("//panel");
        });
    }
}
