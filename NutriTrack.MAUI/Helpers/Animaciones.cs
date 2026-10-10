namespace NutriTrack.MAUI.Helpers
{
    // Animaciones de hover, presión y aparición que se activan desde el XAML
    // con propiedades adjuntas. Solo mueven y escalan elementos: no tienen
    // lógica de negocio. Los colores del hover van con VisualStateManager.
    //
    // Uso:
    //   helpers:Animaciones.Interactivo="True"    fila o botón: se achica al presionar
    //                                             y mueve sus flechas al pasar el mouse
    //   helpers:Animaciones.Desplazable="True"    marca la flecha que se mueve en el hover
    //   helpers:Animaciones.ElevarAlPasar="True"  tarjeta: se agranda un poco al pasar el mouse
    //   helpers:Animaciones.Aparicion="True"      aparece con fade al cargar, escalonada
    //                                             según su posición en el layout
    //
    // En Android no hay puntero: solo queda la animación al presionar.
    public static class Animaciones
    {
        private const uint DuracionPresion = 120;
        private const uint DuracionHover = 150;
        private const uint DuracionAparicion = 200;

        private const double EscalaPresionada = 0.98;
        private const double EscalaElevada = 1.02;
        private const double DesplazamientoFlecha = 4;
        private const double DesplazamientoAparicion = 12;
        private const int RetrasoPorPosicion = 60;

        public static readonly BindableProperty InteractivoProperty =
            BindableProperty.CreateAttached("Interactivo", typeof(bool), typeof(Animaciones), false,
                propertyChanged: AlActivarInteractivo);

        public static bool GetInteractivo(BindableObject elemento) => (bool)elemento.GetValue(InteractivoProperty);
        public static void SetInteractivo(BindableObject elemento, bool valor) => elemento.SetValue(InteractivoProperty, valor);

        public static readonly BindableProperty DesplazableProperty =
            BindableProperty.CreateAttached("Desplazable", typeof(bool), typeof(Animaciones), false);

        public static bool GetDesplazable(BindableObject elemento) => (bool)elemento.GetValue(DesplazableProperty);
        public static void SetDesplazable(BindableObject elemento, bool valor) => elemento.SetValue(DesplazableProperty, valor);

        public static readonly BindableProperty ElevarAlPasarProperty =
            BindableProperty.CreateAttached("ElevarAlPasar", typeof(bool), typeof(Animaciones), false,
                propertyChanged: AlActivarElevarAlPasar);

        public static bool GetElevarAlPasar(BindableObject elemento) => (bool)elemento.GetValue(ElevarAlPasarProperty);
        public static void SetElevarAlPasar(BindableObject elemento, bool valor) => elemento.SetValue(ElevarAlPasarProperty, valor);

        public static readonly BindableProperty AparicionProperty =
            BindableProperty.CreateAttached("Aparicion", typeof(bool), typeof(Animaciones), false,
                propertyChanged: AlActivarAparicion);

        public static bool GetAparicion(BindableObject elemento) => (bool)elemento.GetValue(AparicionProperty);
        public static void SetAparicion(BindableObject elemento, bool valor) => elemento.SetValue(AparicionProperty, valor);

        private static void AlActivarInteractivo(BindableObject elemento, object valorAnterior, object valorNuevo)
        {
            if (elemento is not View vista || valorNuevo is not true)
                return;

            // El botón ya avisa cuando se presiona, también con el dedo
            if (vista is Button boton)
            {
                boton.Pressed += (_, _) => _ = PresionarAsync(boton);
                return;
            }

            var puntero = new PointerGestureRecognizer();
            puntero.PointerEntered += (_, _) => MoverFlechas(vista, DesplazamientoFlecha);
            puntero.PointerExited += (_, _) => MoverFlechas(vista, 0);
            puntero.PointerPressed += (_, _) => _ = PresionarAsync(vista);
            vista.GestureRecognizers.Add(puntero);
        }

        private static void AlActivarElevarAlPasar(BindableObject elemento, object valorAnterior, object valorNuevo)
        {
            if (elemento is not View vista || valorNuevo is not true)
                return;

            var puntero = new PointerGestureRecognizer();
            puntero.PointerEntered += (_, _) => _ = vista.ScaleToAsync(EscalaElevada, DuracionHover, Easing.CubicOut);
            puntero.PointerExited += (_, _) => _ = vista.ScaleToAsync(1, DuracionHover, Easing.CubicOut);
            vista.GestureRecognizers.Add(puntero);
        }

        private static void AlActivarAparicion(BindableObject elemento, object valorAnterior, object valorNuevo)
        {
            if (elemento is not VisualElement vista || valorNuevo is not true)
                return;

            // Se oculta recién al cargarse: si Loaded no llegara, el elemento queda visible
            vista.Loaded += AlCargar;

            static void AlCargar(object? sender, EventArgs e)
            {
                var vista = (VisualElement)sender!;
                vista.Loaded -= AlCargar;
                _ = AparecerAsync(vista);
            }
        }

        // Escala breve: baja y vuelve, así no queda achicado si no llega el "soltar"
        private static async Task PresionarAsync(VisualElement vista)
        {
            await vista.ScaleToAsync(EscalaPresionada, DuracionPresion, Easing.CubicOut);
            await vista.ScaleToAsync(1, DuracionPresion, Easing.CubicOut);
        }

        private static void MoverFlechas(View vista, double desplazamiento)
        {
            var flechas = vista.GetVisualTreeDescendants()
                .OfType<VisualElement>()
                .Where(GetDesplazable);

            foreach (var flecha in flechas)
                _ = flecha.TranslateToAsync(desplazamiento, 0, DuracionHover, Easing.CubicOut);
        }

        // Fade y desplazamiento hacia arriba, escalonado según la posición en el layout
        private static async Task AparecerAsync(VisualElement vista)
        {
            var posicion = vista.Parent is Layout layout ? Math.Max(layout.IndexOf(vista), 0) : 0;

            vista.Opacity = 0;
            vista.TranslationY = DesplazamientoAparicion;

            await Task.Delay(posicion * RetrasoPorPosicion);
            await Task.WhenAll(
                vista.FadeToAsync(1, DuracionAparicion, Easing.CubicOut),
                vista.TranslateToAsync(0, 0, DuracionAparicion, Easing.CubicOut));
        }
    }
}
