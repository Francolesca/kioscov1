﻿namespace kioscov1.Models.Entities
{
    public class DetalleCompra
    {
        public int Id { get; set; }
        public int CompraId {  get; set; }
        public Compra Compra { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }

        public decimal Precio { get; set; }

    }
}
