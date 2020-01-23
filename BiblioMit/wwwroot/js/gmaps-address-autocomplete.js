google.maps.event.addDomListener(window, 'load', function () {
    var options = {
        types: ['address'],
        componentRestrictions: { country: "cl" }
    };
    var input = document.getElementById('Address');
    var places = new google.maps.places.Autocomplete(input, options);
});