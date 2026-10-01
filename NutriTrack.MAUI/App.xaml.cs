using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI
{
    public partial class App : Application
    {
        private readonly ISesionService _sesionService;

        public App(ISesionService sesionService)
        {
            InitializeComponent();          // 1) carga Colors.xaml y Styles.xaml
            UserAppTheme = AppTheme.Light;
            _sesionService = sesionService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // 2) recién acá se crea el Shell, cuando los colores ya existen
            return new Window(new AppShell(_sesionService));
        }
    }
}