$('input.number').keyup(function () {
    var selection = window.getSelection().toString();
    if (selection !== '') {
        return;
    }
    var input = parseInt($(this).val().toString().replace(/[\D\s\._\-]+/g, ""));
    if (input)
        input = 0;
    $(this).val(function (_) {
        return input === 0 ? "" : input.toLocaleString("es-CL");
    });
});
//# sourceMappingURL=parseNumber.js.map