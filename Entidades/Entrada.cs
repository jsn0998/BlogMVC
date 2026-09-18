using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BlogMVC.Entidades
{
    public class Entrada
    {
        public int Id { get; set; }

        [StringLength(250)]
        public required string Titulo { get; set; }
        public required string Cuerpo { get; set; }


        [Unicode(false)]// Restriccion que sirve solo para que se ingrese en el campo sin caracteres especials como: el spacio, la tilde
        public string? PortadaUrl { get; set; }

        public DateTime FechaPublicacion { get; set; }

        [Required]
        public string UsuarioCreacionId { get; set; } = null!;

        public Usuario? UsuarioCreacion { get; set; }

        public string? UsuarioActualizacionId { get; set; }

        public Usuario? UsuarioActualizacion { get; set; }

        public bool Borrado { get; set; }

        public List<Comentario> Comentarios { get; set; } = [];
    }
}

