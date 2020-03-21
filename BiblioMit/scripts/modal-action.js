$(function () {
    $('#modal-action').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget);
        var url = button.attr("href");
        var modal = $(this);
        modal.find('.modal-content').load(url);
    });
    $('#modal-action').on('hidden.bs.modal', function () {
        $(this).removeData('bs.modal');
        $('#modal-action .modal-content').empty();
    });
    $('#modal-action').change(_ => {
        $.validator.unobtrusive.parse('form#modal-form');
    });
});
//# sourceMappingURL=modal-action.js.map