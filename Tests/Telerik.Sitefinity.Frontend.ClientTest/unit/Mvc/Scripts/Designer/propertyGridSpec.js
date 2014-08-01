/* tests for the PropertyGridModule */
describe('PropertyGridModule tests.', function () {
    beforeEach(module('PropertyGridModule'));

    describe('Controllers test.', function () {
        var $httpBackend, $rootScope, createController, propertyService;

        beforeEach(inject(function ($injector) {

            // Get hold of a scope (i.e. the root scope)
            $rootScope = $injector.get('$rootScope');

            // The $controller service is used to create instances of controllers
            var $controller = $injector.get('$controller');

            var propertyService = $injector.get("PropertyServiceMock");

            createController = function () {
                return $controller('PropertyGridCtrl', {
                    '$scope': $rootScope, 'propertyService': propertyService,
                });
            };
        }));

        it('[EGaneva] / The items are populated correctly from the angular service.', function () {
            var controller = createController();
            $rootScope.$digest();

            expect($rootScope.items).not.toBeNull();
            expect($rootScope.items.length).toEqual(3);
            expect($rootScope.items[0].PropertyName).toEqual("Html");
            expect($rootScope.items[0].PropertyValue).toEqual("testValue");
            expect($rootScope.items[1].PropertyName).toEqual("SharedContentID");
            expect($rootScope.items[1].PropertyValue).toEqual("00000000-0000-5555-0000-000000000000");
        });
    });

    describe('Filters tests.', function () {
        it('[EGaneva] / Should return all non-proxy properties when the first property is a proxy',
            inject(function (propertyHierarchyFilter) {
                var inputArray = [{ "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "Settings", "PropertyPath": "/Settings", "ItemTypeName": "System.Object", "ClientPropertyTypeName": "System.Object", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": null, "NeedsEditor": false, "IsProxy": true, "TypeEditor": null }, { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "ReadTemplateName", "PropertyPath": "/Settings/ReadTemplateName", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "Special", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "006" }, { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "Message", "PropertyPath": "/Settings/Message", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "Hello, World!!!", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "007" }, { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "AsyncManager", "PropertyPath": "/Settings/AsyncManager", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "008" }, { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "Profile", "PropertyPath": "/Settings/Profile", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "009" }, { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "ViewEngineCollection", "PropertyPath": "/Settings/ViewEngineCollection", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "00A" }];

                expect(propertyHierarchyFilter(inputArray).length).toEqual(inputArray.length - 1);
            }));

        it('[EGaneva] / Should return all only the properties from the same level as requested property',
            inject(function (propertyHierarchyFilter) {
                var inputArray = [{ "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "prop1", "PropertyPath": "/First/Second/prop1", "ItemTypeName": "System.Object", "ClientPropertyTypeName": "System.Object", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": null, "NeedsEditor": false, "IsProxy": false, "TypeEditor": null }, 
                    { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "prop2", "PropertyPath": "/First/Second/prop2", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "Special", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "006" }, 
                    { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "Second", "PropertyPath": "/First/Second", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "Hello, World!!!", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "007" },
                    { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "prop4", "PropertyPath": "/First/Second/prop4", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "008" }, 
                    { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "prop5", "PropertyPath": "/prop5", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "009" },
                    { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "prop6", "PropertyPath": "/First/Second/Third/prop4", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "00A" }];
                
                var filteredResult = propertyHierarchyFilter(inputArray, "Second", "/First/Second");
                expect(filteredResult.length).toEqual(3);
                for(var i= 0;i<filteredResult.length;i++)
                    expect(filteredResult[i].PropertyPath).toMatch('First/Second/');
            }));
    });
});
