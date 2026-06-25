namespace NutriTrack.API.DTOs
{
    public class ConsultaFichaIndividualAnimalDTO
    {
        public string CaravanaCuig {  get; set; }
        public string CaravanaNroManejo { get; set; }
        public DateTime FechaNacimiento  {get; set; }
        public float PesoAlNacer { get; set; }
        public string Sexo {  get; set; }
        public string Raza { get; set; }
        public DateTime FechaAlta { get; set; }
        public string? ColorPelaje { get; set; }
        public string Estado { get; set; }
        public String? RodeoActual { get; set; }
        public string? Madre { get; set; }
        public string? Padre { get; set; }
        public UltimoPesoDTO? UltimoPeso {  get; set; }
         
    }

    //funcion pq hay dos datos asi lo agrupo en un objeto
    public class UltimoPesoDTO
    {
        public DateTime FechaPesaje {  get; set; }
        public float PesoKg { get; set; }
    }
}
