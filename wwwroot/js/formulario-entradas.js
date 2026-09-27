const quill = new Quill('#editor', {
    modules: {
        toolbar: [
            [{ header: [1, 2, false] }],
            ['bold', 'italic', 'underline'],
            [ 'code-block'],
        ],
    },
    placeholder: 'Coloque aqui la entrada ...',
    theme: 'snow', // or 'bubble'
});

function cargarContenido(contenido) {
    /* Establecer contenido al editor */
    quill.setContents(contenido,'silent');
}

function btnEnviarClick() {
    let esValido = validarFormularioCompleto();

    if (!esValido) {      
        return;
    }
    /* Obtener el contenido del editor */
    const delta = quill.getContents();

    /* Conversion del html a cadena string json */
    const deltaJSON = JSON.stringify(delta.ops);

    $("#Cuerpo").val(deltaJSON);
    $("#formEntrada").trigger("submit");
}

function validarFormularioCompleto() {
    let formularioEntradaEsValido = $("#formEntrada").valid();
    let cuerpoEsValido = validarCuerpo();
    return formularioEntradaEsValido && cuerpoEsValido;
}

function validarCuerpo() {
    let mensajeDeError = null;
    let esValido = true;
    /* Se obtiene el html que representa el cuerpo */
    const htmlCuerpo = quill.getSemanticHTML();

    /* Validacion si el cuerpo tiene o no contenido (segun documentacion https://quilljs.com/docs/quickstart )*/
    if (htmlCuerpo === "<p></p>") {
        mensajeDeError = "El cuerpo es requerido";
        esValido = false;
    }

    $("#cuerpo-error").html(mensajeDeError);
    return esValido;
}

/* Evento de quill que se ejecuta cada vez que el usuario escriba en el cuerpo */
quill.on('text-change', function(delta, oldDelta, source){
    validarCuerpo();
});

function mostrarPrevisualizacion(event) {

    const input = event.target;
    const imagenPreview = document.getElementById("PreviewImagen");

    if (input.files && input.files[0]) {
        /* Se obtiene una representacion de la imagen seleccionada por el usuario */
        const urlImagen = URL.createObjectURL(input.files[0]);

        /* usando esta constante que representa el elemento html */
        imagenPreview.src = urlImagen;
        imagenPreview.style.display = "block";
    }
}