/* global sitefinity, angular, jQuery */

(function ($) {

    var pageEditorServices = angular.module('pageEditorServices', []);

    /**
     * The provider for the widget context currently active within the page editor.
     * This context is used to give information about the widget, such as it's id, the id of
     * the page and so on.
     */
    pageEditorServices.provider('widgetContext', function () {
        var context = sitefinity.pageEditor.widgetContext;

        this.$get = function () {
            return {
                widgetId: context.Id,
                pageId: context.PageId,
                mediaType: context.MediaType,
                culture: context.PropertyValueCulture,
                webServiceUrl: context.PropertyServiceUrl,
                hideSaveAllTranslations: context.HideSaveAllTranslations
            };
        };
    });

    //this is the service responsible for managing the properties data for all interested parties
    pageEditorServices.factory('propertyService', function ($rootScope, $http, $q, widgetContext) {

        var httpGetUrl,
            properties,
            initialData,
            CULTURE_HEADER = 'SF_UI_CULTURE',
            loadingPromise;

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

        /**
         * Constructs and returns the GET URL for the server side web service
         * that returns the properties of a widget.
         */
        var getUrl = function () {
            if (!httpGetUrl) {
                httpGetUrl = widgetContext.webServiceUrl + widgetContext.widgetId + '/';
            }
            return httpGetUrl;
        };

        /**
         * Constructs and returns the PUT URL for the server side web service
         * that returns the properties of a widget.
         */
        var updateUrl = function (saveMode) {
            return widgetContext.webServiceUrl + 'batch/' + widgetContext.widgetId + '/?pageId=' +
                widgetContext.pageId + '&mediaType=' + widgetContext.mediaType + '&propertyLocalization=' + saveMode;
        };

        var toAssociativeArray = function (propertyBag) {
            var widget = {};
            for (var i = 0; i < propertyBag.length; i++) {
                var prop = propertyBag[i];
                widget[prop.PropertyName] = prop;
            }
            return widget;
        };

        //get the property data from the service
        var load = function () {
            if (loadingPromise)
                return loadingPromise;

            var deferred = $q.defer();

            $http.get(getUrl(), requestOptions())
                .success(function (data) {
                    properties = data;
                    //clone the result in initialData
                    initialData = $.extend(true, {}, data);
                    loadingPromise = null;
                    deferred.resolve(data);
                })
                .error(function (data) {
                    loadingPromise = null;
                    deferred.reject(data);
                });

            loadingPromise = deferred.promise;
            return loadingPromise;
        };

        var service = {

            /**
             * Retrieves the properties for the current widget context.
             *
             * @returns {object} promise The promise object that is resolved or rejected once the operation completes.
             */
            get: function () {
                if (!properties) {
                    return load();
                } else {
                    var deferred = $q.defer();
                    deferred.resolve(properties);
                    return deferred.promise;
                }
            },

            /**
             * Sets the values of the in-memory widget properties.
             *
             * @param {Array} newProperties The array of property objects with the new values.
             * @returns {void}
             */
            set: function (newProperties) {
                if (properties) {
                    properties.Items = newProperties;
                }
            },

            /**
             * Saves the properties to the server.
             *
             * @param {Number} saveMode The type of save that should be performed. 0 - default, 1 - all translations, 2 - currently translation only
             * @param {Array} modifiedData Optional. If present only this data will be saved; otherwise all the dirty properties will be saved.
             *
             * @returns {object} promise The promise object that is resolved or rejected once the operation completes.
             */
            save: function (saveMode, modifiedData) {
                if (!modifiedData || modifiedData.length === 0) {
                    modifiedData = this.getDirty();
                }

                var deferred = $q.defer();

                $http.put(updateUrl(saveMode), modifiedData, requestOptions())
                    .success(function (data) {
                        initialData = $.extend(true, {}, properties);
                        deferred.resolve(data);
                    })
                    .error(function (data) {
                        deferred.reject(data);
                    });

                return deferred.promise;
            },

            /**
             * Resets the changes made to properties and restores them to their initial state.
             */
            reset: function () {
                properties = $.extend(true, {}, initialData);
            },

            /**
             * Gets the properties whose state is different than that one of the initial properties.
             */
            getDirty: function () {
                var modifiedData = [];
                if (properties) {
                    for (var i = 0; i < properties.Items.length; i++) {
                        if (properties.Items[i].PropertyValue !== initialData.Items[i].PropertyValue) {
                            modifiedData.push(properties.Items[i]);
                        }
                    }
                }
                return modifiedData;
            },

            /**
             * Converts the property bag that is returned by the "get" method of the service to an associative array.
             *
             * @param {Array} propertyBag 
             */
            toAssociativeArray: function (propertyBag) {
                return toAssociativeArray(propertyBag);
            }
        };

        return service;
    });

})(jQuery);