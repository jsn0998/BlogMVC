namespace BlogMVC.Utilidades
{
    public static class HttpContextExtensions
    {
        public static string ObtenerUrlRetorno(this HttpContext httpContext)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(httpContext));
            return httpContext.Request.Path + httpContext.Request.QueryString;
        }
    }
}
