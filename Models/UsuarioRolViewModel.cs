namespace BlogMVC.Models
{
    public class UsuarioRolViewModel
    {

        public required string Nombre { get; set; }
        public bool LoTiene { get; set; } /* Porque una persona puede tener mas de un rol */
    }
}
