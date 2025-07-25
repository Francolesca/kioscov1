﻿using kioscov1.Models;
using kioscov1.Models.Entities;
using kioscov1.ViewsModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security.Claims;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace kioscov1.Controllers
{
    public class MovimientosStockController : Controller
    {

        private AppDbContext _context;
        public MovimientosStockController(AppDbContext context) {
            _context = context;
         }

        public async Task<IActionResult> Index(DateTime? fechaDesde, DateTime? fechaHasta, string origenFilter)
        {
            var query = _context.MovimientosStock
                .Include(m => m.Detalles)
                .AsQueryable();


            if (fechaDesde.HasValue)
                query = query.Where(m => m.Fecha >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                query = query.Where(m => m.Fecha <= fechaHasta.Value.Date.AddDays(1).AddTicks(-1));

            if (!string.IsNullOrEmpty(origenFilter))
                query = query.Where(m => m.Origen == origenFilter);

            var movimientos = await query
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            var usuariosDict = await _context.Usuarios
                .ToDictionaryAsync(u => u.Id, u => u.Nombre);

            foreach (var m in movimientos)
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
            var origenes = await _context.MovimientosStock
                .Select(m => m.Origen)
                .Distinct()
                .OrderBy(o => o)
                .ToListAsync();

            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
            ViewBag.OrigenFiltro = origenFilter;
            ViewBag.Origenes = new SelectList(origenes, selectedValue: origenFilter);
            return View(movimientos);
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

        public async Task<IActionResult> Exportar(int id)
        {
            var movimiento = await _context.MovimientosStock
                .Include (m => m.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync (m => m.Id == id);

            if(movimiento == null) 
                return NotFound();

            ExcelPackage.License.SetNonCommercialOrganization("<Your Noncommercial Organization>");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("MovimientoStock");

                worksheet.Cells[1, 1].Value = "Fecha";
                worksheet.Cells[1, 2].Value = movimiento.Fecha.ToString("g");
                worksheet.Cells[2, 1].Value = "Usuario";
                worksheet.Cells[2, 2].Value = movimiento.UsuarioId;

                worksheet.Cells[4, 1].Value = "Producto";
                worksheet.Cells[4, 2].Value = "Stock Anterior";
                worksheet.Cells[4, 3].Value = "Stock Nuevo";
                worksheet.Cells[4, 4].Value = "Diferencia";

                worksheet.Row(4).Style.Font.Bold = true;

                int row = 5;

                foreach(var d in movimiento.Detalles)
                {
                    worksheet.Cells[row, 1].Value = d.Producto?.Nombre ?? "N/D";
                    worksheet.Cells[row, 2].Value = d.StockAnterior;
                    worksheet.Cells[row, 3].Value = d.StockNuevo;
                    worksheet.Cells[row, 4].Value = d.StockNuevo - d.StockAnterior;
                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"MovimientoStock_{id}.xlsx");

            }


        }

        public IActionResult CrearMovimientoStock ()
        {
            var productos = _context.Productos.ToList();
            ViewBag.Productos = productos;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CrearMovimientoStock([FromBody] MovimientoStockVM model)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if (model == null || model.DetalleMovimientoStock == null || !model.DetalleMovimientoStock.Any())
            {
                return BadRequest("No se recibieron detalles de movimiento.");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var movimiento = new MovimientoStock
            {
                Fecha = model.MovimientoStock.Fecha,
                Origen = model.MovimientoStock.Origen,
                Comentario = model.MovimientoStock.Comentario,
                UsuarioId = userId,
                Detalles = new List<DetalleMovimientoStock>()
            };

            foreach (var d in model.DetalleMovimientoStock)
            {
                var producto = await _context.Productos.FindAsync(d.ProductoId);
                if (producto != null)
                {
                    movimiento.Detalles.Add(new DetalleMovimientoStock
                    {
                        ProductoId = producto.Id,
                        StockAnterior = producto.Stock,
                        StockNuevo = producto.Stock + d.Cantidad,
                    });
                    producto.Stock += d.Cantidad;
                    _context.Productos.Update(producto);
                }


            }

            _context.MovimientosStock.Add(movimiento);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Ingreso guardado correctamente" });

        }
    }
}
