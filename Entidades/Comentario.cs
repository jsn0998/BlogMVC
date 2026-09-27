using System.ComponentModel.DataAnnotations;

namespace BlogMVC.Entidades
{
    public class Comentario
    {
        public int Id { get; set; }

        public int EntradaId { get; set; }

        public Entrada? Entrada { get; set; }

        [Required]
        public string Cuerpo { get; set; } = string.Empty;

        public DateTime FechaPublicacion { get; set; }

        public string? UsuarioId { get; set; }// se estblecio UsuarioId como opcional porque si el usuario es eliminado pues se estableceria UsuarioId = null
         
        public Usuario? Usuario { get; set; }/* Al aplicar el signo ? a Usuario, si el usuario es eliminado de la base de datos, No se eliminara su correspondiente comentario */

        public bool Borrado { get; set; }/* No se eliminara el registro de comentario de la base de datos solo se marcara dicho registro como Borrado */
    }
}
