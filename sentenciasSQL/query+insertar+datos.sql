DECLARE @usuarioCreacionId NVARCHAR(450);
DECLARE @contador INT = 1;
DECLARE @titulo NVARCHAR(255);
DECLARE @cuerpo NVARCHAR(MAX);

SELECT @usuarioCreacionId = Id FROM AspNetUsers;

WHILE @contador <= 100
BEGIN
	 SET @titulo = 'Entrada ' + CAST(@contador AS NVARCHAR(10));
     SET @cuerpo = '[{"insert":"Este es el cuerpo de la entrada ' + CAST(@contador AS NVARCHAR(10)) + ' \n"}]';

     INSERT INTO [dbo].[Entradas] ([Titulo], [Cuerpo], [FechaPublicacion], [UsuarioCreacionId], [Borrado])
     VALUES (@titulo, @cuerpo, DATEADD(d, @contador, GETDATE()), @usuarioCreacionId, 'false');

     SET @contador = @contador + 1;
END
