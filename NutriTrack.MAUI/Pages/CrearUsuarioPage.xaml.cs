using System.Net.Http.Json;

namespace NutriTrack.MAUI.Pages;

public partial class CrearUsuarioPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5068/")
    };

    public CrearUsuarioPage()
    {
        InitializeComponent();
    }

    private async void CrearUsuario_Clicked(object sender, EventArgs e)
    {
        if (RolPicker.SelectedItem?.ToString() == "Administrador")
        {
            bool confirmar = await DisplayAlert(
                "Confirmar administrador",
                "¿Está seguro de que desea crear un usuario Administrador?",
                "Sí",
                "No");

            if (!confirmar)
                return;
        }

        var usuario = new
        {
            Nombre = NombreEntry.Text,
            Correo = CorreoEntry.Text,
            NombreUsuario = NombreUsuarioEntry.Text,
            Contrasenia = ContraseniaEntry.Text,
            Rol = RolPicker.SelectedItem?.ToString()
        };

        try
        {
            var respuesta = await _httpClient.PostAsJsonAsync(
                "api/Usuario",
                usuario);

            if (respuesta.IsSuccessStatusCode)
            {
                await DisplayAlert(
                    "Éxito",
                    "Usuario creado correctamente.",
                    "OK");

                NombreEntry.Text = "";
                CorreoEntry.Text = "";
                NombreUsuarioEntry.Text = "";
                ContraseniaEntry.Text = "";
                RolPicker.SelectedItem = null;
            }
            else
            {
                var mensaje = await respuesta.Content.ReadAsStringAsync();

                await DisplayAlert(
                    "Error",
                    mensaje,
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Error",
                "No se pudo conectar con el servidor.",
                "OK");
        }
    }
}