const progress = new signalR.HubConnectionBuilder()
    .withUrl("/progressHub")
    .build();

progress.on("Progress", (_user: string, progress: number) => {
    var pgr = progress.toFixed(2);
    $("#progressHub").css('width', pgr+"%").attr('aria-valuenow', pgr).html(pgr+'%');
});

progress.on("Status", (_user: string, color: string) => {
    var status = "bg-" + color;
    $("#progressHub").removeClass('bg-danger bg-warning bg-success bg-info');
    $("#progressHub").addClass(status);
});

progress.start().catch((err: any) => console.error(err.toString()));