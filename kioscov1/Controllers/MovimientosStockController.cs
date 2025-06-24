using kioscov1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

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

    }
}
