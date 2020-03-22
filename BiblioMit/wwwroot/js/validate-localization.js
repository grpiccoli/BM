(function ($) {
    var culture = navigator.language.split('-')[0];
    $.when($.get("/lib/cldr-data/supplemental/likelySubtags.json"), $.get("/lib/cldr-data/main/" + culture + "/numbers.json"), $.get("/lib/cldr-data/supplemental/numberingSystems.json"), $.get("/lib/cldr-data/main/" + culture + "/ca-gregorian.json"), $.get("/lib/cldr-data/main/" + culture + "/timeZoneNames.json"), $.get("/lib/cldr-data/supplemental/timeData.json"), $.get("/lib/cldr-data/supplemental/weekData.json")).then(function () {
        return [].slice.apply(arguments, [0]).map(function (result) {
            return result[0];
        });
    }).then(Globalize.load).then(function () {
        if (culture) {
            Globalize.locale(culture);
        }
    });
})(jQuery);
//# sourceMappingURL=validate-localization.js.map