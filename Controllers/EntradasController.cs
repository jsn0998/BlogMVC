using BlogMVC.Datos;
using BlogMVC.Entidades;
using BlogMVC.Models;
using BlogMVC.Servicios;
using BlogMVC.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ClientModel.Primitives;

namespace BlogMVC.Controllers
{
    public class EntradasController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IServicioChat servicioChat;

        /* Nombre de la carpeta que contendra las imagenes de las publicaciones */
        private readonly string contenedor = "entradas";

        public EntradasController(
            ApplicationDbContext context,
            IAlmacenadorArchivos almacenadorArchivos,
            IServicioUsuarios servicioUsuarios,
            IServicioChat servicioChat
            ) {
            this.context = context;
            this.almacenadorArchivos = almacenadorArchivos;
            this.servicioUsuarios = servicioUsuarios;
            this.servicioChat = servicioChat;
        }

        [HttpGet]
        public IActionResult Crear() {
            return View();
        }

        /* Solo usuarios con los roles de admin y crudEntradas pueden crear una entrada */
        [HttpPost]
        [Authorize(Roles = $"{Constantes.RolAdmin},${Constantes.CRUDEntradas}")]
        public async Task<IActionResult> Crear(EntradaCrearViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            string? portadaUrl = null;
            if (modelo.ImagenPortada is not null)
            {
                /* se obtiene la url del archivo (imagen) cargada */
                portadaUrl = await almacenadorArchivos.Almacenar(contenedor, modelo.ImagenPortada);
            }

            string usuarioId = servicioUsuarios.ObtenerUsuarioId()!;
            var entrada = new Entrada
            {
                Titulo = modelo.Titulo,
                Cuerpo = modelo.Cuerpo,
                FechaPublicacion = DateTime.UtcNow,
                PortadaUrl = portadaUrl,
                UsuarioCreacionId = usuarioId,
            };

            context.Add(entrada);
            await context.SaveChangesAsync();
            return RedirectToAction("Detalle", new { id = entrada.Id});
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var entrada = await context.Entradas
                .IgnoreQueryFilters() /* Para ignorar los filtros que esten en OnModelCreating de la clase ApplicactionDbContext */
                .Include(x => x.UsuarioCreacion)/* Incluir la data del Usuario quien creo la publicacion */
                .Include(x => x.Comentarios)/* Comentarios que se le hizo a la publicacion */
                    .ThenInclude(x => x.Usuario)/* Usuarios que comentaron a la publicacion */
                .FirstOrDefaultAsync(x => x.Id == id);

            /* Si entrada es nula se redirige a la pantalla de noEncontrado */
            if (entrada is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var puedeEditarEntradas = await servicioUsuarios.PuedeUsuarioHacerCRUDEntradas();

            if (entrada.Borrado && !puedeEditarEntradas)
            {
                var urlRetorno = HttpContext.ObtenerUrlRetorno();/* urlRetorno contiene la ultima url */
                return RedirectToAction("Login","Usuarios", new { urlRetorno });
            }

            var puedeBorrarComentarios = await servicioUsuarios.PuedeUsuarioBorrarComentarios();
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var modelo = new EntradaDetalleViewModel
            {
                Id = entrada.Id,
                Titulo = entrada.Titulo,
                Cuerpo = entrada.Cuerpo,
                PortadaUrl = entrada.PortadaUrl,
                FechaPublicacion = entrada.FechaPublicacion,
                EscritoPor = entrada.UsuarioCreacion!.Nombre,
                MostrarBotonEdicion = puedeEditarEntradas,
                EntradaBorrada = entrada.Borrado,
                Comentarios = entrada.Comentarios.Select(x=>new ComentarioViewModel
                {
                    Id = x.Id,
                    Cuerpo = x.Cuerpo,
                    EscritoPor = x.Usuario!.Nombre.ToString(),
                    FechaPublicacion =x.FechaPublicacion,
                    // el usuario logueado puede borrar comantario si este tiene permiso de borrar comentario o si este es el autor de dicho comentario
                    MostrarBotonBorrar = puedeBorrarComentarios || x.UsuarioId == usuarioId,
                })
            };

            return View(modelo);

        }

        [HttpGet]
        [Authorize(Roles =$"{Constantes.RolAdmin},${Constantes.CRUDEntradas}")]
        public async Task<IActionResult> Editar(int id)
        {
            var entrada = await context.Entradas
                .IgnoreQueryFilters() /* Para ignorar los filtros que esten en OnModelCreating de la clase ApplicactionDbContext */
                .FirstOrDefaultAsync(x => x.Id == id);
            if (entrada is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = new EntradaEditarViewModel
            {
                Id = entrada.Id,
                Titulo = entrada.Titulo,
                Cuerpo = entrada.Cuerpo,
                ImagenPortadaActual = entrada.PortadaUrl
            };

            return View(modelo);
        }

        [HttpPost]
        [Authorize(Roles = $"{Constantes.RolAdmin},${Constantes.CRUDEntradas}")]
        public async Task<IActionResult> Editar(EntradaEditarViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var entradaDB = await context.Entradas
                .IgnoreQueryFilters()/* Para ignorar los filtros que esten en OnModelCreating de la clase ApplicactionDbContext */
                .FirstOrDefaultAsync(x=>x.Id== modelo.Id);
            if (entradaDB is null)
            {
                RedirectToAction("NoEncontrado", "Home");
            }

            string? portadaUrl = null;
            if (modelo.ImagenPortada is not null)
            {
                portadaUrl = await almacenadorArchivos.Editar(modelo.ImagenPortadaActual, contenedor, modelo.ImagenPortada);

            }else if (modelo.ImagenRemovida)/* Si el usuario desea remover la imagen actual sin cambiarla por otra*/
            {
                await almacenadorArchivos.Borrar(modelo.ImagenPortadaActual, contenedor);
            }
            else
            {
                portadaUrl = entradaDB.PortadaUrl;
            }

            string usuarioId = servicioUsuarios.ObtenerUsuarioId()!;

            /* En Entity Framework Corp si se tiene una entidad que en base a esta 
               se cargo una tabla se puede simplemente establecer sus propiedades y 
               se actualizara su registro correspondiente en la base de datos 
            */
            entradaDB.Titulo = modelo.Titulo;
            entradaDB.Cuerpo = modelo.Cuerpo;
            entradaDB.PortadaUrl = portadaUrl;
            entradaDB.UsuarioActualizacionId = usuarioId;

            await context.SaveChangesAsync();
            return RedirectToAction("Detalle", new {id = entradaDB.Id});

        }

        [HttpPost]
        [Authorize(Roles = $"{Constantes.RolAdmin},${Constantes.CRUDEntradas}")]
        public async Task<IActionResult> Borrar(int id, bool borrado)
        {
            var entradaDB = await context.Entradas
                .IgnoreQueryFilters() /* Para ignorar los filtros que esten en OnModelCreating de la clase ApplicactionDbContext */
                .FirstOrDefaultAsync(x=>x.Id== id);

            if (entradaDB is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            entradaDB.Borrado = borrado;// Eliminado logico, no es fisico
            await context.SaveChangesAsync();
            return RedirectToAction("Detalle", new {id= entradaDB.Id});
        }


        
        [HttpGet]
        // [Authorize(Roles = $"{Constantes.RolAdmin},${Constantes.CRUDEntradas}")]
        public async Task GenerarCuerpo([FromQuery] string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                await Response.WriteAsync("El título no puede estar vacío");
                return;
            }


            /* Obtencion de la respuesta por segmentos de texto */
            await foreach (var segmento in servicioChat.GenerarCuerpoStream(titulo))
            {
                await Response.WriteAsync(segmento);
                await Response.Body.FlushAsync();/* Envio del segmento de texto al cliente */
            }
        }

        /*
        [HttpGet]
        public async Task<IActionResult> GenerarCuerpo([FromQuery] string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                return BadRequest("El titulo no puede estar vacio");
            }

            var texto = "<p>Este es un artículo de ejemplo</p>";

            // var texto = servicioChat.GenerarCuerpo(titulo);

            return Ok(texto);
        }
        */
    }
}
