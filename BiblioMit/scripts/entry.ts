const entry = new signalR.HubConnectionBuilder()
    .withUrl("/entryHub")
    .build();

entry.on("Update", (func: any, e: any) => {
    console.log(func, e);
    if (func === "log") {
        const p = document.createElement("p");
        p.textContent = e;
        $("#log").html(p);
        document.getElementById("log").scrollTop = document.getElementById("log").scrollHeight;
    } else if (func === "agregada") {
        $(".agregadas").html(e);
    } else if (func === "actualizada") {
        $(".actualizadas").html(e);
    } else if (func === "progress") {
        var pgr = e.toFixed(2);
        $("#progressHub").css('width', pgr + "%").attr('aria-valuenow', pgr).html(pgr + '%');
    } else if (func === "status") {
        var status = "bg-" + e;
        $("#progressHub").removeClass('bg-danger bg-warning bg-success bg-info');
        $("#progressHub").addClass(status);
    }
});

entry.start().catch((err: any) => console.error(err.toString()));