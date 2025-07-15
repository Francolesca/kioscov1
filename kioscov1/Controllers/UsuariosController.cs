using kioscov1.Models;
using kioscov1.Models.UserEntity;
using kioscov1.ViewsModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace kioscov1.Controllers
{
    [Authorize(Roles ="Administrador")]
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;
        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios.Include(u => u.Rol).ToListAsync();

            return View(usuarios);
        }
        //GET
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(_context.Roles, "Id", "Nombre");
            return View();
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {

            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Roles = new SelectList(_context.Roles, "Id", "Nombre", usuario.RolId);
            return View();
        }
        public async Task<IActionResult> ListaRoles()
        {
            var roles = await _context.Roles.ToListAsync();

            return View(roles);
        }
        //GET
        public IActionResult CreateRol()
        {
            return View();
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRol(Rol rol)
        {

            if (ModelState.IsValid)
            {
                _context.Add(rol);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View();
        }

        public async Task<IActionResult> ChangePassword(int? id)
        {
            if (id == null) return NotFound();
            var userIdClaim = int.Parse( User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userIdClaim != id) return Forbid();
            
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound(); 

            var model = new ChangePasswordVM { Id = usuario.Id };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var usuario = await _context.Usuarios.FindAsync(model.Id);
            if (usuario == null) return NotFound();

            if (usuario.Password != model.PasswordAct)
            {
                ModelState.AddModelError(string.Empty, "La contraseña actual es incorrecta.");
                return View(model);
            }

            usuario.Password = model.PasswordNew;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Contraseña cambiada correctamente.";
            return RedirectToAction("ChangePassword");
        }
    }
}
