namespace NutriTrack.MAUI.Models
{
    // Respuesta de los listados paginados de la API (en el back: ListadoPaginadoDTO<T>)
    public class ListadoPaginado<T>
    {
        public List<T> Items { get; set; } = [];
        public int Total { get; set; }
        public int Pagina { get; set; }
        public int TamanioPagina { get; set; }
    }
}
