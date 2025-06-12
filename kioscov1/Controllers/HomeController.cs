using System.Diagnostics;
using System.Security.Claims;
using kioscov1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace kioscov1.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        public HomeController (AppDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;

        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ObtenerDatos()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var today = DateTime.Today;
            var totalVentas = await _context.Ventas
                .Where(v => v.Fecha.Date == today)
                .SumAsync(v => (decimal?)v.Importe)  ?? 0;

            var topProducts = await _context.DetallesVenta
                .Include(d => d.Producto)
                .Include(d => d.Venta)
                .Where(d => d.Venta.Fecha.Date == today)
                .GroupBy(d => new { d.ProductoId, d.Producto.Nombre })
                .Select(g => new
                {
                    nombre = g.Key.Nombre,
                    cantidadVenta = g.Sum(d => d.Cantidad),
                })
                .OrderByDescending(p => p.cantidadVenta)
                .Take(10)
                .ToListAsync();

            var data = new
            {
                totalVentas,
                topProducts
            };
            return Json(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
