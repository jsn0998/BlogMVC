using BlogMVC.Configuraciones;
using Microsoft.Extensions.Options;
using OpenAI;

namespace BlogMVC.Servicios
{
    public class ServicioChatOpenAI
    {
        private readonly IOptions<ConfiguracionesIA> options;
        private readonly OpenAIClient openAIClient;

        /* OpenAIClient es el cliente para poder consumir los permisos de OpenAI */
        public ServicioChatOpenAI(IOptions<ConfiguracionesIA> options, OpenAIClient openAIClient) {
            this.options = options;
            this.openAIClient = openAIClient;
        }

        /*
        public async Task<string> GenerarCuerpo(string cuerpo)
        {
            var modeloTexto = options.Value.ModeloTexto;
        }
        */
    }
}
