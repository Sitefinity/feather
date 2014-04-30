//this is the service responsible for managing the properties data for all interested parties
angular.module("controlPropertyServices").factory('PropertyDataService', function ($http, PageControlDataService) {
    var propertyData;
    var initialData;

    //compares the initial data with the current data and returns array which contains only the modified properties
    getChangedProperties = function () {
        var modifiedData = [];
        if (propertyData) {
            for (var i = 0; i < propertyData.Items.length; i++) {
                if (propertyData.Items[i].PropertyValue !== initialData.Items[i].PropertyValue)
                    modifiedData.push(propertyData.Items[i]);
            }
        }
        return modifiedData;
    };

    saveProperties = function (onsuccess, onerror, saveMode, modifiedData) {
        var id = PageControlDataService.data.Id;
        var pageId = PageControlDataService.data.PageId;
        var mediaType = PageControlDataService.data.MediaType;
        var culture = PageControlDataService.data.PropertyValueCulture;
        var cultureHeaders = { 'SF_UI_CULTURE': culture };
        var basePropertyServiceUrl = PageControlDataService.data.PropertyServiceUrl;

        var putUrl = basePropertyServiceUrl + "batch/" + id + "/?pageId=" + pageId + "&mediaType=" + mediaType + "&propertyLocalization=" + saveMode;

        if (!modifiedData || modifiedData.length == 0)
            modifiedData = this.getChangedProperties();
        return $http.put(putUrl, modifiedData, { headers: cultureHeaders })
            .success(
                function (xhrData, status, headers, config) {
                    initialData = $.extend(true, {}, propertyData);
                    onsuccess(xhrData, status, headers, config);
                })
            .error(onerror);
    };

    //get the property data from the service
    getDataFromService = function (onsuccess, onerror) {
        var controlId = PageControlDataService.data.Id;
        var culture = PageControlDataService.data.PropertyValueCulture;
        var cultureHeaders = { 'SF_UI_CULTURE': culture };
        var basePropertyServiceUrl = PageControlDataService.data.PropertyServiceUrl;

        var getUrl = basePropertyServiceUrl + controlId + "/";

        return $http.get(getUrl, { cache: false, headers: cultureHeaders })
            .success(
            function (data) {
                propertyData = data;
                //clone the result in initialData
                initialData = $.extend(true, {}, data);
                onsuccess(data);
            })
            .error(onerror);
    };

    //returns the array of properties
    getProperties = function (onsuccess, onerror) {
        if (!propertyData)
            getDataFromService(onsuccess, onerror);
        else
            onsuccess(propertyData);
    };

    //explicitly set local properties
    setProperties = function (newProperties) {
        if (propertyData)
            propertyData.Items = newProperties;
    };

    //reset property changes
    resetPropertyChanges = function () {
        propertyData = $.extend(true, {}, initialData);
    };

    //the public interface of the service
    return {
        getProperties: getProperties,
        setProperties: setProperties,
        saveProperties: saveProperties,
        resetPropertyChanges: resetPropertyChanges,
        getChangedProperties: getChangedProperties
    };
});
