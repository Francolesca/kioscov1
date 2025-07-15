using kioscov1.Models.UserEntity;

namespace kioscov1.Models.Entities
{
    public class TurnoCaja
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public DateTime Apertura { get; set; }
        public DateTime? Cierre { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal? MontoTransferencia { get; set; }
        public decimal? MontoFinal{ get; set; }
        public List<Venta> Ventas { get; set; }
    }
}
