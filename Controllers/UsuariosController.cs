using BlogMVC.Datos;
using BlogMVC.Entidades;
using BlogMVC.Models;
using BlogMVC.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlogMVC.Controllers
{
    public class UsuariosController: Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly ApplicationDbContext context;

        /*
            Se inyecta el UserManager como Usuario en el constructor 
            Se inyecta el SignInManager como Usuario en el constructor
        */

        public UsuariosController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
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

            // Se usa el userManager para crear el usuario, se le pasa el usuario y el password
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
                    ModelState.AddModelError(string.Empty, error.Description);// se agrega los errores al ModelState
                }
                // retorno a la vista con los errores de Identity
                return View(modelo);
            }

        }


        [AllowAnonymous]
        public IActionResult Login(string? message = null, string? urlRetorno = null)
        {

            if (message is not null)
            {
                ViewData["Message"] = message;
            }

            if (urlRetorno is not null)
            {
                ViewData["urlRetorno"] = urlRetorno;
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

            // si el resultado de login fue exitoso
            if (resultado.Succeeded)
            {
                
                if (string.IsNullOrWhiteSpace(modelo.UrlRetorno))
                {// se redirijira al usuario a la accion index del controlador Home
                    return RedirectToAction("Index", "Home");
                }
                else
                {// si existe una url de retorno se devuelve al usuario a la url de retorno
                    return LocalRedirect(modelo.UrlRetorno);
                }
                
             
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
           
        [HttpGet]
        // [Authorize(Roles =Constantes.RolAdmin)]/* solo administradores pueden ver el lsitado de usuarios de la aplicacion */
        public async Task<IActionResult> Listado(string? mensaje = null)
        {
            var usuarios = await context.Users.Select(x => new UsuarioViewModel
            { // convierte la seleccion de los registros de la tabla Users a tipo de dato UsuarioViewModel
                Id = x.Id,
                Email = x.Email!// se indica que Email no es null con !
            }).ToListAsync();

            var modelo = new UsuariosListadoViewModel();
            modelo.Usuarios = usuarios;
            modelo.Mensaje = mensaje;
            return View(modelo);
        }

        [HttpGet]
        // [Authorize(Roles =Constantes.RolAdmin)]/* solo administradores pueden ver el lsitado de usuarios de la aplicacion */
        public async Task<IActionResult> RolesUsuario(string usuarioId)
        {
            /* Buscar el usuario por su id */
            var usuario = await userManager.FindByIdAsync(usuarioId);

            /* si el usuario no existe se redirije al usuaro a la pantalla de NoEncontrado */
            if (usuario is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var rolesQueElUsuarioTiene = await userManager.GetRolesAsync(usuario);/* Se obtiene los roles que el usuario tiene ahora */
            var rolesExistentes = await context.Roles.ToListAsync();

            var rolesDelUsuario = rolesExistentes.Select(x => new UsuarioRolViewModel {  
                Nombre = x.Name!,
                LoTiene = rolesQueElUsuarioTiene.Contains(x.Name!)// si lo tiene entonces LoTiene tendra el valor de true sino tendra el valor de false
            }).ToList();

            var modelo = new UsuariosRolesUsuarioViewModel { 
                UsuarioId = usuarioId,
                Email = usuario.Email!,
                Roles = rolesDelUsuario.OrderBy(x=>x.Nombre)
            };

            return View(modelo);
        }

        [HttpPost]
        // [Authorize(Roles =Constantes.RolAdmin)]/* solo administradores pueden ver el lsitado de usuarios de la aplicacion */
        public async Task<IActionResult> EditarRoles(EditarRolesViewModel modelo)
        {
            var usuario = await userManager.FindByIdAsync(modelo.UsuarioId);
            /* si el usuario no existe se redirije al usuaro a la pantalla de NoEncontrado */
            if (usuario is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            /* Por un momento se le quitara todos los roles al usuario */
            await context.UserRoles.Where(x=>x.UserId == usuario.Id).ExecuteDeleteAsync();
            await userManager.AddToRolesAsync(usuario, modelo.RolesSeleccionados);

            var mensaje = $"Los roles de {usuario.Email} han sido actualizados";
            return RedirectToAction("Listado", new {mensaje});

        }

    }
}
