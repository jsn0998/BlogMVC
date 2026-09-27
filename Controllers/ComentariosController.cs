using BlogMVC.Datos;
using BlogMVC.Entidades;
using BlogMVC.Models;
using BlogMVC.Servicios;
using BlogMVC.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogMVC.Controllers
{
    public class ComentariosController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IServicioUsuarios servicioUsuarios;
        
        public ComentariosController(ApplicationDbContext context, IServicioUsuarios servicioUsuarios)
        {
            this.context = context;
            this.servicioUsuarios = servicioUsuarios;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Comentar(EntradasComentarViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("detalle", "entradas", new { id = modelo.Id });
            }

            /* Devuelve true si la entrada existe de lo conteario devuelve false */
            var existeEntrada = await context.Entradas.AnyAsync(x=>x.Id == modelo.Id);

            if (!existeEntrada)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var usuarioId = servicioUsuarios.ObtenerUsuarioId()!;

            var comentario = new Comentario
            {
                EntradaId = modelo.Id,
                Cuerpo = modelo.Cuerpo,
                UsuarioId = usuarioId,
                FechaPublicacion = DateTime.UtcNow
            };

            context.Add(comentario);
            await context.SaveChangesAsync();
            return RedirectToAction("detalle", "entradas", new { id = modelo.Id });

        }

        [HttpGet]
        [Authorize]/* Se valida que el usuario esta logueado */
        public async Task<IActionResult> Borrar(int id)
        {
            var comentario = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);
            if (comentario is null)
            {
                RedirectToAction("NoEncontrado", "Home");
            }

            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var puedeBorarrCaualquierComentario = await servicioUsuarios.PuedeUsuarioBorrarComentarios();

            if (usuarioId != comentario.UsuarioId  & !puedeBorarrCaualquierComentario)
            {
                var urlRetorno = HttpContext.ObtenerUrlRetorno();
                return RedirectToAction("login","usuarios", new {urlRetorno = urlRetorno });
            }

            return View(comentario);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BorrarComentario(int id)
        {
            var comentario = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);
            if (comentario is null)
            {
                RedirectToAction("NoEncontrado", "Home");
            }

            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var puedeBorarrCaualquierComentario = await servicioUsuarios.PuedeUsuarioBorrarComentarios();

            if (usuarioId != comentario.UsuarioId & !puedeBorarrCaualquierComentario)
            {
                var urlRetorno = HttpContext.ObtenerUrlRetorno();
                return RedirectToAction("login", "usuarios", new { urlRetorno = urlRetorno });
            }

            comentario.Borrado = true;
            await context.SaveChangesAsync();

            return RedirectToAction("detalle","entradas",new {id = comentario.EntradaId});

        }
    }
}
