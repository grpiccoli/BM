$(function () {
    $('#modal-action').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); // Button that triggered the modal
        var url = button.attr("href");
        var modal = $(this);
        // note that this will replace the content of modal-content everytime the modal is opened
        modal.find('.modal-content').load(url);
    });
    // when the modal is closed
    $('#modal-action').on('hidden.bs.modal', function () {
        // remove the bs.modal data attribute from it
        $(this).removeData('bs.modal');
        // and empty the modal-content element
        $('#modal-action .modal-content').empty();
    });
    $('#modal-action').change(_ => {
        $.validator.unobtrusive.parse('form#modal-form');
    });
});