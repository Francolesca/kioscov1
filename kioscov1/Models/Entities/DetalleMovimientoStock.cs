namespace kioscov1.Models.Entities
{
    public class DetalleMovimientoStock
    {
        public int Id { get; set; }
        public int MovimientoStockId { get; set; }
        public int ProductoId {  get; set; }
        public int StockAnterior { get; set; }
        public int StockNuevo { get; set; }

        public Producto? Producto { get; set; }
        public MovimientoStock? MovimientoStock { get; set; }


    }
}