$("form input").on('keyup keypress', (e) => {
    var keyCode = e.keyCode || e.which;
    if (keyCode == 13) {
        e.preventDefault();
    }
});
//# sourceMappingURL=dontSendFormOnEnter.js.map