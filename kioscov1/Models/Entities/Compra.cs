namespace kioscov1.Models.Entities
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        //public int ProveedorId {get; set;}
        //public Proveedor Proveedor { get; set;}
        public ICollection<DetalleCompra> Detalles { get; set; }

    }
}
