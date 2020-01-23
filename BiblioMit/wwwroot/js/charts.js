function Chart(type, selector, data, options) {
    var _type = type;
    var _selector = selector;
    var _data = data;
    var _options = options;
    var _chart = null;

    var _config = {
        default: {
            // disable init animation
            startDuration: 0,
            // remove ballons
            showBalloon: function(){},
            // export config
            export: {
                enabled: true,
                menu: []
            },
            responsive: {
                enabled: true
            },
            precision: 0,
            decimalSeparator: ",",
            thousandsSeparator: "."
        },
        pie: {
            type: "pie",
            // two lines label text
            labelText: "[[text]]<br>[[percents]]%",
            // put label near the chart
            labelRadius: 15,
            valueField: "value",
            titleField: "text",
            // remove margins and fit to available space
            autoMargins: false,
            marginTop: 40,
            marginBottom: 40,
            marginLeft: 0,
            marginRight: 0,
            pullOutRadius: 0,
            // inner white outline
            outlineColor: "#FFFFFF",
            outlineAlpha: 0.8,
            outlineThickness: 2,
            legend: {
                enabled: true,
                align: "center",
                spacing: 20,
                valueAlign: "left",
                valueText: " [[value]] Ton.",
                valueWidth: 80,
                verticalGap: 5
            }

        },
        bigPie: {
            type: "pie",
            // two lines label text
            labelText: "[[text]]<br>[[percents]]%",
            // put label near the chart
            labelRadius: 15,
            valueField: "value",
            titleField: "text",
            // remove margins and fit to available space
            autoMargins: false,
            marginTop: 70,
            marginBottom: 50,
            marginLeft: 0,
            marginRight: 0,
            pullOutRadius: 0,
            // inner white outline
            outlineColor: "#FFFFFF",
            outlineAlpha: 0.8,
            outlineThickness: 2,
            legend: {
                enabled: true,
                align: "center",
                spacing: 20,
                valueAlign: "left",
                valueText: " [[value]] Ton.",
                valueWidth: 80,
                verticalGap: 5
            }

        },
        bar: {
            type: "serial",
            categoryField: "category",
            columnSpacing: 3,
            categoryAxis: {
                autoRotateAngle: 0,
                gridPosition: "start",
                gridThickness: 0,
                labelRotation: 90,
                minVerticalGap: 34,
                autoGridCount: false,
                // this is the max number of labels shown
                gridCount: 20
            },
            guides: [],
            trendLines: [],
            allLabels: [],
            balloon: {},
            legend: {
                enabled: true,
                align: "center",
                useGraphSettings: true
            }
        },
        line: {
            type: "serial",
            categoryField: "category",
            categoryAxis: {
                gridPosition: "start",
                gridThickness: 0
            },
            trendLines: [],
            guides: [],
            allLabels: [],
            balloon: {},
            legend: {
                enabled: true,
                spacing: 5,
                useGraphSettings: true,
                verticalGap: 0
            }
        }
    };

    function init() {
        _chart = AmCharts.makeChart(
            _selector,
            buildData()
        );
    }

    function buildData() {
        var data = _config["default"];

        // add type defaults to generic defaults
        for (var o in _config[_type]) {
            data[o] = _config[_type][o];
        }

        // rewrite default config with options
        for (var o in _options) {
            data[o] = _options[o];
        }

        data["dataProvider"] = _data;

        return data;
    }

    this.getSelector = function() {
        return _selector;
    };

    this.getChart = function() {
        return _chart;
    };

    init();

    return this;
}