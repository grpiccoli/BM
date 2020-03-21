var charts = [];
var exportsData = {
    images: {}
};
var processedCharts = 0;
$(document).ready(function () {
    for (var c in charts) {
        if (document.getElementById(c) === null)
            continue;
        var chart = charts[c];
        charts.push(Chart(chart.type, c, chart.data, chart.options));
    }
    $("#downloadReport").click(function (e) {
        e.preventDefault();
        generatePdf();
    });
});
function generatePdf() {
    setExportStatus(true);
    setTimeout(() => {
        processedCharts = 0;
        for (var c in charts) {
            addToExport(charts[c], handleUploadFiles);
        }
    }, 1000);
}
function setExportStatus(exporting) {
    var exportingClass = "exporting";
    if (exporting) {
        $(".showOnExport").addClass(exportingClass);
    }
    else {
        $(".showOnExport").removeClass(exportingClass);
    }
}
function addToExport(c, uploadFunction) {
    var chart = c.getChart();
    var selector = c.getSelector();
    chart["export"].capture({}, function () {
        this.toPNG({}, (base64) => {
            exportsData.images[selector] = base64;
            processedCharts++;
            if (processedCharts === charts.length) {
                uploadFunction();
            }
        });
    });
}
function handleUploadFiles() {
    exportsData["selected_quarter_name"] = selected_quarter_name;
    exportsData["quarter_lastyear_name"] = quarter_lastyear_name;
    exportsData["total_semillas"] = total_semillas;
    exportsData["total_cosechas"] = total_cosechas;
    exportsData["total_abastecimiento"] = total_abastecimiento;
    exportsData["commune_table_tab1"] = commune_table_tab1;
    exportsData["commune_table_tab2"] = commune_table_tab2;
    exportsData["product_table_tab2"] = product_table_tab2;
    exportsData["report_analysis"] = report_analysis;
    $.post("http://139.162.167.71:8081/generatepdf", exportsData, function (res) {
        try {
            var result = JSON.parse(res);
            if (result.hasOwnProperty("status") && result.status === "success") {
                setExportStatus(false);
                triggerDownload(result.documentPath);
            }
            else {
                setExportStatus(false);
                console.log("error");
            }
        }
        catch (e) {
            setExportStatus(false);
            console.log("Error: " + e);
        }
    });
}
function triggerDownload(src) {
    var link = document.createElement("a");
    link.href = src;
    link.download = "boletin.pdf";
    $("body").append(link);
    link.click();
    $(link).remove();
}
//# sourceMappingURL=dashboard.js.map