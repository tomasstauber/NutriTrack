using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Helpers
{
    // Menú del panel principal: qué módulos existen, en qué ruta están
    // y qué roles los ven (matriz de menú por rol).
    //
    // Para que un módulo aparezca en el menú, su pantalla tiene que registrar
    // la ruta con MenuModulos.RegistrarRuta<TPagina>(...) en AppShell.xaml.cs,
    // y la página y su ViewModel registrarse en MauiProgram.cs.
    // Mientras un módulo no registre su ruta, simplemente no aparece.
    public static class MenuModulos
    {
        // Rutas de cada módulo
        public const string Animales = "animales";
        public const string Rodeos = "rodeos";
        public const string PlanAlimenticio = "planes-alimenticios";
        public const string Ingredientes = "ingredientes";
        public const string Medicamentos = "medicamentos";
        public const string EventoSanitario = "eventos-sanitarios";
        public const string Peso = "pesos";
        public const string Usuarios = "usuarios";
        public const string Reportes = "reportes";

        private static readonly IReadOnlyList<EntradaMenu> Entradas =
        [
            new() { Titulo = "Animales", Ruta = Animales, Orden = 1, Grupo = GrupoModulo.Animal,
                    Roles = [RolUsuario.Administrador, RolUsuario.EncargadoDeCampo] },

            new() { Titulo = "Rodeos", Ruta = Rodeos, Orden = 2, Grupo = GrupoModulo.Animal,
                    Roles = [RolUsuario.Administrador, RolUsuario.EncargadoDeCampo] },

            new() { Titulo = "Plan alimenticio", Ruta = PlanAlimenticio, Orden = 3, Grupo = GrupoModulo.Alimenticio,
                    Roles = [RolUsuario.Administrador, RolUsuario.AsesorTecnico] },

            new() { Titulo = "Ingredientes", Ruta = Ingredientes, Orden = 4, Grupo = GrupoModulo.Alimenticio,
                    Roles = [RolUsuario.Administrador, RolUsuario.AsesorTecnico] },

            new() { Titulo = "Medicamentos", Ruta = Medicamentos, Orden = 5, Grupo = GrupoModulo.Sanitario,
                    Roles = [RolUsuario.Administrador, RolUsuario.EncargadoDeCampo, RolUsuario.AsesorTecnico] },

            new() { Titulo = "Evento sanitario", Ruta = EventoSanitario, Orden = 6, Grupo = GrupoModulo.Sanitario,
                    Roles = [RolUsuario.Administrador, RolUsuario.EncargadoDeCampo, RolUsuario.AsesorTecnico] },

            new() { Titulo = "Registrar peso", Ruta = Peso, Orden = 7, Grupo = GrupoModulo.Animal,
                    Roles = [RolUsuario.Administrador, RolUsuario.EncargadoDeCampo] },

            new() { Titulo = "Usuarios", Ruta = Usuarios, Orden = 8, Grupo = GrupoModulo.Administracion,
                    Roles = [RolUsuario.Administrador] },

            new() { Titulo = "Reportes", Ruta = Reportes, Orden = 9, Grupo = GrupoModulo.Administracion,
                    Roles = [RolUsuario.Administrador] }
        ];

        // Título, descripción y sigla de la tarjeta de cada módulo
        private static readonly IReadOnlyDictionary<GrupoModulo, (string Titulo, string Descripcion, string Sigla)> Grupos =
            new Dictionary<GrupoModulo, (string, string, string)>
            {
                [GrupoModulo.Animal] = ("Animal", "Hacienda, rodeos y pesajes", "An"),
                [GrupoModulo.Sanitario] = ("Sanitario", "Eventos sanitarios y medicamentos", "Sa"),
                [GrupoModulo.Alimenticio] = ("Alimenticio", "Planes de alimentación e ingredientes", "Al"),
                [GrupoModulo.Administracion] = ("Administración", "Usuarios y reportes", "Ad")
            };

        // Rutas que ya registró algún módulo
        private static readonly HashSet<string> RutasRegistradas = [];

        // Registra la ruta de Shell de un módulo y lo habilita en el menú.
        // Uso en AppShell.xaml.cs:  MenuModulos.RegistrarRuta<AnimalesPage>(MenuModulos.Animales);
        public static void RegistrarRuta<TPagina>(string ruta) where TPagina : Page
        {
            Routing.RegisterRoute(ruta, typeof(TPagina));
            RutasRegistradas.Add(ruta);
        }

        // Entradas que ve un rol, ordenadas, solo de módulos con ruta registrada
        public static IReadOnlyList<EntradaMenu> ObtenerPara(RolUsuario rol) =>
            Entradas
                .Where(e => e.Roles.Contains(rol) && RutasRegistradas.Contains(e.Ruta))
                .OrderBy(e => e.Orden)
                .ToList();

        // Agrupa las entradas por módulo, en el orden del enum GrupoModulo.
        // Un módulo sin entradas para el rol no genera grupo.
        public static IReadOnlyList<GrupoMenu> AgruparPorCategoria(IEnumerable<EntradaMenu> entradas) =>
            entradas
                .GroupBy(e => e.Grupo)
                .OrderBy(g => g.Key)
                .Select(g => new GrupoMenu
                {
                    Clave = g.Key.ToString(),
                    Titulo = Grupos[g.Key].Titulo,
                    Descripcion = Grupos[g.Key].Descripcion,
                    Sigla = Grupos[g.Key].Sigla,
                    Entradas = g.ToList()
                })
                .ToList();
    }
}
