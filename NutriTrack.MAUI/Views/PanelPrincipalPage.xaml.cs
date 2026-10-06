using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class PanelPrincipalPage : ContentPage
    {
        private readonly PanelPrincipalViewModel _vm;

        public PanelPrincipalPage(PanelPrincipalViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm; // conecta la View con su ViewModel
        }

        // Shell reutiliza la página: se recarga cada vez que aparece
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _vm.CargarCommand.Execute(null);
        }
    }
}
