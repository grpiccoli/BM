function Chart(type, selector, data, options) {
    var _type = type;
    var _selector = selector;
    var _data = data;
    var _options = options;
    var _chart;
    var _config = {
        startDuration: 0,
        showBalloon: function () { },
        export: {
            enabled: true
        },
        responsive: {
            enabled: true
        },
        precision: 0,
        decimalSeparator: ",",
        thousandsSeparator: "."
    }, pie, bigPie, bar, line;
    function init() {
        _chart = AmCharts.makeChart(_selector, buildData());
    }
    function buildData() {
        var data = _config["default"];
        for (var o in _config[_type]) {
            data[o] = _config[_type][o];
        }
        for (var p in _options) {
            data[p] = _options[p];
        }
        data["dataProvider"] = _data;
        return data;
    }
    this.getSelector = function (_) {
        return _selector;
    };
    this.getChart = function (_) {
        return _chart;
    };
    init();
    return this;
}
//# sourceMappingURL=charts.js.map