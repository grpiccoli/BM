$('form input#Phone').keyup(function () {
    $(this).val(formatPhone($(this).val().toString()));
});
function formatPhone(p) {
    var numArray = p.replace(/\D/g, '');
    var sections = [];
    if (numArray.slice(2, 3) === '9') {
        sections = [
            '+' + numArray.slice(0, 2),
            numArray.slice(2, 3),
            numArray.slice(3, 7),
            numArray.slice(7)
        ];
        return sections.join(' ').trim();
    }
    else {
        sections = [
            '+' + numArray.slice(0, 2),
            numArray.slice(2, 4),
            numArray.slice(4, 7),
            numArray.slice(7)
        ];
        return sections.join(' ').trim();
    }
}
//# sourceMappingURL=format-phone.js.map