using BlogMVC.Entidades;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BlogMVC.Servicios
{
    public interface IServicioUsuarios
    {
        string? ObtenerUsuarioId();
        Task<bool> PuedeUsuarioBorrarComentarios();
        Task<bool> PuedeUsuarioHacerCRUDEntradas();
    }

    public class ServicioUsuarios : IServicioUsuarios
    {
        private readonly UserManager<Usuario> userManager;
        private readonly HttpContext httpContext;
        private readonly Usuario usuarioActual;
        private static readonly string[] RolesCRUEntradas = { Constantes.RolAdmin, Constantes.CRUDEntradas };
        private static readonly string[] RolesBorrarComentarios = { Constantes.RolAdmin, Constantes.BorraComentarios };

        /* Metodo que permite determinar si el usuario se encuentra en alguno de los roles enviado como parametro*/
        public async Task<bool> UsuarioEstaEnRol(IEnumerable<string> roles)
        {
            /* Obtener los roles del usuario actual */
            var rolesUsuario = await userManager.GetRolesAsync(usuarioActual);

            /* 
               Comparacion con el IEnumerable de roles recibido en la funcion para determinar
               si existe en roles algun elemento que se encuentre en rolesUsuario 

                Si existe en el listado roles algun elemento que se encuentre en rolesUsuario
            */
            return roles.Any(rolesUsuario.Contains);
        }

        public async Task<bool> PuedeUsuarioHacerCRUDEntradas()
        {
            return await UsuarioEstaEnRol(RolesCRUEntradas);
        }

        public async Task<bool> PuedeUsuarioBorrarComentarios()
        {
            return await UsuarioEstaEnRol(RolesBorrarComentarios);
        }

        /* Tipear en el teclado "ctor" y dar "Enter" para obtener el httpContextAccessor y el userManager */
        public ServicioUsuarios(IHttpContextAccessor httpContextAccessor, UserManager<Usuario> userManager)
        {
            this.userManager = userManager;
            httpContext = httpContextAccessor.HttpContext!;
            usuarioActual = new Usuario
            {
                Id = ObtenerUsuarioId()!
            };
        }

        public string? ObtenerUsuarioId()
        {
            /* Los Claims es informacion acerca del usuario */
            var idClaim = httpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();

            if (idClaim is null)
            {
                return null;
            }

            return idClaim.Value;
        }
    }
}
