using kioscov1.Models.Entities;

namespace kioscov1.ViewsModel
{
    public class MovimientoStockVM
    {
        public MovimientoStockInput MovimientoStock { get; set;}
        public List<DetalleMovimientoStockVM>? DetalleMovimientoStock { get; set; }
    }
    public class MovimientoStockInput
    {
        public DateTime Fecha { get; set; }
        public string Origen { get; set; }
        public string Comentario { get; set; }
    }
    public class DetalleMovimientoStockVM
    {
        public int ProductoId { get; set; }
        public int Cantidad {  get; set; }
    }
}
