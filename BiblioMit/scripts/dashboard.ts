var charts: any = [];
var exportsData: any = {
    images: {}
};
var processedCharts = 0;

$(document).ready(function(){
    // loop all charts and build them
    for (var c in charts) {
        // prevent process a chart that has no div in this template
        if (document.getElementById(c) === null) continue;

        var chart = charts[c];
        // build chart and add to charts array
        charts.push(Chart(chart.type, c, chart.data, chart.options));
    }

    $("#downloadReport").click(function (e) {
        e.preventDefault();
        generatePdf();
    });
});

// function to upload all images in base64 and build the pdf document
function generatePdf() {
    setExportStatus(true);

    // wait while charts in hidden tabs are rendered
    setTimeout(() => {
        processedCharts = 0;
        // add all charts to exports data
        for (var c in charts) {
            addToExport(charts[c], handleUploadFiles);
        }
    }, 1000);
}

function setExportStatus(exporting: any) {
    var exportingClass = "exporting";
    if (exporting) {
        $(".showOnExport").addClass(exportingClass);
    } else {
        $(".showOnExport").removeClass(exportingClass);
    }
}

// encode charts in base64 png and add to upload array
function addToExport(c: any, uploadFunction: any) {
    var chart = c.getChart();
    var selector = c.getSelector();

    chart["export"].capture({}, function() {
        this.toPNG({}, (base64: any) => {
            exportsData.images[selector] = base64;
            processedCharts++;

            // upload images when all charts processed
            if(processedCharts === charts.length) {
                uploadFunction();
            }
        });
    });
}

// upload encoded charts images
function handleUploadFiles() {
    // add context data
    exportsData["selected_quarter_name"] = selected_quarter_name;
    exportsData["quarter_lastyear_name"] = quarter_lastyear_name;
    
    exportsData["total_semillas"] = total_semillas;
    exportsData["total_cosechas"] = total_cosechas;
    exportsData["total_abastecimiento"] = total_abastecimiento;
    
    exportsData["commune_table_tab1"] = commune_table_tab1;
    exportsData["commune_table_tab2"] = commune_table_tab2;
    exportsData["product_table_tab2"] = product_table_tab2;
    
    exportsData["report_analysis"] = report_analysis;
    
    $.post("http://139.162.167.71:8081/generatepdf", exportsData, function(res) {
        try {
            var result = JSON.parse(res);
            if (result.hasOwnProperty("status") && result.status === "success") {
                setExportStatus(false);
                triggerDownload(result.documentPath);
            } else {
                setExportStatus(false);
                // TODO: handle errors
                console.log("error");
            }
        } catch(e) {
            setExportStatus(false);
            // TODO: handle errors
            console.log("Error: " + e);
        }
    });
}

function triggerDownload(src: any) {
    var link = document.createElement("a");
    link.href = src;
    link.download = "boletin.pdf";
    $("body").append(link);
    link.click();
    $(link).remove();
}