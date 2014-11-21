(function ($) {
    var _hideStreetView = function (container) {
        container.gmap('get', 'map').getStreetView().setVisible(false);
    };

    var _changeZoom = function (container, zoomLevel) {
        container.gmap('option', 'zoom', zoomLevel);
    };

    var _addMarkerToMap = function (container, position, zoomLevel) {
        if (zoomLevel) {
            _changeZoom(container, zoomLevel);
        }

        container.gmap('clear', 'markers');
        _hideStreetView(container);
        var marker = container.gmap('addMarker', {
            'position': position,
            'draggable': false,
            'bounds': false
        });
        container.gmap('get', 'map').setOptions({ 'center': position });
    };

    var _initializeMap = function (container, possition, zoomLevel) {
        var that = this;
        var mapOptions = {
            'minZoom': 2,
            'zoom': zoomLevel,
            'maxZoom': 16,
            'panControl': false
        };
        container.gmap(mapOptions).bind('init', function (event, map) {
            if (!possition) {
                if (that._enabled) {
                    that._setCurrentLocation();
                }
            } else {
                _addMarkerToMap(container, possition, zoomLevel);
            }
        });
    };

    var _initializeMapFields = function () {
        var mapWrappers = jQuery('.addressMapWrp');
        var i, addressValue, mapContainer, latlng;
        for (i = 0 ; i < mapWrappers.length; i++) {
            addressValue = $.parseJSON(jQuery(mapWrappers[i]).find('.addressValueInput').attr('value'));
            mapContainer = jQuery(mapWrappers[i]).find('.mapContainer');

            if (mapContainer.gmap('get', 'map') != 'object') {
                latlng = new google.maps.LatLng(addressValue.Latitude, addressValue.Longitude);
                _initializeMap(mapContainer, latlng, addressValue.MapZoomLevel);
            }
        }
    };

    var _refreshMap = function (mapContainer) {
        mapContainer.gmap('refresh');
        var markers = mapContainer.gmap('get', 'markers');
        if (markers.length > 0) {
            var markerPosition = markers[0].position;
            var centerPosition = mapContainer.gmap('get', 'map').center;
            if (markerPosition.kb != centerPosition.kb || markerPosition.lb != centerPosition.lb) {
                mapContainer.gmap('get', 'map').setOptions({ 'center': markerPosition });
            }
        }
    };

    jQuery('.viewMapLnk').last().bind('click', function (e) {
        var mapWrapper = jQuery(e.currentTarget).siblings('.addressMapWrp');
        mapWrapper.toggle();

        if (mapWrapper.is(":visible")) {
            _refreshMap(mapWrapper.find('.mapContainer'));
        }
    });

    _initializeMapFields();

})(jQuery);