using kioscov1.Models.Entities;
using kioscov1.Models.UserEntity;

using Microsoft.EntityFrameworkCore;

namespace kioscov1.Models
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }
        public DbSet<MovimientoStock> MovimientosStock{ get; set; }
        public DbSet<DetalleMovimientoStock> DetallesMovimientosStock { get; set; }
        public DbSet<TurnoCaja> TurnosCaja { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(a => a.Venta)
                .WithMany(v => v.Detalles)
                .HasForeignKey(a => a.VentaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetalleMovimientoStock>()
                .HasOne(a => a.MovimientoStock)
                .WithMany(v => v.Detalles)
                .HasForeignKey(a => a.MovimientoStockId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TurnoCaja>()
                .HasMany(t => t.Ventas)
                .WithOne(v => v.TurnoCaja)
                .HasForeignKey(v => v.TurnoCajaId);
        }
        
    }
}
