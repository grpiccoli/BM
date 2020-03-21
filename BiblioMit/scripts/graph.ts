//DATES
$('.input-daterange').datepicker({
    inputs: $('.actual_range'),
    format: 'yyyy-MM-dd',
    language: 'es'
});

//CHART
var start = $("#start").val();
var end = $("#end").val();
am4core.useTheme(am4themes_kelly);
var chart4: any = am4core.create("chartdiv", am4charts.XYChart);
chart4.scrollbarX = new am4core.Scrollbar();
chart4.scrollbarX.parent = chart4.bottomAxesContainer;
chart4.scrollbarY = new am4core.Scrollbar();
chart4.scrollbarY.parent = chart4.leftAxesContainer;
chart4.language.locale = am4lang_es_ES;
var dataSource = chart4.dataSource; 
dataSource.url = "/ambiental/graphdata?area=1&var=t&start=" + start + "&end=" + end;
var dateAxis = chart4.xAxes.push(new am4charts.DateAxis());
dateAxis.dataFields.category = "date";
//dateAxis.dateFormats.setKey("day", "MM");
//dateAxis.dateFormats.setKey("month", "MMM");
dateAxis.dateFormats.setKey("year", "yy");
dateAxis.periodChangeDateFormats.setKey("month", "MMM yy");
dateAxis.tooltipDateFormat = "dd MMM, yyyy";
dateAxis.renderer.minGridDistance = 40;
var valueAxis = chart4.yAxes.push(new am4charts.ValueAxis());
chart4.cursor = new am4charts.XYCursor();
chart4.cursor.xAxis = dateAxis;
chart4.exporting.menu = new am4core.ExportMenu();
$.getJSON("/ambiental/exportmenu", (menu) => {
    chart4.exporting.menu.items = menu;
});

var series: any = new Object();

var semaforo = !$("#semaforo").hasClass("d-none");
//LOAD DATA
function loadData(values: any, event: any, boolpsmb: any) {
    psmb.disable();
    variables.disable();
    if(semaforo) tl.disable();
    var temps: any[] = [];
    var requests: any[] = [];
    var tags: any[] = [];
    var names: any[] = [];
    //var tag, area tag, var name, area name, group value
    $.each(values, (_i, e) => {
        var x = boolpsmb ? [e.value, event.value, e.label, event.label, e.groupId === 2 ? "fito" : "graph"] :
            [event.value, e.value, event.label, e.label, event.groupValue === "Fitoplancton" ? "fito": "graph"]; 
        tags.push(x[0] + "_" + x[1]);
        names.push(x[2] + " " + x[3]);
        var d = $.Deferred();
        requests.push(d.promise());
        var url = "/ambiental/" + x[4] + "data?area=" + x[1] + "&var=" + x[0] + "&start=" + start + "&end=" + end;
        console.log(url);
        $.getJSON(url, (data) => {
            temps.push(data);
            d.resolve();
        });
    });
    $.when.apply(null, requests).done(() => {
        if (temps.length !== 0)
            $.each(chart4.data, (j) => {
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
        if(semaforo) tl.enable();
    });
}

//PSMB SELECT
let epsmb = document.getElementById('psmb');
epsmb.addEventListener('addItem', (event: any) => {
    loadData(variables.getValue(), event.detail, 1);
    if (polygons[event.detail.value] !== undefined && event !== undefined)
        google.maps.event.trigger(polygons[event.detail.value], 'click', {});
}, false);
epsmb.addEventListener('removeItem', (event: any) => {
    $.each(variables.getValue(true), (_i, e) => {
        var name = e + "_" + event.detail.value;
        chart4.series.removeIndex(chart4.series.indexOf(series[name])).dispose();
    });
    if (event !== undefined)
        google.maps.event.trigger(polygons[event.detail.value], 'click', {});
}, false);
let psmb = new Choices(epsmb, {
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
    maxItemText: (maxItemCount: any) => {
        return `Máximo ${maxItemCount} valores`;
    },
    fuseOptions: {
        include: 'score'
    }
});
psmb.setChoices(async () => await fetch('/ambiental/psmblist')
    .then(items => items.json())
    .catch(err => console.log(err))
);
//psmb.setChoices(async () => {
//    const items = await fetch('/ambiental/psmblist');
//    return items.json();
//});
    //.ajax(
    //function (callback) {
    //    fetch('/ambiental/psmblist')
    //        .then(function (response) {
    //            response.json().then(function (data) {
    //                callback(data, "value", "label", false);
    //            });
    //        })
    //        .catch(function (error) {
    //            console.error(error);
    //        });
    //});

//VARIABLE SELECT
let evariable = document.getElementById('variable');
evariable.addEventListener('addItem', (event: any) => {
    loadData(psmb.getValue(), event.detail, 0);
}, false);
evariable.addEventListener('removeItem', (event: any) => {
    $.each(psmb.getValue(true), (_i, e) => {
        var name = event.detail.value + "_" + e;
        chart4.series.removeIndex(chart4.series.indexOf(series[name])).dispose();
    });
}, false);
let variables = new Choices(evariable, {
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
    maxItemText: (maxItemCount: any) => {
        return `Máximo ${maxItemCount} valores`;
    },
    fuseOptions: {
        include: 'score'
    }
});
//    .ajax(function (callback) {
//    fetch('/ambiental/variablelist')
//        .then(function (response) {
//            response.json().then(function (data) {
//                callback(data, "value", "label", false);
//            });
//        })
//        .catch(function (error) {
//            console.error(error);
//        });
//});

variables.setChoices(async () => await fetch('/ambiental/variablelist')
    .then(items => items.json())
    .catch(err => console.log(err))
);

var temp;
//SEMAFORO select
let etl = document.getElementById('tl');
etl.addEventListener('addItem', (event: any) => {
    if (event.detail.groupValue === "Análisis") {
        var a = event.detail.value;
        var tls = tl.getValue();
        var psmbs = tls.filter((x: any) => x.groupId === 2);
        var sps = tls.filter((x: any) => x.groupId === 3);
        temp = psmbs;
        if (psmbs.length !== 0 && sps.length !== 0) {
            psmb.disable();
            variables.disable();
            tl.disable();
            var temps: any[] = [];
            var requests: any[] = [];
            var tags: any[] = [];
            var names: any[] = [];
            switch (a) {
                case "11":
                    var ts = tls.filter((x: any) => x.groupId === 4);
                    if (ts.length !== 0) {
                        psmbs.forEach((psmb: any) => {
                            sps.forEach((sp: any) => {
                                ts.forEach((t: any) => {
                                    tags.push([a, psmb.value, sp.value, t.value].join("_"));
                                    names.push([event.detail.name, psmb.name, sp.name, t.name].join(" "));
                                    var d = $.Deferred();
                                    requests.push(d.promise());
                                    var url = "/ambiental/tldata?a=" + a + "&psmb=" + psmb.value + "&sp=" + sp.value +
                                        "&t=" + t.value.slice(1, 2) + "&start=" + start + "&end=" + end;
                                    console.log(url);
                                    $.getJSON(url, (data) => {
                                        temps.push(data);
                                        d.resolve();
                                    });
                                });
                            });
                        });
                    } else {
                        alert("Talla no seleccionada, seleccione requerimientos y luego análisis");                        
                    }
                    break;
                case "12":
                    var ls = tls.filter((x: any) => x.groupId === 5);
                    if (ls.length !== 0) {
                        psmbs.forEach((psmb: any) => {
                            sps.forEach((sp: any) => {
                                ls.forEach((l: any) => {
                                    tags.push([a, psmb.value, sp.value, l.value].join("_"));
                                    names.push([event.detail.name, psmb.name, sp.name, l.name].join(" "));
                                    var d = $.Deferred();
                                    requests.push(d.promise());
                                    var url = "/ambiental/tldata?a=" + a + "&psmb=" + psmb.value + "&sp=" + sp.value +
                                        "&l=" + l.value + "&start=" + start + "&end=" + end;
                                    $.getJSON(url, (data) => {
                                        temps.push(data);
                                        d.resolve();
                                    });
                                });
                            });
                        });
                    } else {
                        alert("Larva no seleccionada, seleccione requerimientos y luego análisis");                        
                    }
                    break;
                case "13":
                    var ss = tls.filter((x: any) => x.groupId === 7);
                    if (ss.legnth !== 0) {
                        psmbs.forEach((psmb: any) => {
                            sps.forEach((sp: any) => {
                                ss.forEach((s: any) => {
                                    tags.push([a, psmb.value, sp.value, s.value].join("_"));
                                    names.push([event.detail.name, psmb.name, sp.name, s.name].join(" "));
                                    var d = $.Deferred();
                                    requests.push(d.promise());
                                    var url = "/ambiental/tldata?a=" + a + "&psmb=" + psmb.value + "&sp=" + sp.value +
                                        "&s=" + s.value + "&start=" + start + "&end=" + end;
                                    $.getJSON(url, (data) => {
                                        temps.push(data);
                                        d.resolve();
                                    });
                                });
                            });
                        });
                    } else {
                        alert("Sexo no seleccionado, seleccione requerimientos y luego análisis");
                    }
                    break;
                case "14":
                    psmbs.forEach((psmb: any) => {
                        sps.forEach((sp: any) => {
                            tags.push([a, psmb.value, sp.value].join("_"));
                            names.push([event.detail.name, psmb.name, sp.name].join(" "));
                            var d = $.Deferred();
                            requests.push(d.promise());
                            var url = "/ambiental/tldata?a=" + a + "&psmb=" + psmb.value + "&sp=" + sp.value + "&start=" + start + "&end=" + end;
                            $.getJSON(url, (data) => {
                                temps.push(data);
                                d.resolve();
                            });
                        });
                    });
                    break;
                case "15":
                    var er = tls.filter((x: any) => x.groupId === 6);
                    if (er.legnth !== 0) {
                        psmbs.forEach((psmb: any) => {
                            sps.forEach((sp: any) => {
                                er.forEach((s: any) => {
                                    tags.push([a, psmb.value, sp.value, s.value].join("_"));
                                    names.push([event.detail.name, psmb.name, sp.name, s.name].join(" "));
                                    var d = $.Deferred();
                                    requests.push(d.promise());
                                    var url = "/ambiental/tldata?a=" + a + "&psmb=" + psmb.value + "&sp=" + sp.value +
                                        "&rs=" + s.value + "&start=" + start + "&end=" + end;
                                    $.getJSON(url, (data) => {
                                        temps.push(data);
                                        d.resolve();
                                    });
                                });
                            });
                        });
                    } else {
                        alert("Estado reproductivo no seleccionado, seleccione requerimientos y luego análisis");
                    }
                    break;
                case "16":
                    ss = tls.filter((x: any) => x.groupId === 7);
                    if (ss.legnth !== 0) {
                        psmbs.forEach((psmb: any) => {
                            sps.forEach((sp: any) => {
                                ss.forEach((s: any) => {
                                    tags.push([a, psmb.value, sp.value, s.value].join("_"));
                                    names.push([event.detail.name, psmb.name, sp.name, s.name].join(" "));
                                    var d = $.Deferred();
                                    requests.push(d.promise());
                                    var url = "/ambiental/tldata?a=" + a + "&psmb=" + psmb.value + "&sp=" + sp.value +
                                        "&s=" + s.value + "&start=" + start + "&end=" + end;
                                    $.getJSON(url, (data) => {
                                        temps.push(data);
                                        d.resolve();
                                    });
                                });
                            });
                        });
                    } else {
                        alert("Sexo no seleccionado, seleccione requerimientos y luego análisis");
                    }
                    break;
                case "17":
                    psmbs.forEach((psmb: any) => {
                        sps.forEach((sp: any) => {
                            tags.push([a, psmb.value, sp.value].join("_"));
                            names.push([event.detail.name, psmb.name, sp.name].join(" "));
                            var d = $.Deferred();
                            requests.push(d.promise());
                            var url = "/ambiental/tldata?a=" + a + "&psmb=" + psmb.value + "&sp=" + sp.value + "&start=" + start + "&end=" + end;
                            $.getJSON(url, (data) => {
                                temps.push(data);
                                d.resolve();
                            });
                        });
                    });
                    break;
            }
            $.when.apply(null, requests).done(() => {
                if (temps.length !== 0)
                    temp = temps;
                console.log(temps);
                console.log(tags[0]);
                $.each(chart4.data, (j) => {
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
        } else {
            tl.removeActiveItemsByValue(a);
            alert("PSMB y especie no seleccionados, seleccione requerimientos y luego análisis");
        }
    }
}, false);
etl.addEventListener('removeItem', (event: any) => {
    if (event.detail.groupValue === "Análisis") {
        var a = event.detail.value;
        var tls = tl.getValue();
        var psmbs = tls.filter((x: any) => x.groupId === 2);
        var sps = tls.filter((x: any) => x.groupId === 3);
        if (psmbs.length !== 0 && sps.length !== 0) {
            psmb.disable();
            variables.disable();
            tl.disable();
            switch (a) {
                case "11":
                    var ts = tls.filter((x: any) => x.groupId === 4);
                    if (ts.length !== 0) {
                        psmbs.forEach((psmb: any) => {
                            sps.forEach((sp: any) => {
                                ts.forEach((t: any) => {
                                    var name = [a, psmb.value, sp.value, t.value].join("_");
                                    chart4.series.removeIndex(chart4.series.indexOf(series[name])).dispose();
                                });
                            });
                        });
                    }
                    break;
                case "12":
                    var ls = tls.filter((x: any) => x.groupId === 5);
                    if (ls.length !== 0) {
                        psmbs.forEach((psmb: any) => {
                            sps.forEach((sp: any) => {
                                ls.forEach((l: any) => {
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
let tl = new Choices(etl, {
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
    maxItemText: (maxItemCount: any) => {
        return `Máximo ${maxItemCount} valores`;
    },
    fuseOptions: {
        include: 'score'
    }
});

//.ajax(function (callback) {
//if (semaforo) {
//    fetch('/ambiental/tllist')
//        .then(function (response) {
//            response.json().then(function (data) {
//                callback(data, "value", "label", false);
//            });
//        })
//        .catch(function (error) {
//            console.error(error);
//        });
//}
//});

tl.setChoices(async () => await fetch('/ambiental/tllist')
    .then(items => items.json())
    .catch(err => console.log(err))
);
//tl.setChoices(async function () {
//    const items = await fetch('/ambiental/tllist');
//    return items.json();
//});
//DOWNLOAD EXCEL
//function getXlsx() {
//    alasql.promise('SELECT * INTO XLSX("graphData.xlsx",{headers:true}) FROM ?', [chart.data])
//        .then(function (_data) {
//            console.log('Data saved');
//        }).catch(function (err) {
//            console.log('Error:', err);
//        });
//}

//MAP
function Area(array: any) {
    var n = google.maps.geometry.spherical.computeArea(array) / 10000;
    return parseFloat(n.toString()).toFixed(2);
}
var addListenerOnPolygon = function (polygon: any) {
    var selected = 'red';
    google.maps.event.addListener(polygon, 'click', function (e) {
        if ($.isEmptyObject(e)) {
            psmb.getValue(true).includes(this.zIndex.toString()) ?
                this.setOptions({ fillColor: selected, strokeColor: selected }) :
                this.setOptions({ fillColor: undefined, strokeColor: undefined });
        } else {
            if (psmb.getValue(true).includes(this.zIndex.toString())) {
                this.setOptions({ fillColor: undefined, strokeColor: undefined });
                psmb.removeActiveItemsByValue(this.zIndex.toString());
            } else {
                this.setOptions({ fillColor: selected, strokeColor: selected });
                psmb.setChoiceByValue(this.zIndex.toString());
            }
        }
    });
};
function getBounds(positions: any[]) {
    var bounds = new google.maps.LatLngBounds();
    if (Array.isArray(positions[0])) {
        $.each(positions, (_c, position) => {
            $.each(position, (_c, p) => {
                bounds.extend(p);
            });
        });
    } else {
        $.each(positions, (_c, position) => {
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
var polygons: any = {};
var markers: google.maps.Marker[] = [];
window.onload = function initMap() {
    $.getJSON("/ambiental/mapdata", function getData(data) {
        var bounds = new google.maps.LatLngBounds();
        $.each(data.slice(0, 3), (_i, dato) => {
            $.each(dato.position, (_c, position) => {
                bounds.extend(position);
            });
        });
        map.fitBounds(bounds);
        map.setCenter(bounds.getCenter());
        $.each(data, (i, dato) => {
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
            google.maps.event.addListener(marker, 'click', ((marker, _m) => {
                return () => {
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