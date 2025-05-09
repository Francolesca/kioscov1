using kioscov1.Models.Entities;

namespace kioscov1.ViewsModel
{
    public class VentasVM
    {
        public Venta  Venta { get; set; }
        public List<DetalleVenta> Detalles { get; set; }
    }
}
