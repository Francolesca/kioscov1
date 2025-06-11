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
                .SumAsync(v => (decimal?)v.Importe)  ?? 520;
            var totalProductos = await _context.Productos.CountAsync();
            var data = new
            {
                totalVentas,
                totalProductos,
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
