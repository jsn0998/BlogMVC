using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace BlogMVC.Servicios
{
    public class AlmacenadorArchivosAzure : IAlmacenadorArchivos
    {

        private string connectionString;
        public AlmacenadorArchivosAzure(IConfiguration configuration)
        {
            connectionString = configuration.GetValue<string>("AzureStorageConnection")!;
        }

        public async Task<string> Almacenar(string contenedor, IFormFile archivo)
        {
            var cliente = new BlobContainerClient(connectionString, contenedor);
            await cliente.CreateIfNotExistsAsync();
            cliente.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var extension = Path.GetExtension(archivo.FileName);
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var blob = cliente.GetBlobClient(nombreArchivo);/* A traves del cliente blob se podra cargar la imagen */

            var blobHttpHeaders = new BlobHttpHeaders();/* se creara una cabezera */
            blobHttpHeaders.ContentType = archivo.ContentType;/* se asignara un CotentType a la cabezera */

            await blob.UploadAsync(archivo.OpenReadStream(), blobHttpHeaders);/* se pasa las cabezeras para subir el archivo */

            return blob.Uri.ToString();/* retorna la url en Azure */
        }

        public async Task Borrar(string? ruta, string contenedor)
        {

            if (string.IsNullOrWhiteSpace(ruta))/* si la ruta es nula no retornamos nada */
            {
                return;
            }

            var cliente = new BlobContainerClient(connectionString, contenedor);
            await cliente.CreateIfNotExistsAsync();/* crear si no existe el contenedor */

            var nombreArchivo = Path.GetFileName(ruta);// se obtiene el nombre del archivo
            
            var blob = cliente.GetBlobClient(nombreArchivo);// se obtiene el cliente blob

            await blob.DeleteIfExistsAsync();/* se elimina el archivo si existe */
        }
    }
}
