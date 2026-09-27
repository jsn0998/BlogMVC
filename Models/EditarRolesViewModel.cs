namespace BlogMVC.Models
{
    public class EditarRolesViewModel
    {
        public required string UsuarioId { get; set; }

        /* Roles que se le asignara al usuario */
        public List<string> RolesSeleccionados { get; set; } = [];
    }
}
