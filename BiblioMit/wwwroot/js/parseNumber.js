$('input.number').keyup(function () {
    var selection = window.getSelection().toString();
    if (selection !== '') {
        return;
    }
    var input = $(this).val().replace(/[\D\s\._\-]+/g, "");
    input = input ? parseInt(input) : 0;
    $(this).val(
        function () {
            return input === 0 ? "" : input.toLocaleString("es-CL");
        });
});