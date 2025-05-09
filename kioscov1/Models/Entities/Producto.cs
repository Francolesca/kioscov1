namespace kioscov1.Models.Entities
{
    public class Producto
    {
        public int Id { get; set; }
        public int CodigoBarra { get; set; }
        public string Nombre { get; set; }
        public decimal Precio {  get; set; }
        public int Stock { get; set; }
    }
}
