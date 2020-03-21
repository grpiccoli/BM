$('form input#Phone').keyup(function () {
    $(this).val(formatPhone($(this).val().toString()));
});
function formatPhone(p: string) {
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
        //if (numArray.slice(3, 4).join('').match(/[2,32-35,39,41-45,51-53,55,57,58,61,63-65,67,68,71-73,75]/)
    } else {
        sections = [
            '+' + numArray.slice(0, 2),
            numArray.slice(2, 4),
            numArray.slice(4, 7),
            numArray.slice(7)
        ];
        return sections.join(' ').trim();
    }
}
