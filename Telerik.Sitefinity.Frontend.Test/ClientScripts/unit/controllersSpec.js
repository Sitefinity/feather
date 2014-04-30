//'use strict';

///* tests for the controlPropertiesServices */
//describe('ControlPropertyServices tests.', function () {

//    beforeEach(module('controlPropertyServices'));
//    var $httpBackend;

//    beforeEach(inject(function ($injector) {
//        // Set up the mock http service responses
//        $httpBackend = $injector.get('$httpBackend');

//    }));

//    it('Ensure the properties are initially requested.', inject(function (PropertyDataService) {
//        $httpBackend.expectGET('/Sitefinity/Services/Pages/ControlPropertyService.svc/902033f3-aceb-6a2e-a9a3-ff0000ffeb85/');
//        PropertyDataService.getProperties();

//    }));

//    it('Ensure put call is executed.', inject(function (PropertyDataService) {
//        $httpBackend.expectPUT('/Sitefinity/Services/Pages/ControlPropertyService.svc/batch/902033f3-aceb-6a2e-a9a3-ff0000ffeb85/?pageId=111033f3-aceb-6a2e-a9a3-ff0000ffeb85&mediaType=Page');
//        PropertyDataService.saveProperties();
//    }));

//});


///* tests for the advancedDesignerModule */
//describe('Advanced designer tests.', function () {
//    beforeEach(module('advancedDesignerModule'));

//    describe('PropertyHierarchy filter tests.', function () {
//        it('Should return all non-proxy properties when the first property is a proxy',
//            inject(function (propertyHierarchyFilter) {
//                var inputArray = [{ "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "Settings", "PropertyPath": "/Settings", "ItemTypeName": "System.Object", "ClientPropertyTypeName": "System.Object", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": null, "NeedsEditor": false, "IsProxy": true, "TypeEditor": null }, { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "ReadTemplateName", "PropertyPath": "/Settings/ReadTemplateName", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "Special", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "006" }, { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "Message", "PropertyPath": "/Settings/Message", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "Hello, World!!!", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "007" }, { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "AsyncManager", "PropertyPath": "/Settings/AsyncManager", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "008" }, { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "Profile", "PropertyPath": "/Settings/Profile", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "009" }, { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "ViewEngineCollection", "PropertyPath": "/Settings/ViewEngineCollection", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "00A" }];

//                expect(propertyHierarchyFilter(inputArray).length).toEqual(inputArray.length - 1);
//            }));

//        it('Should return all only the properties from the same level as requested property',
//            inject(function (propertyHierarchyFilter) {
//                var inputArray = [{ "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "prop1", "PropertyPath": "/First/Second/prop1", "ItemTypeName": "System.Object", "ClientPropertyTypeName": "System.Object", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": null, "NeedsEditor": false, "IsProxy": false, "TypeEditor": null }, 
//                    { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "prop2", "PropertyPath": "/First/Second/prop2", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "Special", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "006" }, 
//                    { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "Second", "PropertyPath": "/First/Second", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "Hello, World!!!", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "007" },
//                    { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "prop4", "PropertyPath": "/First/Second/prop4", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "008" }, 
//                    { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "prop5", "PropertyPath": "/prop5", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "009" },
//                    { "PropertyId": "a5775131-8df5-6bde-b650-ff000086cdf6", "PropertyName": "prop6", "PropertyPath": "/First/Second/Third/prop4", "ItemTypeName": "System.String", "ClientPropertyTypeName": "System.String", "InCategoryOrdinal": 5, "ElementCssClass": null, "CategoryName": "Misc", "CategoryNameSafe": "Misc", "PropertyValue": "", "NeedsEditor": false, "IsProxy": false, "TypeEditor": null, "$$hashKey": "00A" }];
                
//                var filteredResult = propertyHierarchyFilter(inputArray, "Second", "/First/Second");
//                expect(filteredResult.length).toEqual(3);
//                for(var i= 0;i<filteredResult.length;i++)
//                    expect(filteredResult[i].PropertyPath).toMatch('First/Second/');
//            }));
//    });

//    describe('Advanced designer view controller test.', function () {
//        var $httpBackend, $rootScope, createController, propertyDataService;

//        beforeEach(inject(function ($injector) {

//            // Get hold of a scope (i.e. the root scope)
//            $rootScope = $injector.get('$rootScope');
//            $rootScope.ShowSimpleButton = function (event) {};

//            // The $controller service is used to create instances of controllers
//            var $controller = $injector.get('$controller');

//            var propertyDataService = $injector.get("PropertyDataServiceMock");

//            createController = function () {
//                return $controller('AdvancedDesignerModuleCtrl', {
//                    '$scope': $rootScope, 'PropertyDataService': propertyDataService,
//                });
//            };
//        }));

//        it('The items are populated correctly from the angular service.', function () {
//            var controller = createController();
//            expect($rootScope.Items).not.toBeNull();
//            expect($rootScope.Items.length).toEqual(3);
//            expect($rootScope.Items[0].PropertyName).toEqual("Html");
//            expect($rootScope.Items[0].PropertyValue).toEqual("testValue");
//            expect($rootScope.Items[1].PropertyName).toEqual("SharedContentID");
//            expect($rootScope.Items[1].PropertyValue).toEqual("00000000-0000-5555-0000-000000000000");
//        });

//    });
//});

///* tests for the BreadCrumb */
//describe('BreadCrumb directive test', function() {
//    var element;
//    var scope;

//    beforeEach(module('breadCrumbModule'));

//    beforeEach(inject(function($rootScope, $compile) {
//        element = angular.element('<ul ng-model="propertyPath" breadcrumb></ul>');
//        var compile = $compile(element);
//        compile($rootScope);
//        $rootScope.RefreshHierarchy("Level0/Level1/Level2", "Level2");
//        $rootScope.$digest();
//        scope=$rootScope;
//    }));
    
//    it('Can refreshes the whole breadcrumb content.', inject(function () {
//        expect(element.text()).toContain('Level0');
//        expect(element.text()).toContain('Level1');
//        expect(element.text()).toContain('Level2');
//    }));

//    it('Adds single element to the breadcrumb when proeprtyPath has been changed', inject(function () {
//        scope.propertyPath = "Level0/Level1/Level2/Level3";
//        scope.$digest();
//        expect(element.text()).toContain('Level0');
//        expect(element.text()).toContain('Level1');
//        expect(element.text()).toContain('Level2');
//        expect(element.text()).toContain('Level3');
//    }));
//});