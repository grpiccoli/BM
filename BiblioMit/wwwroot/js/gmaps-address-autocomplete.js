google.maps.event.addDomListener(window, 'load', function () {
    var options = {
        types: ['address'],
        componentRestrictions: { country: "cl" }
    };
    var input = document.getElementById('Address');
    new google.maps.places.Autocomplete(input, options);
});
//# sourceMappingURL=gmaps-address-autocomplete.js.map