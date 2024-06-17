namespace ObligatorioProgramacion3.Models
{
    public class CarritoService
    {
        private readonly List<CarritoItem> _carritoItems = new List<CarritoItem>();

        public void AñadirAlCarrito(CarritoItem item)
        {
            var existingItem = _carritoItems.FirstOrDefault(i => i.PlatoId == item.PlatoId);
            if (existingItem != null)
            {
                existingItem.Cantidad++;
            }
            else
            {
                _carritoItems.Add(item);
            }
        }

        public void EliminarDelCarrito(int platoId)
        {
            var item = _carritoItems.FirstOrDefault(i => i.PlatoId == platoId);
            if (item != null)
            {
                _carritoItems.Remove(item);
            }
        }

        public IEnumerable<CarritoItem> ObtenerCarritoItems()
        {
            return _carritoItems;
        }

        public decimal ObtenerTotal()
        {
            return _carritoItems.Sum(i => i.Precio * i.Cantidad);
        }
    }
}
