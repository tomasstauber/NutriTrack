namespace NutriTrack.API.DTOs
{
    public class ConsultaFichaIndividualAnimalDTO
    {
        public int Id { get; set; }
        public string CaravanaCuig {  get; set; }
        public string CaravanaNroManejo { get; set; }
        public DateTime FechaNacimiento  {get; set; }
        public decimal PesoAlNacer { get; set; }
        public string Sexo {  get; set; }
        public string Raza { get; set; }
        public DateTime FechaAlta { get; set; }
        public string? ColorPelaje { get; set; }
        public string Estado { get; set; }
        public string? RodeoActual { get; set; }
        public string? Madre { get; set; }
        public string? Padre { get; set; }
        public UltimoPesoDTO? UltimoPeso {  get; set; }
         
    }

    public class UltimoPesoDTO
    {
        public int Id { get; set; }
        public DateTime FechaPesaje {  get; set; }
        public decimal PesoKg { get; set; }
        public string? Observaciones { get; set; }
    }
}
