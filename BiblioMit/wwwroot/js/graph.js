var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
var _this = this;
$('.input-daterange').datepicker({
    inputs: $('.actual_range'),
    format: 'yyyy-MM-dd',
    language: 'es'
});
var start = $("#start").val();
var end = $("#end").val();
am4core.useTheme(am4themes_kelly);
var chart4 = am4core.create("chartdiv", am4charts.XYChart);
chart4.scrollbarX = new am4core.Scrollbar();
chart4.scrollbarX.parent = chart4.bottomAxesContainer;
chart4.scrollbarY = new am4core.Scrollbar();
chart4.scrollbarY.parent = chart4.leftAxesContainer;
chart4.language.locale = am4lang_es_ES;
var dataSource = chart4.dataSource;
dataSource.url = "/ambiental/graphdata?area=1&var=t&start=" + start + "&end=" + end;
var dateAxis = chart4.xAxes.push(new am4charts.DateAxis());
dateAxis.dataFields.category = "date";
dateAxis.dateFormats.setKey("year", "yy");
dateAxis.periodChangeDateFormats.setKey("month", "MMM yy");
dateAxis.tooltipDateFormat = "dd MMM, yyyy";
dateAxis.renderer.minGridDistance = 40;
var valueAxis = chart4.yAxes.push(new am4charts.ValueAxis());
chart4.cursor = new am4charts.XYCursor();
chart4.cursor.xAxis = dateAxis;
chart4.exporting.menu = new am4core.ExportMenu();
$.getJSON("/ambiental/exportmenu", function (menu) {
    chart4.exporting.menu.items = menu;
});
var series = new Object();
var semaforo = !$("#semaforo").hasClass("d-none");
function loadData(values, event, boolpsmb) {
    psmb.disable();
    variables.disable();
    if (semaforo)
        tl.disable();
    var temps = [];
    var requests = [];
    var tags = [];
    var names = [];
    $.each(values, function (_i, e) {
        var x = boolpsmb ? [e.value, event.value, e.label, event.label, e.groupId === 2 ? "fito" : "graph"] :
            [event.value, e.value, event.label, e.label, event.groupValue === "Fitoplancton" ? "fito" : "graph"];
        tags.push(x[0] + "_" + x[1]);
        names.push(x[2] + " " + x[3]);
        var d = $.Deferred();
        requests.push(d.promise());
        var url = "/ambiental/" + x[4] + "data?area=" + x[1] + "&var=" + x[0] + "&start=" + start + "&end=" + end;
        $.getJSON(url, function (data) {
            temps.push(data);
            d.resolve();
        });
    });
    $.when.apply(null, requests).done(function () {
        if (temps.length !== 0)
            $.each(chart4.data, function (j) {
                for (var i = 0; i < temps.length; i++) {
                    chart4.data[j][tags[i]] = temps[i][j][tags[i]];
                }
            });
        for (var i = 0; i < temps.length; i++) {
            series[tags[i]] = chart4.series.push(new am4charts.LineSeries());
            series[tags[i]].dataFields.valueY = tags[i];
            series[tags[i]].dataFields.dateX = "date";
            series[tags[i]].name = names[i];
            series[tags[i]].tooltipText = "{name}: [bold]{valueY}[/]";
            series[tags[i]].tooltip.pointerOrientation = "vertical";
        }
        chart4.invalidateData();
        psmb.enable();
        variables.enable();
        if (semaforo)
            tl.enable();
    });
}
var epsmb = document.getElementById('psmb');
epsmb.addEventListener('addItem', function (event) {
    loadData(variables.getValue(), event.detail, 1);
    if (polygons[event.detail.value] !== undefined && event !== undefined)
        google.maps.event.trigger(polygons[event.detail.value], 'click', {});
}, false);
epsmb.addEventListener('removeItem', function (event) {
    $.each(variables.getValue(true), function (_i, e) {
        var name = e + "_" + event.detail.value;
        chart4.series.removeIndex(chart4.series.indexOf(series[name])).dispose();
    });
    if (event !== undefined)
        google.maps.event.trigger(polygons[event.detail.value], 'click', {});
}, false);
var psmb = new Choices(epsmb, {
    maxItemCount: 10,
    removeItemButton: true,
    duplicateItemsAllowed: false,
    paste: false,
    searchResultLimit: 10,
    shouldSort: false,
    placeholderValue: "Seleccione áreas",
    searchPlaceholderValue: "Buscar datos",
    loadingText: 'Cargando...',
    noResultsText: 'Sin resultados',
    noChoicesText: 'Sin opciones a elegir',
    itemSelectText: 'Presione para seleccionar',
    maxItemText: function (maxItemCount) {
        return "M\u00E1ximo " + maxItemCount + " valores";
    },
    fuseOptions: {
        include: 'score'
    }
});
psmb.setChoices(function () { return __awaiter(_this, void 0, void 0, function () {
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0: return [4, fetch('/ambiental/psmblist')
                    .then(function (items) { return items.json(); })
                    .catch(function (err) { return console.log(err); })];
            case 1: return [2, _a.sent()];
        }
    });
}); });
var evariable = document.getElementById('variable');
evariable.addEventListener('addItem', function (event) {
    loadData(psmb.getValue(), event.detail, 0);
}, false);
evariable.addEventListener('removeItem', function (event) {
    $.each(psmb.getValue(true), function (_i, e) {
        var name = event.detail.value + "_" + e;
        chart4.series.removeIndex(chart4.series.indexOf(series[name])).dispose();
    });
}, false);
var variables = new Choices(evariable, {
    maxItemCount: 10,
    removeItemButton: true,
    duplicateItemsAllowed: false,
    paste: false,
    searchResultLimit: 10,
    shouldSort: false,
    placeholderValue: "Seleccione variables",
    searchPlaceholderValue: "Buscar datos",
    loadingText: 'Cargando...',
    noResultsText: 'Sin resultados',
    noChoicesText: 'Sin opciones a elegir',
    itemSelectText: 'Presione para seleccionar',
    maxItemText: function (maxItemCount) {
        return "M\u00E1ximo " + maxItemCount + " valores";
    },
    fuseOptions: {
        include: 'score'
    }
});
variables.setChoices(function () { return __awaiter(_this, void 0, void 0, function () {
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0: return [4, fetch('/ambiental/variablelist')
                    .then(function (items) { return items.json(); })
                    .catch(function (err) { return console.log(err); })];
            case 1: return [2, _a.sent()];
        }
    });
}); });
var temp;
var etl = document.getElementById('tl');
etl.addEventListener('addItem', function (event) {
    if (event.detail.groupValue === "Análisis") {
        var a = event.detail.value;
        var tls = tl.getValue();
        var psmbs = tls.filter(function (x) { return x.groupId === 2; });
        var sps = tls.filter(function (x) { return x.groupId === 3; });
        temp = psmbs;
        if (psmbs.length !== 0 && sps.length !== 0) {
            psmb.disable();
            variables.disable();
            tl.disable();
            var temps = [];
            var requests = [];
            var tags = [];
            var names = [];
            switch (a) {
                case "11":
                    var ts = tls.filter(function (x) { return x.groupId === 4; });
                    if (ts.length !== 0) {
                        psmbs.forEach(function (psmb) {
                            sps.forEach(function (sp) {
                                ts.forEach(function (t) {
                                    tags.push([a, psmb.value, sp.value, t.value].join("_"));
                                    names.push([event.detail.name, psmb.name, sp.name, t.name].join(" "));
                                    var d = $.Deferred();
                                    requests.push(d.promise());
                                    var url = "/ambiental/tldata?a=" + a + "&psmb=" + psmb.value + "&sp=" + sp.value +
                                        "&t=" + t.value.slice(1, 2) + "&start=" + start + "&end=" + end;
                                    $.getJSON(url, function (data) {
                                        temps.push(data);
                                        d.resolve();
                                    });
                                });
                            });
                        });
                    }
                    else {
                        alert("Talla no seleccionada, seleccione requerimientos y luego análisis");
                    }
                    break;
                case "12":
                    var ls = tls.filter(function (x) { return x.groupId === 5; });
                    if (ls.length !== 0) {
                        psmbs.forEach(function (psmb) {
                            sps.forEach(function (sp) {
                                ls.forEach(function (l) {
                                    tags.push([a, psmb.value, sp.value, l.value].join("_"));
                                    names.push([event.detail.name, psmb.name, sp.name, l.name].join(" "));
                                    var d = $.Deferred();
                                    requests.push(d.promise());
                                    var url = "/ambiental/tldata?a=" + a + "&psmb=" + psmb.value + "&sp=" + sp.value +
                                        "&l=" + l.value + "&start=" + start + "&end=" + end;
                                    $.getJSON(url, function (data) {
                                        temps.push(data);
                                        d.resolve();
                                    });
                                });
                            });
                        });
                    }
                    else {
                        alert("Larva no seleccionada, seleccione requerimientos y luego análisis");
                    }
                    break;
                case "13":
                    var ss = tls.filter(function (x) { return x.groupId === 7; });
                    if (ss.legnth !== 0) {
                        psmbs.forEach(function (psmb) {
                            sps.forEach(function (sp) {
                                ss.forEach(function (s) {
                                    tags.push([a, psmb.value, sp.value, s.value].join("_"));
                                    names.push([event.detail.name, psmb.name, sp.name, s.name].join(" "));
                                    var d = $.Deferred();
                                    requests.push(d.promise());
                                    var url = "/ambiental/tldata?a=" + a + "&psmb=" + psmb.value + "&sp=" + sp.value +
                                        "&s=" + s.value + "&start=" + start + "&end=" + end;
                                    $.getJSON(url, function (data) {
                                        temps.push(data);
                                        d.resolve();
                                    });
                                });
                            });
                        });
                    }
                    else {
                        alert("Sexo no seleccionado, seleccione requerimientos y luego análisis");
                    }
                    break;
                case "14":
                    psmbs.forEach(function (psmb) {
                        sps.forEach(function (sp) {
                            tags.push([a, psmb.value, sp.value].join("_"));
                            names.push([event.detail.name, psmb.name, sp.name].join(" "));
                            var d = $.Deferred();
                            requests.push(d.promise());
                            var url = "/ambiental/tldata?a=" + a + "&psmb=" + psmb.value + "&sp=" + sp.value + "&start=" + start + "&end=" + end;
                            $.getJSON(url, function (data) {
                                temps.push(data);
                                d.resolve();
                            });
                        });
                    });
                    break;
                case "15":
                    var er = tls.filter(function (x) { return x.groupId === 6; });
                    if (er.legnth !== 0) {
                        psmbs.forEach(function (psmb) {
                            sps.forEach(function (sp) {
                                er.forEach(function (s) {
                                    tags.push([a, psmb.value, sp.value, s.value].join("_"));
                                    names.push([event.detail.name, psmb.name, sp.name, s.name].join(" "));
                                    var d = $.Deferred();
                                    requests.push(d.promise());
                                    var url = "/ambiental/tldata?a=" + a + "&psmb=" + psmb.value + "&sp=" + sp.value +
                                        "&rs=" + s.value + "&start=" + start + "&end=" + end;
                                    $.getJSON(url, function (data) {
                                        temps.push(data);
                                        d.resolve();
                                    });
                                });
                            });
                        });
                    }
                    else {
                        alert("Estado reproductivo no seleccionado, seleccione requerimientos y luego análisis");
                    }
                    break;
                case "16":
                    ss = tls.filter(function (x) { return x.groupId === 7; });
                    if (ss.legnth !== 0) {
                        psmbs.forEach(function (psmb) {
                            sps.forEach(function (sp) {
                                ss.forEach(function (s) {
                                    tags.push([a, psmb.value, sp.value, s.value].join("_"));
                                    names.push([event.detail.name, psmb.name, sp.name, s.name].join(" "));
                                    var d = $.Deferred();
                                    requests.push(d.promise());
                                    var url = "/ambiental/tldata?a=" + a + "&psmb=" + psmb.value + "&sp=" + sp.value +
                                        "&s=" + s.value + "&start=" + start + "&end=" + end;
                                    $.getJSON(url, function (data) {
                                        temps.push(data);
                                        d.resolve();
                                    });
                                });
                            });
                        });
                    }
                    else {
                        alert("Sexo no seleccionado, seleccione requerimientos y luego análisis");
                    }
                    break;
                case "17":
                    psmbs.forEach(function (psmb) {
                        sps.forEach(function (sp) {
                            tags.push([a, psmb.value, sp.value].join("_"));
                            names.push([event.detail.name, psmb.name, sp.name].join(" "));
                            var d = $.Deferred();
                            requests.push(d.promise());
                            var url = "/ambiental/tldata?a=" + a + "&psmb=" + psmb.value + "&sp=" + sp.value + "&start=" + start + "&end=" + end;
                            $.getJSON(url, function (data) {
                                temps.push(data);
                                d.resolve();
                            });
                        });
                    });
                    break;
            }
            $.when.apply(null, requests).done(function () {
                if (temps.length !== 0)
                    temp = temps;
                $.each(chart4.data, function (j) {
                    for (var i = 0; i < temps.length; i++) {
                        chart4.data[j][tags[i]] = temps[i][j][tags[i]];
                    }
                });
                for (var i = 0; i < temps.length; i++) {
                    series[tags[i]] = chart4.series.push(new am4charts.LineSeries());
                    series[tags[i]].dataFields.valueY = tags[i];
                    series[tags[i]].dataFields.dateX = "date";
                    series[tags[i]].name = names[i];
                    series[tags[i]].tooltipText = "{name}: [bold]{valueY}[/]";
                    series[tags[i]].tooltip.pointerOrientation = "vertical";
                }
            });
            chart4.invalidateData();
            psmb.enable();
            variables.enable();
            tl.enable();
        }
        else {
            tl.removeActiveItemsByValue(a);
            alert("PSMB y especie no seleccionados, seleccione requerimientos y luego análisis");
        }
    }
}, false);
etl.addEventListener('removeItem', function (event) {
    if (event.detail.groupValue === "Análisis") {
        var a = event.detail.value;
        var tls = tl.getValue();
        var psmbs = tls.filter(function (x) { return x.groupId === 2; });
        var sps = tls.filter(function (x) { return x.groupId === 3; });
        if (psmbs.length !== 0 && sps.length !== 0) {
            psmb.disable();
            variables.disable();
            tl.disable();
            switch (a) {
                case "11":
                    var ts = tls.filter(function (x) { return x.groupId === 4; });
                    if (ts.length !== 0) {
                        psmbs.forEach(function (psmb) {
                            sps.forEach(function (sp) {
                                ts.forEach(function (t) {
                                    var name = [a, psmb.value, sp.value, t.value].join("_");
                                    chart4.series.removeIndex(chart4.series.indexOf(series[name])).dispose();
                                });
                            });
                        });
                    }
                    break;
                case "12":
                    var ls = tls.filter(function (x) { return x.groupId === 5; });
                    if (ls.length !== 0) {
                        psmbs.forEach(function (psmb) {
                            sps.forEach(function (sp) {
                                ls.forEach(function (l) {
                                    var name = [a, psmb.value, sp.value, l.value].join("_");
                                    chart4.series.removeIndex(chart4.series.indexOf(series[name])).dispose();
                                });
                            });
                        });
                    }
                    break;
            }
        }
    }
}, false);
var tl = new Choices(etl, {
    maxItemCount: 10,
    removeItemButton: true,
    duplicateItemsAllowed: false,
    paste: false,
    searchResultLimit: 10,
    shouldSort: false,
    placeholderValue: "Variables semáforo",
    searchPlaceholderValue: "Buscar datos",
    loadingText: 'Cargando...',
    noResultsText: 'Sin resultados',
    noChoicesText: 'Sin opciones a elegir',
    itemSelectText: 'Presione para seleccionar',
    maxItemText: function (maxItemCount) {
        return "M\u00E1ximo " + maxItemCount + " valores";
    },
    fuseOptions: {
        include: 'score'
    }
});
tl.setChoices(function () { return __awaiter(_this, void 0, void 0, function () {
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0: return [4, fetch('/ambiental/tllist')
                    .then(function (items) { return items.json(); })
                    .catch(function (err) { return console.log(err); })];
            case 1: return [2, _a.sent()];
        }
    });
}); });
function Area(array) {
    var n = google.maps.geometry.spherical.computeArea(array) / 10000;
    return parseFloat(n.toString()).toFixed(2);
}
var addListenerOnPolygon = function (polygon) {
    var selected = 'red';
    google.maps.event.addListener(polygon, 'click', function (e) {
        if ($.isEmptyObject(e)) {
            psmb.getValue(true).includes(this.zIndex.toString()) ?
                this.setOptions({ fillColor: selected, strokeColor: selected }) :
                this.setOptions({ fillColor: undefined, strokeColor: undefined });
        }
        else {
            if (psmb.getValue(true).includes(this.zIndex.toString())) {
                this.setOptions({ fillColor: undefined, strokeColor: undefined });
                psmb.removeActiveItemsByValue(this.zIndex.toString());
            }
            else {
                this.setOptions({ fillColor: selected, strokeColor: selected });
                psmb.setChoiceByValue(this.zIndex.toString());
            }
        }
    });
};
function getBounds(positions) {
    var bounds = new google.maps.LatLngBounds();
    if (Array.isArray(positions[0])) {
        $.each(positions, function (_c, position) {
            $.each(position, function (_c, p) {
                bounds.extend(p);
            });
        });
    }
    else {
        $.each(positions, function (_c, position) {
            bounds.extend(position);
        });
    }
    return bounds;
}
var map = new google.maps.Map(document.getElementById('map'), {
    mapTypeId: 'terrain'
});
var infowindow = new google.maps.InfoWindow({
    maxWidth: 150
});
var polygons = {};
var markers = [];
window.onload = function initMap() {
    $.getJSON("/ambiental/mapdata", function getData(data) {
        var bounds = new google.maps.LatLngBounds();
        $.each(data.slice(0, 3), function (_i, dato) {
            $.each(dato.position, function (_c, position) {
                bounds.extend(position);
            });
        });
        map.fitBounds(bounds);
        map.setCenter(bounds.getCenter());
        $.each(data, function (i, dato) {
            var consessionPolygon = new google.maps.Polygon({
                paths: dato.position,
                zIndex: dato.id
            });
            addListenerOnPolygon(consessionPolygon);
            polygons[dato.id] = consessionPolygon;
            var marker = new google.maps.Marker({
                position: getBounds(dato.position).getCenter(),
                title: dato.name + " " + dato.id
            });
            google.maps.event.addListener(marker, 'click', (function (marker, _m) {
                return function () {
                    var content = '<h4>' + dato.name + '</h4>' + '<table>' + '<tr><th>Código</th><td align="right">' + dato.id + '</td></tr>';
                    if (dato.comuna !== undefined)
                        content += '<tr><th>Comuna</th><td align="right">' + dato.comuna + '</td></tr>';
                    if (dato.provincia !== undefined)
                        content += '<tr><th>Provincia</th><td align="right">' + dato.provincia + '</td></tr>';
                    content += '<tr><th>Región</th><td align="right">' + dato.region + '</td></tr>' +
                        '<tr><th>Área (ha)</th><td align="right">' + Area(consessionPolygon.getPath().getArray()) + '</td></tr>' +
                        '<tr><th>Fuentes</th><td></td></tr>' +
                        '<tr><td>Sernapesca</td><td align="right"><a target="_blank" href="https://www.sernapesca.cl"><img src="../images/ico/sernapesca.svg" height="30" /></a></td></tr>' +
                        '<tr><td>PER Mitilidos</td><td align="right"><a target="_blank" href="https://www.mejillondechile.cl/"><img src="../images/ico/mejillondechile.min.png" height="30" /></a></td></tr>' +
                        '<tr><td>Subpesca</td><td align="right"><a target="_blank" href="https://www.subpesca.cl"><img src="../images/ico/subpesca.png" height="30" /></a></td></tr>';
                    infowindow.setContent(content);
                    infowindow.open(map, marker);
                    map.setCenter(marker.getPosition());
                };
            })(marker, i));
            markers.push(marker);
            consessionPolygon.setMap(map);
        });
        var markerCluster = new MarkerClusterer(map, markers, { imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });
        google.maps.event.trigger(polygons[1], 'click', {});
    });
};
//# sourceMappingURL=graph.js.map