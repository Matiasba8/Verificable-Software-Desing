// Para el new del formulario al cargar el documento
$(document).ready(function () {

    var enajenantes = $('#enajenantes');
    var cne_selector = $('#cne-selector');

    if (cne_selector.val() == "Regularización de Patrimonio") {

        var enajenantes = $('#enajenates');
        enajenantes.addClass("hide");

    } else if (cne_selector.val() == "Compraventa") {
        enajenantes.removeClass("hide");
    }
});



// al cambiar el selector del cne
$('body').on("change", "#cne-selector", function () {

    var enajenantes = $('#enajenantes');

    if ($(this).val() == "Regularización de Patrimonio") {
        enajenantes.addClass("hide");

    } else if ($(this).val() == "Compraventa") {
        enajenantes.removeClass("hide");
    }
});