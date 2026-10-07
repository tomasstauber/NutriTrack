namespace NutriTrack.MAUI.Models
{
    public enum TipoOrigenAnimales
    {
        SinRodeo,
        DeRodeo,
        Todos
    }

    // De dónde salen los animales del selector múltiple (#101).
    // Solo se crea con SinRodeo, DeRodeo(id) o Todos: así no se puede pedir
    // idRodeo y sinRodeo juntos (el back responde 400).
    // Es un record: dos orígenes con los mismos datos son iguales (==),
    // así el selector sabe si le pidieron "el mismo origen" o uno distinto.
    public sealed record OrigenAnimales
    {
        public TipoOrigenAnimales Tipo { get; }

        // Solo tiene valor si Tipo es DeRodeo
        public int? IdRodeo { get; }

        private OrigenAnimales(TipoOrigenAnimales tipo, int? idRodeo)
        {
            Tipo = tipo;
            IdRodeo = idRodeo;
        }

        // Animales activos sin rodeo asignado (GET api/Animal?sinRodeo=true)
        public static OrigenAnimales SinRodeo { get; } = new(TipoOrigenAnimales.SinRodeo, null);

        // Animales activos de un rodeo (GET api/Animal?idRodeo={id})
        public static OrigenAnimales DeRodeo(int idRodeo) => new(TipoOrigenAnimales.DeRodeo, idRodeo);

        // Todos los animales activos (GET api/Animal sin filtro de rodeo)
        public static OrigenAnimales Todos { get; } = new(TipoOrigenAnimales.Todos, null);
    }
}
