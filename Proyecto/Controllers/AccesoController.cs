using Microsoft.AspNetCore.Mvc;

using Proyecto.Data;
using Proyecto.Models;
using Microsoft.EntityFrameworkCore;
using Proyecto.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace Proyecto.Controllers
{
    
    public class AccesoController : Controller
    {
        private readonly AppDBContext _appDbContext;
        public AccesoController(AppDBContext appDBContext)
        {
            _appDbContext = appDBContext;
        }


        [HttpGet]
        public IActionResult Registrarse()
        {
            return View();
        }
        //En este apartado tendremos las validaciones correspondientes para crear un usuario
        [HttpPost]
        public async Task<IActionResult> Registrarse(UsuarioVM modelo)
        {
            // Validar si las contraseñas coinciden
            if (modelo.clave != modelo.ConfirmarClave)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            // Verificar si el correo ya existe
            bool correoExistente = await _appDbContext.Usuarios
                .AnyAsync(u => u.Correo == modelo.Correo);

            if (correoExistente)
            {
                ViewData["Mensaje"] = "El correo ya está registrado";
                return View();
            }

            // Crear el nuevo usuario
            Usuario usuario = new Usuario()
            {
                NombreCompleto = modelo.NombreCompleto,
                Correo = modelo.Correo,
                clave = modelo.clave
            };

            await _appDbContext.Usuarios.AddAsync(usuario);
            await _appDbContext.SaveChangesAsync();

            // Verificar si el usuario fue creado
            if (usuario.IdUsuario != 0)
                return RedirectToAction("Login", "Acceso");

            ViewData["Mensaje"] = "Usuario no creado";
            return View();
        }


        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index","Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login (LoginVM modelo)
        {
            Usuario? usuario_encontrado = await _appDbContext.Usuarios
                                            .Where(u=>
                                             u.Correo == modelo.Correo&&
                                             u.clave == modelo.clave)
                                            .FirstOrDefaultAsync(); 
            

            if(usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "No se encontro el usuario";
                return View();
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario_encontrado.NombreCompleto)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };
            //Aca guardamos la informacion del usuario dentro de la app de cookies 
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
                );
            


           return RedirectToAction("Index", "Home");
        }

    }
}
