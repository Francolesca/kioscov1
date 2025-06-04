using kioscov1.Models;
using kioscov1.Models.UserEntity;
using kioscov1.ViewsModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace kioscov1.Controllers
{
    public class AccesoController : Controller
    {
        private readonly AppDbContext _context;
        public AccesoController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public async Task<IActionResult> Login(LoginVM model) 
        {
            Usuario? usuario = await _context.Usuarios
                .Where(u => u.Nombre == model.Usuario)
                .FirstOrDefaultAsync();
            if (usuario == null) 
            {
                ViewData["AlertMessage"] = "Usuario incorrecto.";
                return View();
            }
            if (usuario.Password != model.Password)
            {
                ViewData["AlertMessage"] = "Clave Incorrecta.";
                return View();
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim (ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                properties);

            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Acceso");
        }
    }
}
