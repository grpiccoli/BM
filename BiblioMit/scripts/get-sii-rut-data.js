$("form input#RUT")
    .rut({ formatOn: 'keyup', validateOn: 'keyup' })
    .on('rutInvalido', function () {
    $(this).parents(".form-group").addClass("has-error");
    $('[data-valmsg-for="RUT"]').html("Rut Inválido");
})
    .on('rutValido', function () {
    $(this).parents(".form-group").removeClass("has-error");
    $('[data-valmsg-for="RUT"]').html("");
});
$('form input#RUT')
    .rut({ formatOn: 'change', validateOn: 'change' })
    .on('rutInvalido', function () {
    $(this).parents(".form-group").addClass("has-error");
    $('[data-valmsg-for="RUT"]').html("Rut Inválido");
    $("input[type=submit]").prop('disabled', true);
})
    .on('rutValido', function () {
    $(this).parents(".form-group").removeClass("has-error");
    $('[data-valmsg-for="RUT"]').html("");
    var url = "/Clientes/ClientExists/RUT";
    url = url.replace("RUT", $(this).val().toString().replace(new RegExp("-.*"), "")
        .replace(new RegExp("\\.", "g"), ""));
    $.get(url, {}, (r) => {
        if (r.response === "True" && $(document).find("title").text() === 'Nuevo Cliente - CLM') {
            $('form input#RUT').parents(".form-group").addClass("has-error");
            $('[data-valmsg-for="RUT"]').html('Cliente ya ingresado a la base de datos');
            $("input[type=submit]").prop('disabled', true);
        }
        else {
            if ($('#RUT').val() !== '') {
                $.getJSON('https://siichile.herokuapp.com/consulta', {
                    rut: $('#RUT').val().toString().replace(/[^0-9Kk]/g, '')
                }, function (result) {
                    if (result.error) {
                        $('#Name').val('');
                        $('[data-valmsg-for="RUT"]').html(result.rut + " " + result.error);
                        $('#Giros.selectpicker').selectpicker('val', '');
                        $("input[type=submit]").prop('disabled', true);
                    }
                    else {
                        $('[data-valmsg-for="RUT"]').empty();
                        $('#Name').val(toTitleCase(result.razon_social.toString().toLowerCase()));
                        $('#Giros.selectpicker')
                            .selectpicker('val', result.actividades.map((a) => a.codigo));
                        $("input[type=submit]").prop('disabled', false);
                    }
                });
            }
            else {
                $('#rutVal').empty();
                $('#rutPass').empty();
                $('#Name').val('');
                $("input[type=submit]").prop('disabled', true);
            }
        }
    });
});
function toTitleCase(str) {
    return str.replace(/(?:^|\s)\w/g, function (match) {
        return match.toUpperCase();
    });
}
//# sourceMappingURL=get-sii-rut-data.js.map