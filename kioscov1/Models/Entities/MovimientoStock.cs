namespace kioscov1.Models.Entities
{
    public class MovimientoStock
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string? UsuarioId { get; set; }
        public List<DetalleMovimientoStock>? Detalles { get; set; }
    }
}
