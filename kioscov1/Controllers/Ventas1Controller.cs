using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kioscov1.Models;
using kioscov1.Models.Entities;
using kioscov1.ViewsModel;

namespace kioscov1.Controllers
{
    public class Ventas1Controller : Controller
    {
        private readonly AppDbContext _context;

        public Ventas1Controller(AppDbContext context)
        {
            _context = context;
        }

        // GET: Ventas1
        public async Task<IActionResult> Index()
        {
            return View(await _context.Ventas.ToListAsync());
        }

        // GET: Ventas1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // GET: Ventas1/Create
        public IActionResult Create()
        {
            var productos = _context.Productos.ToList();
            ViewBag.Productos = productos;

            return View();
        }

        // POST: Ventas1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public  async Task<IActionResult> Create([FromBody] VentasVM model)
        {
            var turno = await _context.TurnosCaja
                .FirstOrDefaultAsync(t => t.UsuarioId == model.Venta.UsuarioId.ToString() && t.Cierre == null);

            if (turno != null)
            {
                model.Venta.TurnoCajaId = turno.Id;
            }

            try
            {
                Venta oVenta = model.Venta;
                oVenta.Detalles = model.Detalles;

                foreach (var d in model.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(d.ProductoId);
                    if (producto != null)
                    {
                        producto.Stock -= d.Cantidad;
                        _context.Productos.Update(producto);
                    }
                }

                _context.Add(oVenta);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Venta guardada correctamente", ventaId = oVenta.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al registrar venta: " + ex.Message);
            }

        }

        // GET: Ventas1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
            {
                return NotFound();
            }
            return View(venta);
        }

        // POST: Ventas1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fecha")] Venta venta)
        {
            if (id != venta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VentaExists(venta.Id))
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
            return View(venta);
        }

        // GET: Ventas1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // POST: Ventas1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venta = await _context.Ventas.FindAsync(id);
            if (venta != null)
            {
                _context.Ventas.Remove(venta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VentaExists(int id)
        {
            return _context.Ventas.Any(e => e.Id == id);
        }
    }
}
