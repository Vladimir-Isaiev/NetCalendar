$('#myModalMap').on('shown.bs.modal', function (e) {
    GetMap();
})

var map;
var markerDestination = null;
var markerStart = null;
var Kiev;

var directionsDisplay;
var directionsService;


var geocoder = new google.maps.Geocoder;
var infowindow = new google.maps.InfoWindow;


function GetMap() {
    var lat = parseFloat($('#hdDestinationLat').val());
    var lon = parseFloat($('#hdDestinationLong').val());
    markerStart = null;

    if (lat == 0) {
        markerDestination = null;
    }

    Kiev = new google.maps.LatLng(50.4501, 30.5234);
    directionsDisplay = new google.maps.DirectionsRenderer();
    directionsService = new google.maps.DirectionsService();
    google.maps.visualRefresh = true;

    var mapOptions = {
        zoom: 12,
        center: Kiev,
        mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
    };

    map = new google.maps.Map(document.getElementById("canvas"), mapOptions);

    directionsDisplay.setMap(map);
    directionsDisplay.setOptions({ suppressMarkers: true, suppressInfoWindows: true });

    map.addListener('click', function (e) {
        placeMarkerAndPanTo(e.latLng, map);
    });

    if (lat != 0) {
        var geoDest = new google.maps.LatLng(lat, lon);

        markerDestination = new google.maps.Marker({
            position: geoDest,
            map: map,
            title: "Destination",
            draggable: true
        });
        map.panTo(geoDest);
    }

}


    function placeMarkerAndPanTo(latLng, map) {
        if (markerDestination == null) {
            markerDestination = new google.maps.Marker({
                position: latLng,
                map: map,
                title: "Destination",
                draggable: true
            });
            
            getAdress(markerDestination);
            
            $('#hdDestinationLat').val(markerDestination.position.lat());
            $('#hdDestinationLong').val(markerDestination.position.lng());
            return;
        } else {
            if (markerStart == null) {
                markerStart = new google.maps.Marker({
                    position: latLng,
                    map: map,
                    'title': "My Place",
                    'draggable': true
                });
                drawRoute(markerDestination.position, markerStart.position);
                return;
            }
        }
        markerDestination = null;
        markerStart = null;
        GetMap();
}



    function getAdress(markerDestination) {
        geocoder.geocode({ 'location': markerDestination.position }, function (results, status) {
            
            if (status === 'OK') {
                if (results[0]) {
                    infowindow.setContent(results[0].formatted_address);
                    infowindow.open(map, markerDestination);
                    $("#adr").val(results[0].formatted_address);
                } else {
                    $("#adr").val(" ");
                }
            } else {
                $("#adr").val(" ");
            }
        });

    }



    function drawRoute(start, end) {
        var request = {
            origin: start,
            destination: end,
            travelMode: google.maps.TravelMode.DRIVING,
            unitSystem: google.maps.UnitSystem.METRIC,
            waypoints: [
                {
                    location: start,
                    stopover: false
                }, {
                    location: end,
                    stopover: true
                }
            ],
            optimizeWaypoints: true,
            provideRouteAlternatives: true,
            avoidHighways: false,
            avoidTolls: true
        };

        directionsService.route(request, function (result, status) {
            if (status == google.maps.DirectionsStatus.OK) {
                directionsDisplay.setDirections(result);
                var routes = result.routes;
            }
        });

    }
