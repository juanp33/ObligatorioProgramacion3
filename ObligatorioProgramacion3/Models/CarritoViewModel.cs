namespace ObligatorioProgramacion3.Models
{
    public class CarritoViewModel
    {
        public IEnumerable<CarritoItem> Items { get; set; }
        public decimal Total { get; set; }
        public int reservaId { get; set; }
    }
}
