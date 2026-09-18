using BlogMVC.Entidades;
using BlogMVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlogMVC.Controllers
{
    public class UsuariosController: Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;

        /*
            Se inyecta el UserManager como Usuario en el constructor 
            Se inyecta el SignInManager como Usuario en el constructor
        */

        public UsuariosController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult Registro() {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            // Se valida el modelo que se recibe, en caso de qu eno sea valñida se retorna la misma vista con el modelo que no es valido
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            /* Se instancia un nuevo Usuario */
            var usuario = new Usuario()
            {
                Email = modelo.Email,
                UserName = modelo.Email,
                Nombre = modelo.Nombre
            };

            // Se usa el userManager para crear el usuario
            var resultado = await userManager.CreateAsync(usuario, password: modelo.Password);

            // Si el resultado es exitoso dicho usuario creado se logueara de inmediato en el sistema
            if (resultado.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // si ocurre algun error
                // Se usa un foreach para iterar los errores
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(modelo);
            }

        }


        [AllowAnonymous]
        public IActionResult Login(string? message = null)
        {

            if (message is not null)
            {
                ViewData["Message"] = message;
            }

            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            // Se valida que el modelo recibido es valido
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }


            // Se intenta hacer un logue del usuario
            var resultado = await signInManager.PasswordSignInAsync(modelo.Email, modelo.Password, modelo.Recuerdame, lockoutOnFailure: false);

            // si es exitoso se redirijira al usuario a la accion index del controlador Home
            if (resultado.Succeeded)
            {
                return RedirectToAction("Index","Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o password incorrecto");
                return View(modelo);
            }

        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index","Home");
        }

    }
}
