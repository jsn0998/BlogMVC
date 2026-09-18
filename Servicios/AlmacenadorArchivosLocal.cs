namespace BlogMVC.Servicios
{
    public class AlmacenadorArchivosLocal : IAlmacenadorArchivos
    {
        /* Permite obtener dionde se encuentra mi www.root */
        private readonly IWebHostEnvironment env;

        /* Permite construir la url en la cual se esta ejecutando mi proyecto de envc*/
        private readonly IHttpContextAccessor httpContextAccessor;

        public async Task<string> Almacenar(string contenedor, IFormFile archivo)
        {
            var extension = Path.GetExtension(archivo.FileName);
            /* Crear el nombre del archivo de manera aleatoria */
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";

            /* Folder en el cual se almacenaran los archivos */
            string folder = Path.Combine(env.WebRootPath, contenedor);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string ruta = Path.Combine(folder, nombreArchivo);

            /*
             Se instancia el MemoryStream
             
            */
            using (var ms = new MemoryStream())
            {
                await archivo.CopyToAsync(ms);// se copia el archivo al MemoryStream
                var contenido = ms.ToArray();// convertimos la representacion del MemoryStream en un arreglod de bytes
                await File.WriteAllBytesAsync(ruta, contenido);// con el arreglo de bytes se puede usar File.WriteAllBytesAsync para escribirt el contenido del archivo en esta ruta
            }

            /* Se construye la url en la cual se encuentra nuestro archivo */
            var request = httpContextAccessor.HttpContext!.Request;// Se obtiene el objeto que representa la peticion http

            /* Construccion de la url */
            var url = $"{request.Scheme}://{request.Host}";

            var urlArchivo = Path.Combine(url, contenedor, nombreArchivo).Replace("\\","/");
            return urlArchivo;

        }

        public Task Borrar(string? ruta, string contenedor)
        {
            /* Si la ruta es vacia se retorna que la tarea ha sido completada*/

            if (string.IsNullOrEmpty(ruta))
            {
                return Task.CompletedTask;// se retorna que la tarea ha sido completada
            }

            var nombreArchivo = Path.GetFileName(ruta);

            var directorioArchivo= Path.Combine(env.WebRootPath,contenedor, nombreArchivo);

            /* Si el archivo existe se lo elimina */

            if (File.Exists(directorioArchivo))
            {
                File.Delete(directorioArchivo);
            }

            return Task.CompletedTask;// se retorna que la tarea ha sido completada
        }
    }
}
