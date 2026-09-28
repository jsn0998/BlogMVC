Creadenciales para acceder a la aplicacion:
Email: email1@gmail.com
contraseña: @ngulAr0999

Email: email2@gmail.com
contraseña: @ngulAr0999

Email: email3@gmail.com
contraseña: @ngulAr0999

Caracteristicas del proyecto:
Cada entrada del blog podrá tener una imagen de portada, la cual será subida a través de un formulario.
Para ver el listado de entradas utilizaremos la técnica de Infinite Scrolling que es como hacen en redes sociales para que puedas scrollear sin límites.
Se tendra un sistema de usuarios para que las personas puedan registrarse y dejar sus comentarios.
Utilizaremos un sistema de roles para poder asignarle dinámicamente permisos a nuestros usuarios para que puedan editar entradas, borrar comentarios o ser administradores.
La parte de la inteligencia artificial la estaremos dejando para el siguiente módulo.

Roles de la aplicacion: admin, crud-entradas y borrar-comentarios
admin: Todos los permisos 
CRUDEntradas: Personas que tendras permiso para crear, actualizar y borrar entradas 
BorraComentarios: Personas con permiso para borrar comentario

Comandos para usar: Consola del administrador de paquetes
Add-Migration SistemaDeUsarios: Agregar las tablas que de establecieron en ApplicationDbContext hasta ese momento en un archivo de migracion denominado SistemaDeUsarios
Add-Migration TablasEntradasYComentarios: Agregar las tablas que de establecieron en ApplicationDbContext hasta ese momento en un archivo de migracion denominado TablasEntradasYComentarios
Update-Database


Add-Migration TablasEntradasYComentarios: Se crean las tablas en base a los modelos creado y a los DbSet establecidos en la clase ApplicationDbContext del proyecto (CTRL+ ,)
Update-Database


Remove-Migration: Remover la mas reciente migracion (Add-Migration)

Se uso DataSeeding para crear los roles en la base de datos /Servicios/Constantes
DataSeeding es una tecnica apra insertar datos en la basede datos en ciertos momentos especiales

Texto enriquecido - Quilljs (video 294)

Credenciales para OpenAI Platform
Email: jsn0998@gmail.com
contraseña: Re@ct0999
https://platform.openai.com/home

Para utilizar la IA de OpenIA se debe de crear un archivo denominado secret.json
con el siguiente contenido:
{
  "ConfiguracionesIA": {
    "modeloTexto": "gpt-4o-mini",
    "modeloImagenes": "dall-e-3",
    "modeloSentimientos": "gpt-4o-mini",
     "llaveOpenAI": "clave"
  }
}

