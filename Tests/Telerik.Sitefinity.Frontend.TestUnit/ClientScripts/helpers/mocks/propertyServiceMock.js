angular.module('pageEditorServices').factory('PropertyServiceMock', function ($rootScope, $q) {

        var properties,
            initialData,
            loadingPromise;

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

            properties = {
                "Items": [{ "CategoryName": "Misc", "CategoryNameSafe": "Misc", "ClientPropertyTypeName": "System.String", "ElementCssClass": null, "InCategoryOrdinal": 5, "IsProxy": false, "ItemTypeName": "System.String", "NeedsEditor": false, "PropertyId": "5f7dd477-b259-6671-9d67-ff00001274f9", "PropertyName": "Html", "PropertyPath": "\/Settings\/Html", "PropertyValue": "testValue", "TypeEditor": null },
                    { "CategoryName": "Misc", "CategoryNameSafe": "Misc", "ClientPropertyTypeName": "System.String", "ElementCssClass": null, "InCategoryOrdinal": 5, "IsProxy": false, "ItemTypeName": "System.String", "NeedsEditor": false, "PropertyId": "5f7dd477-b259-6671-9d67-ff00001274f9", "PropertyName": "SharedContentID", "PropertyPath": "\/Settings\/SharedContentID", "PropertyValue": "00000000-0000-5555-0000-000000000000", "TypeEditor": null },
                    { "CategoryName": "Misc", "CategoryNameSafe": "Misc", "ClientPropertyTypeName": "System.String", "ElementCssClass": null, "InCategoryOrdinal": 5, "IsProxy": false, "ItemTypeName": "System.String", "NeedsEditor": false, "PropertyId": "5f7dd477-b259-6671-9d67-ff00001274f9", "PropertyName": "ProviderName", "PropertyPath": "\/Settings\/ProviderName", "PropertyValue": "", "TypeEditor": null }]
            };

            initialData = $.extend(true, {}, properties);
            loadingPromise = null;
            deferred.resolve(properties);

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
              
                initialData = $.extend(true, {}, properties);
                deferred.resolve(modifiedData);

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
