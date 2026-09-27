using BlogMVC.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace BlogMVC.Utilidades
{
    public static class Seeding
    {
        private static List<string> roles = new List<string>()
        {
            Constantes.BorraComentarios,
            Constantes.CRUDEntradas,
            Constantes.RolAdmin,
        };

        /* Implementacion sincrona para el DataSeeding */
        /* El paramaetro booleano indica si se esta utilizando un manejador de almacenamiento pero como no se usara se lo ignora con "_" */
        public static void Aplicar(DbContext context, bool _)
        {
            foreach (var role in roles)
            {
                /* Se utiliza Set porque es un DBContext */
                /* IdentityRole: Clase que representa un rol */
                var rolDB = context.Set<IdentityRole>().FirstOrDefault(x => x.Name == role);

                if (rolDB is null)/* si no existe el rol se lo crea */
                {
                    context.Set<IdentityRole>().Add(new IdentityRole
                    {
                        Name = role,
                        NormalizedName = role.ToUpper()
                    });

                    context.SaveChanges();
                }
            }
        }

        /* Implementacion asincrona para el DataSeeding */
        public static async Task AplicarAsync(DbContext context, bool _, CancellationToken cancellationToken)
        {
            {
                foreach (var role in roles)
                {
                    /* Se utiliza Set porque es un DBContext */
                    /* IdentityRole: Clase que representa un rol */
                    var rolDB = await context.Set<IdentityRole>().FirstOrDefaultAsync(x => x.Name == role);

                    if (rolDB is null)
                    {
                        context.Set<IdentityRole>().Add(new IdentityRole
                        {
                            Name = role,
                            NormalizedName = role
                        });

                        await context.SaveChangesAsync(cancellationToken);
                    }
                }
            }

        }

    }
}
