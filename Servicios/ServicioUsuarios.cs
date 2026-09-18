using BlogMVC.Entidades;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BlogMVC.Servicios
{
    public interface IServicioUsuarios
    {
        string? ObtenerUsuarioId();
    }

    public class ServicioUsuarios : IServicioUsuarios
    {
        private readonly UserManager<Usuario> userManager;
        private readonly HttpContext httpContext;

        /* tipear en el teclado "ctor" y dar "Enter" para obtener el httpContextAccessor y el userManager */
        public ServicioUsuarios(IHttpContextAccessor httpContextAccessor, UserManager<Usuario> userManager)
        {
            this.userManager = userManager;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public string? ObtenerUsuarioId()
        {
            var idClaim = httpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();

            if (idClaim is null)
            {
                return null;
            }

            return idClaim.Value;
        }
    }
}
