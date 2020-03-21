google.maps.event.addDomListener(window, 'load', () => {
    var options = {
        types: ['address'],
        componentRestrictions: { country: "cl" }
    };
    var input = document.getElementById('Address') as HTMLInputElement;
    new google.maps.places.Autocomplete(input, options);
});