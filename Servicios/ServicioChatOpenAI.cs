using BlogMVC.Configuraciones;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

namespace BlogMVC.Servicios
{
    public class ServicioChatOpenAI : IServicioChat
    {
        private readonly IOptions<ConfiguracionesIA> options;
        private readonly OpenAIClient openAIClient;

        /* 
            Mensajes del sistema: Se usan para indicar a la inteligencia artifical las espectativas que se tiene de ella, 
            esto se hace a traves de un mensaje 
            que es como una conficuracion inicial que asignara todas las respuestas subsiguientes que se recibiran 
            Texto que se asigna como mensaje de tipo sistema a la inteligencia artificial 
        */
        private string systemPromptGenerarCuerpo = """
            Eres un ingeniero se software en ASP .NET Core.
            Escribes articulos con un tono jovial y amigable
            Te esfuerzas para que los principiantes entiendan las cosas dando ejemplos prácticos ...
            """;


        /* Mensajes del usuarios: Peticion que el usuario de la IA enviara para generar una respuesta */
        private string ObtenerPromptGenerarCuerpo(string titulo) => $"""
            Crear un artículo para un blog. El título del articulo será {titulo}
            Si lo entiendes conveniente, debes insertar tips.

            El formato de respuesta es HTML, por lo tanto debes de colocar negrita donde consideres, 
            títulos, subtítulos, entre otras cosas que ayuden a resaltar el formato.

            La respuesta no debe de ser un documento HTML, sino solamente el artículo en formato HTML,
            con sus párrafos bien separados. Por tanto, nada de DOCTYPE, ni head, ni body. Solo el artículo.

            No incluyas el título del artículo en el artículo.
            """;


        /* OpenAIClient es el cliente para poder consumir los permisos de OpenAI */
        public ServicioChatOpenAI(IOptions<ConfiguracionesIA> options, OpenAIClient openAIClient)
        {
            this.options = options;
            this.openAIClient = openAIClient;
        }


        public async Task<string> GenerarCuerpo(string titulo)
        {
            var modeloTexto = options.Value.ModeloTexto;// Se busca el modelo
            var clienteChat = openAIClient.GetChatClient(modeloTexto);// Se instancia el cliente de chat
            /*
                Mensajes del sistema: Se usan para indicar a la inteligencia artifical las espectativas que se tiene de ella, esto se hace a traves de un mensaje que es como una conficuracion inicial que asignara todas las respuestas subsiguientes que se recibiran
                Mensajes del usuarios: Peticion que el usuario de la IA enviara para generar una respuesta
                Mensajes del asistente:
            */
            var mensajeDeSistema = new SystemChatMessage(systemPromptGenerarCuerpo);

            var promptUsuario = ObtenerPromptGenerarCuerpo(titulo);
            var mensajeUsuario = new UserChatMessage(promptUsuario);

            /* Creacion de los mensajes para la IA */
            ChatMessage[] mensajes = { mensajeDeSistema, mensajeUsuario };

            /* Configuracion que recibe un arreglo de mensajes */
            var respuesta = await clienteChat.CompleteChatAsync(mensajes);

            /* Se obtiene el texto de la IA*/
            var cuerpo = respuesta.Value.Content[0].Text.Trim();

            return cuerpo;
        }

        /*
         * Cuando se usa IAsymcEnaumerable es una manera de generar mediante streamming 
         * y de manera asíncron ala respuesta de este método que va a ser pequeños segemenots de texto que se mostraran 
         * a medida qu elo genere la IA
        */
        public async IAsyncEnumerable<string> GenerarCuerpoStream(string titulo)
        {
            var modeloTexto = options.Value.ModeloTexto;// Se busca el modelo
            var clienteChat = openAIClient.GetChatClient(modeloTexto);// Se instancia el cliente de chat

            /* Creacion del mensaje del sistema */
            var mensajeDeSistema = new SystemChatMessage(systemPromptGenerarCuerpo);

            /* Creacion del prompt del usuario */
            var promptUsuario = ObtenerPromptGenerarCuerpo(titulo);
            var mensajeUsuario = new UserChatMessage(promptUsuario);

            /* Creacion de los mensajes para la IA */
            ChatMessage[] mensajes = { mensajeDeSistema, mensajeUsuario };

            /* Obtencion del mensaje de la IA por pedazos */
            await foreach (var completionUpdate in clienteChat.CompleteChatStreamingAsync(mensajes))
            {
                /* Se obtiene un conjunto de actualizaciones */
                foreach (var contenido in completionUpdate.ContentUpdate)
                {
                    yield return contenido.Text;
                }
            }
        }

    }
}
