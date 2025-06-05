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
