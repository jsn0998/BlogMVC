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

        public bool Borrado { get; set; }/* No se eliminar el registro de entrada (publicacion) de la base de datos solo se marcara dicho registro como Borrado */

        public List<Comentario> Comentarios { get; set; } = [];/* A partir de un registro de la tabla entradas es posible obtner su listado de registro de comentarios correspondientes */
    }
}

