using kioscov1.Models;
using kioscov1.Models.Entities;
using kioscov1.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace kioscov1.Controllers
{
    public class TurnoCajaController : Controller
    {
        private readonly AppDbContext _context;
        public TurnoCajaController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> AbrirTurno(decimal montoInicial)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("No se pudo identificar el usuario.");
            }

            var turnoAbierto = await _context.TurnosCaja
                .FirstOrDefaultAsync(t => t.UsuarioId == userIdClaim && t.Cierre == null);
            if (turnoAbierto != null)
                return BadRequest("Ya hay un turno abierto.");

            var turno = new TurnoCaja
            {
                UsuarioId = userIdClaim,
                Apertura = DateTime.Now,
                MontoInicial = montoInicial
            };

            _context.TurnosCaja.Add(turno);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> CerrarTurno()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("No se pudo identificar el usuario.");
            }

            var turno = await _context.TurnosCaja
                .Include(t => t.Ventas)
                .FirstOrDefaultAsync(t => t.UsuarioId == userIdClaim && t.Cierre == null);

            if (turno == null) 
                return BadRequest("No hay turno abierto");
            
            turno.Cierre = DateTime.Now;
            turno.MontoTransferencia = turno.Ventas
                .Where(v => v.TipoPago == TipoPago.Transferencia)
                .Sum(v => v.Importe);
            turno.MontoFinal = turno.MontoInicial + turno.Ventas.Sum(v => v.Importe);

            _context.Update(turno);
            await _context.SaveChangesAsync();

            var resTurno = new
            {
                MontoInicial = turno.MontoInicial,
                MontoFinal = turno.MontoFinal,
                MontoTransferencia = turno.MontoTransferencia,
                TotalVentas = turno.Ventas.Count(),
                FechaApertura = turno.Apertura,
                FechaCierre = turno.Cierre
            };
            return Json(resTurno);

        }

        [HttpGet]
        public async Task<IActionResult> EstadoTurno()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("No se pudo identificar el usuario.");

            var tieneTurnoAbierto = await _context.TurnosCaja
                .AnyAsync(t => t.UsuarioId == userIdClaim && t.Cierre == null);

            return Ok(new {tieneTurnoAbierto});
        }

        public async Task<IActionResult> HistorialTurnos(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("No se pudo identificar el usuario.");

            var query =  _context.TurnosCaja.AsQueryable();

            if (fechaDesde.HasValue ) 
                query = query.Where(t => t.Apertura >= fechaDesde.Value.Date);

            if(fechaHasta.HasValue )
                query = query.Where(t => t.Apertura <= fechaHasta.Value.Date.AddDays(1).AddTicks(-1));

            var hTurnos = await query
                .OrderByDescending(t => t.Apertura)
                .ToListAsync();

            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
            return View(hTurnos);
        }

        public async Task<IActionResult>Details(int id)
        {
            var turno = await _context.TurnosCaja
                .Include(t => t.Ventas)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (turno == null)
                return NotFound();
            return View(turno);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
