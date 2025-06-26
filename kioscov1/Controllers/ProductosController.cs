using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kioscov1.Models;
using kioscov1.Models.Entities;
using System.Security.Claims;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;

namespace kioscov1.Controllers
{
    public class ProductosController : Controller
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult ControlStock() 
        {
            var productos = _context.Productos.ToList();
            return View(productos);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarStock([FromBody] List<ProductoStockUpdate> productosModificados)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("No se pudo identificar el usuario.");
            }

            var movimiento = new MovimientoStock
            {
                Fecha = DateTime.Now,
                UsuarioId = userIdClaim,
                Origen = "ControlStock",
                Detalles = new List<DetalleMovimientoStock>()
            };

            foreach (var p in productosModificados)
            {
                var producto = await _context.Productos.FindAsync(p.Id);

                if (producto != null)
                {
                    movimiento.Detalles.Add(new DetalleMovimientoStock
                    {
                        ProductoId = p.Id,
                        StockAnterior = producto.Stock,
                        StockNuevo = p.Stock,
                    });
                    producto.Stock = p.Stock;
                    _context.Update(producto);
                }
            }
            _context.MovimientosStock.Add(movimiento);
            await _context.SaveChangesAsync();
            return Ok();
        }

        public class ProductoStockUpdate
        {
            public int Id { get; set; }
            public int Stock { get; set; }
        }


        // GET: Productos
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["SortOrder"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc": "";
            ViewData["StockSortParm"] = sortOrder == "Stock" ? "stock_desc" : "Stock";
            ViewData["CurrentFilter"] = searchString;
            if(searchString != null)
                pageNumber = 1;
            else searchString = currentFilter;
            var product = from l in _context.Productos select l;
            if (!string.IsNullOrEmpty(searchString)) 
                product = product.Where(s => s.Nombre.Contains(searchString));
            int pageSize = 10;
            product = sortOrder switch
            {
                "name_desc" => product.OrderByDescending(s => s.Nombre),
                "Stock" => product.OrderBy(s => s.Stock),
                "stock_desc" => product.OrderByDescending(s => s.Stock),
                _ => product.OrderBy(s => s.Nombre),
            };
            return View(await Paginacion<Producto>.CreateAsync(product.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CodigoBarra,Nombre,Precio,Stock")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = User.FindFirst(ClaimTypes.Role)?.Value;
            if (id == null)
            {
                return NotFound();
            }
            
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CodigoBarra,Nombre,Precio,Stock")] Producto producto)
        {

            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        //GET
        [HttpGet]
        public IActionResult Importar()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Importar(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0) {
                ViewBag.Mensaje = "Seleccione un archivo valido.";
                 return View();
            }
            var productos = new List<Producto>();
            ExcelPackage.License.SetNonCommercialOrganization("<Your Noncommercial Organization>");
            using (var stream = new MemoryStream())
            {
                await archivo.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream)) {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int i = 2; i < rowCount; i++)
                    {
                        var codigo = worksheet.Cells[i, 1].Text;
                        var nombre = worksheet.Cells[i, 2].Text;
                        var precio = decimal.TryParse(worksheet.Cells[i,3].Text , out var p) ? p : 0;

                        if (!string.IsNullOrEmpty(nombre))
                        {
                            productos.Add(
                                new Producto
                                {
                                    CodigoBarra = codigo,
                                    Nombre = nombre,
                                    Precio = precio,
                                    Stock = 0
                                });
                        }
                    }
                }
            }
            _context.Productos.AddRange(productos);
            await _context.SaveChangesAsync();

            ViewBag.Mensaje = $"{productos.Count} productos importados correctamente.";
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Exportar()
        {
            var productos = await _context.Productos.ToListAsync();
            ExcelPackage.License.SetNonCommercialOrganization("<Your Noncommercial Organization>");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Productos");
                worksheet.Cells[1, 1].Value = "Código";
                worksheet.Cells[1, 2].Value = "Nombre";
                worksheet.Cells[1, 3].Value = "Precio";

                using (var range = worksheet.Cells[1, 1, 1, 3])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }
                int row = 2;
                foreach (var p in productos)
                {
                    worksheet.Cells[row, 1].Value = p.CodigoBarra;
                    worksheet.Cells[row, 2].Value = p.Nombre;
                    worksheet.Cells[row, 3].Value = p.Precio;
                    row++;
                }
                worksheet.Cells.AutoFitColumns();
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string nombreArchivo = $"Productos_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
            }
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }

    }
}
