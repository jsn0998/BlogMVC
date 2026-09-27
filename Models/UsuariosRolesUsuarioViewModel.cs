namespace BlogMVC.Models
{
    /* 
        Significado del nombre de la clase:
        Usuarios es el controlador 
        RolesUsuario es el nombre de la accion
    */
    public class UsuariosRolesUsuarioViewModel
    {
        public required string UsuarioId { get; set; }
        public required string Email { get; set; }

        public IEnumerable<UsuarioRolViewModel> Roles { get; set; } = [];
    }
}
