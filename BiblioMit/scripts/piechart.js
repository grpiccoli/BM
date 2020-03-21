AmCharts.addInitHandler((chart) => {
    if (chart.legend === undefined || chart.legend.truncateLabels === undefined)
        return;
    var titleField = chart.titleField;
    var legendTitleField = chart.titleField + "Legend";
    for (var i = 0; i < chart.dataProvider.length; i++) {
        var label = chart.dataProvider[i][chart.titleField];
        if (label.length > chart.legend.truncateLabels)
            label = label.substr(0, chart.legend.truncateLabels - 1) + '...';
        chart.dataProvider[i][legendTitleField] = label;
    }
    chart.titleField = legendTitleField;
    chart.balloonText = chart.balloonText.replace(/\[\[title\]\]/, "[[" + titleField + "]]");
}, ["pie"]);
var chart = AmCharts.makeChart("chartdiv", {
    "type": "pie",
    "theme": "light",
    "labelsEnabled": false,
    "legend": {
        "markerType": "circle",
        "position": "right",
        "marginRight": 80,
        "autoMargins": false,
        "truncateLabels": 25
    },
    "dataProvider": [{
            "country": "non.",
            "litres": 256.9
        }, {
            "country": "magna.",
            "litres": 131.1
        }, {
            "country": "elementum.",
            "litres": 115.8
        }, {
            "country": "Aenean.",
            "litres": 109.9
        }, {
            "country": "commodo.",
            "litres": 108.3
        }, {
            "country": "est.",
            "litres": 65
        }, {
            "country": "elit.",
            "litres": 40
        }],
    "valueField": "litres",
    "titleField": "country",
    "balloonText": "[[title]]<br><span style='font-size:10px'><b>[[value]]</b> ([[percents]]%)</span>"
});
//# sourceMappingURL=piechart.js.map