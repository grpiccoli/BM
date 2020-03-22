var progress = new signalR.HubConnectionBuilder()
    .withUrl("/progressHub")
    .build();
progress.on("Progress", function (_user, progress) {
    var pgr = progress.toFixed(2);
    $("#progressHub").css('width', pgr + "%").attr('aria-valuenow', pgr).html(pgr + '%');
});
progress.on("Status", function (_user, color) {
    var status = "bg-" + color;
    $("#progressHub").removeClass('bg-danger bg-warning bg-success bg-info');
    $("#progressHub").addClass(status);
});
progress.start().catch(function (err) { return console.error(err.toString()); });
//# sourceMappingURL=progress.js.map