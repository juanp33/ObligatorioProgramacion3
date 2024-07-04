namespace ObligatorioProgramacion3.Models
{
    public class SelectMesa
    {
        public DateTime Fecha {  get; set; }
        public int restauranteId { get; set; }
        public List<ChechboxMesa> ChechboxMesa { get; set; }

        
    }
}
