var drop = $("input");
drop.on('dragenter', _ => {
    $(".drop").css({
        "border": "4px dashed #09f",
        "background": "rgba(0, 153, 255, .05)"
    });
    $(".cont").css({
        "color": "#09f"
    });
}).on('dragleave dragend mouseout drop', _ => {
    $(".drop").css({
        "border": "3px dashed #DADFE3",
        "background": "transparent"
    });
    $(".cont").css({
        "color": "#8E99A5"
    });
});
function handleFileSelect(evt) {
    var files = evt.target.files;
    for (var i = 0; i < files.length; i++) {
        var f = files[i];
        var reader = new FileReader();
        reader.onload = ((theFile) => {
            return (_) => {
                var file = theFile.name.slice(0, 12) + "...xlsx";
                var filename = decodeURI(escape(file));
                $(".tit").html('<span class="icon-excel"><span class="path1"></span><span class="path2"></span></span><br/>' + filename);
                $('#filename').html(filename);
            };
        })(f);
        reader.readAsDataURL(f);
    }
}
$('#files').change(handleFileSelect);
//# sourceMappingURL=dragndrop.js.map