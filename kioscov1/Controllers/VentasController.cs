using kioscov1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kioscov1.Models.Entities;
using kioscov1.ViewsModel;

namespace kioscov1.Controllers
{
    public class VentasController : Controller
    {
        private readonly AppDbContext _context;
        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .ToListAsync();

            return View(ventas);
        }
        //HttpGet
        public IActionResult Create()
        {
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id","CodigoBarra","Nombre");
            //var viewModel = new VentasVM
            //{
            //    Productos = _context.Productos.ToList()
            //};
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Venta venta)
        {
            //if (!ModelState.IsValid || ventasVM.Detalles.Count == 0)
            //{
            //    ventasVM.Productos = _context.Productos.ToList();
            //    return View();
            //}

            //var venta = new Venta
            //{
            //    Fecha = DateTime.Now,
            //    Detalles = new List<DetalleVenta>()
            //};

            
            //foreach (var detalle in ventasVM.Detalles)
            //{
            //    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
            //    //if (producto == null || producto.Stock < detalle.Cantidad)
            //    //{
            //    //    ModelState.AddModelError("", $"Stock insuficiente para {producto?.Nombre ?? "producto desconocido"}.");
            //    //    ViewBag.Productos = _context.Productos;
            //    //    return View(ventasVM);
            //    //}

            //    producto.Stock -= detalle.Cantidad;

            //    venta.Detalles.Add(new DetalleVenta
            //    {
            //        ProductoId = producto.Id,
            //        Cantidad = detalle.Cantidad,
            //        PrecioUnitario = detalle.PrecioUnitario,
            //        PrecioTotal = detalle.PrecioTotal * detalle.Cantidad,
            //    });

            //}

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }
    }
}
