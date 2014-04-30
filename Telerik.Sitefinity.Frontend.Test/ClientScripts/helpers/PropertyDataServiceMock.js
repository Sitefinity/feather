controlPropertyServices.factory('PropertyDataServiceMock', function () {
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

    //saves the properties
    saveProperties = function (onsuccess, onerror, modifiedData) {
        if (!modifiedData)
            modifiedData = getChangedProperties();

        initialData = $.extend(true, {}, propertyData);
        onsuccess(modifiedData, status, headers, config);
    };

    //returns the array of properties
    getProperties = function (onsuccess, onerror) {
        if (!propertyData)
            propertyData = {
                "Items": [{ "CategoryName": "Misc", "CategoryNameSafe": "Misc", "ClientPropertyTypeName": "System.String", "ElementCssClass": null, "InCategoryOrdinal": 5, "IsProxy": false, "ItemTypeName": "System.String", "NeedsEditor": false, "PropertyId": "5f7dd477-b259-6671-9d67-ff00001274f9", "PropertyName": "Html", "PropertyPath": "\/Settings\/Html", "PropertyValue": "testValue", "TypeEditor": null },
                    { "CategoryName": "Misc", "CategoryNameSafe": "Misc", "ClientPropertyTypeName": "System.String", "ElementCssClass": null, "InCategoryOrdinal": 5, "IsProxy": false, "ItemTypeName": "System.String", "NeedsEditor": false, "PropertyId": "5f7dd477-b259-6671-9d67-ff00001274f9", "PropertyName": "SharedContentID", "PropertyPath": "\/Settings\/SharedContentID", "PropertyValue": "00000000-0000-5555-0000-000000000000", "TypeEditor": null },
                    { "CategoryName": "Misc", "CategoryNameSafe": "Misc", "ClientPropertyTypeName": "System.String", "ElementCssClass": null, "InCategoryOrdinal": 5, "IsProxy": false, "ItemTypeName": "System.String", "NeedsEditor": false, "PropertyId": "5f7dd477-b259-6671-9d67-ff00001274f9", "PropertyName": "ProviderName", "PropertyPath": "\/Settings\/ProviderName", "PropertyValue": "", "TypeEditor": null }]
            };

        onsuccess(propertyData);
    };

    //reset property changes
    resetPropertyChanges = function () {
        propertyData = $.extend(true, {}, initialData);
    };

    //the public interface of the service
    return {
        getProperties: getProperties,
        saveProperties: saveProperties,
        resetPropertyChanges: resetPropertyChanges
    };
});
