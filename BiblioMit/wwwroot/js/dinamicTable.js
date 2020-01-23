$(document).ready(adjustDT);
$(window).resize(adjustDT);
function adjustDT() {
    var tot = $(window).height() - $('.beforeDT').height() - $('footer').height() - $('.navbar').height() - $("#constrainer2 thead").height() - 60;
    console.log(tot);
    $("#constrainer2 tbody").height(tot);
}