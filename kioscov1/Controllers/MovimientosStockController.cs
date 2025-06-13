using kioscov1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace kioscov1.Controllers
{
    public class MovimientosStockController : Controller
    {

        private AppDbContext _context;
        public MovimientosStockController(AppDbContext context) {
            _context = context;
         }

        public async Task<IActionResult> Index()
        {
            var Movimientos = await _context.MovimientosStock
                .Include(m => m.Detalles)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
            var usuariosDict = await _context.Usuarios
                .ToDictionaryAsync(u => u.Id, u => u.Nombre);

            foreach (var m in Movimientos)
            {
                if (int.TryParse(m.UsuarioId, out int userId) &&
                        usuariosDict.TryGetValue(userId, out var nombre))   
                {
                    m.UsuarioId = nombre;
                }
                else
                {
                    m.UsuarioId = "Desconocido";
                }
            }
            return View(Movimientos);
        }

        public async Task<IActionResult> Details (int id)
        {
            var Movimiento = await _context.MovimientosStock
                .Include(m => m.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            var usuariosDict = await _context.Usuarios
                .ToDictionaryAsync(u => u.Id, u => u.Nombre);

            if (int.TryParse(Movimiento.UsuarioId, out int userId) &&
                    usuariosDict.TryGetValue(userId, out var nombre))
            {
                Movimiento.UsuarioId = nombre;
            }
            else
            {
                Movimiento.UsuarioId = "Desconocido";
            }
            return View(Movimiento);
        }

    }
}
