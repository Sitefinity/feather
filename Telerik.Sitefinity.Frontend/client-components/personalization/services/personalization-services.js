(function ($) {
    var personalizationServices = angular.module('personalizationServices', ['pageEditorServices']);

    //this is the service responsible for managing the properties data for all interested parties
    personalizationServices.factory('personalizationService', ['$http', '$q', 'widgetContext',
		function ($http, $q, widgetContext) {
		    var CULTURE_HEADER = 'SF_UI_CULTURE';

		    /**
		     * Generates the headers dictionary for the HTTP request
		     * to be performed. The headers will contain by default the
		     * Sitefinity UI culture header.
		     */
		    var requestOptions = function () {
		        var header = {};

		        if (widgetContext.culture)
		            header[CULTURE_HEADER] = widgetContext.culture;

		        return {
		            cache: false,
		            headers: header
		        };
		    };

		    var getServiceUrl = function () {
		        return sitefinity.services.getPersonalizationServiceUrl();
		    };

		    var getSegments = function () {
		        var deferred = $q.defer(),
			        url = getServiceUrl() + "segments/";
		        var reqOptions = requestOptions();
		        reqOptions.params = {
		            "ControlId": widgetContext.widgetId
		        };

		        $http.get(url, reqOptions)
				    .success(function (data) {
				        deferred.resolve(data);
				    })
				    .error(function (data) {
				        deferred.reject(data);
				    });

		        return deferred.promise;
		    };

		    var personalize = function (model) {
		        var deferred = $q.defer(),
			        url = getServiceUrl();


		        var data = {
		            "ControlId": widgetContext.widgetId,
		            "SegmentId": model.segmentId
		        };

		        $http.post(url, data, requestOptions())
				    .success(function (data) {
				        deferred.resolve({ Id: data, SegmentName: model.segmentName });
				    })
				    .error(function (data) {
				        deferred.reject(data);
				    });

		        return deferred.promise;
		    };

		    //the public interface of the service
		    return {
		        getSegments: getSegments,
		        personalize: personalize
		    };
		}
    ]);

})(jQuery);