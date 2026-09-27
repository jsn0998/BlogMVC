using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BlogMVC.Models
{
    public class EntradaCrearViewModel 
    {
        [Required(ErrorMessage ="El {0} es requerido")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "El {0} es requerido")]
        public string Cuerpo { get; set; }

        [DisplayName("Imagen Portada")]
        public IFormFile? ImagenPortada { get; set; }
    }
}
