namespace kioscov1.Models.Entities
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int UsuarioId {  get; set; }
        public decimal Importe { get; set; }
        public int? TurnoCajaId { get; set; }
        public TurnoCaja? TurnoCaja { get; set; }

        public ICollection<DetalleVenta>? Detalles { get; set; }
    }
}
