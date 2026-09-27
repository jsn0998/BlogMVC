/* 
    dotNetHelper: Objeto  que permite ejecutar codigo de csharp en javascript 
*/

window.observarElemento = async (idElemento, dotNetHelper) => {
    // un observador permite reaccionar a lo que pasa a un conjunto de elementos html
    // entradas: Elementos html que se esta observando
    /* IntersetionObserver: Es un observador de interseccion: Pemrite ejecutar una funcionalidad cuando el elemento entre en la pantalla del usuario */
    let elemento = document.getElementById(idElemento);
    if (!elemento) {
        return;
    }

    let observador = new IntersectionObserver((entradas) => {
        if (entradas[0].isIntersecting) {// del arreglo del listado de elementos, si el elemento 0 esta en pantalla
            // invocar metodo de manera asincrona
            dotNetHelper.invokeMethodAsync("CargarMasElementos");
        }

    });

    observador.observe(elemento);
}