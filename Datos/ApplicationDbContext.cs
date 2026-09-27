using BlogMVC.Entidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogMVC.Datos
{
    /* Usuario es el tipo de dato que genera un usuario */
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }


        protected ApplicationDbContext()
        {
        }

        /* Se creara una tabla a partir de la clase Entrada */
        public DbSet<Entrada> Entradas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Comentario>()
                .HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Entrada>()
                .HasOne(e => e.UsuarioCreacion)
                .WithMany()
                .HasForeignKey(e => e.UsuarioCreacionId)
                .OnDelete(DeleteBehavior.NoAction);

            // Se aplicara este filtro cada vez que se haga un query en la entidad Comentario y estoy diciendo que ma traiga los comentarios que no han sido borrados*/
            builder.Entity<Comentario>().HasQueryFilter(x => !x.Borrado);

            // Obtener entradas que no han sido eliminadas (eliminado logico, no fisico) */
            builder.Entity<Entrada>().HasQueryFilter(x => !x.Borrado);
        }



        /* Se creara una tabla a partir de la clase Comentario */
        public DbSet<Comentario> Comentarios { get; set; }

    }
}
