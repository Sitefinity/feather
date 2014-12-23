/* Tests for selected items view */
describe("selected items view", function () {
    var scope;

    var allItems = [
        { Title: "title 1", Id: "4c003fb0-2a77-61ec-be54-ff00007864f4", Name: "name 1", Type: { Value: "type 1" } },
        { Title: "title 2", Id: "4c003fb0-2a77-61ec-be54-ff00007864f3", Name: "name 2", Type: { Value: "type 2" } },
        { Title: "title 3", Id: "4c003fb0-2a77-61ec-be54-ff10007864f4", Name: "name 3", Type: { Value: "type 3" } }
    ];

    //This is the id of the cached templates in $templateCache. The external templates are cached by a karma/grunt preprocessor.
    var selectedItemsViewTemplatePath = 'client-components/selectors/common/sf-selected-items-view.html';

    //Load the module under test.
    beforeEach(module('sfSelectors'));

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));

    beforeEach(module(function ($provide) {
        var serverContext = {
            getRootedUrl: function (path) {
                return appPath + '/' + path;
            },
            getUICulture: function () {
                return null;
            },
            getEmbeddedResourceUrl: function (assembly, url) {
                return url;
            },
            getFrontendLanguages: function () {
                return ['en', 'de'];
            }
        };
        $provide.value('serverContext', serverContext);
    }));

    beforeEach(inject(function ($rootScope, $httpBackend, _$q_, $templateCache, _$timeout_) {
        //Build the scope with whom the directive will be compiled.
        scope = $rootScope.$new();
        $httpBackend.whenGET(selectedItemsViewTemplatePath);
    }));

    it('[NPetrova] / should copy allItems in currentItems collection.', function () {
        scope.allItems = allItems;
        var template = "<sf-selected-items-view sf-items='allItems'></sf-selected-items-view>";

        commonMethods.compileDirective(template, scope);
        var s = scope.$$childHead;

        expect(s.currentItems).toBeDefined();
        expect(s.sfItems.length).toBe(scope.allItems.length);
        expect(s.currentItems.length).toBe(scope.allItems.length);
        for (var i = 0; i < scope.allItems.length; i++) {
            expect(s.sfItems[i].Id).toBe(scope.allItems[i].Id);
            expect(s.currentItems[i].Id).toBe(scope.allItems[i].Id);
        }
    });

    it('[NPetrova] / should clear currentItems if passed allItems is empty array.', function () {
        scope.allItems = allItems;
        var template = "<sf-selected-items-view sf-items='allItems'></sf-selected-items-view>";

        commonMethods.compileDirective(template, scope);
        var s = scope.$$childHead;

        expect(s.currentItems).toBeDefined();
        expect(s.sfItems.length).toBe(scope.allItems.length);
        expect(s.currentItems.length).toBe(scope.allItems.length);

        scope.allItems = [];
        commonMethods.compileDirective(template, scope);

        expect(s.currentItems).toBeDefined();
        expect(s.currentItems.length).toBe(0);
    });

    it('[NPetrova] / should apply custom identifier field.', function () {
        scope.allItems = allItems;
        scope.identifierField = "Name";
        var template = "<sf-selected-items-view sf-items='allItems' sf-identifier-field='identifierField'></sf-selected-items-view>";

        commonMethods.compileDirective(template, scope);
        var s = scope.$$childHead;

        var item = s.sfItems[0];

        expect(s.bindIdentifierField(item)).toBe(scope.allItems[0].Name);
        expect(s.bindIdentifierField(item)).not.toBe(scope.allItems[0].Title);

    });

    it('[NPetrova] / should return item\'s id if invalid identifier is applied.', function () {
        scope.allItems = allItems;
        scope.identifierField = "Invalid";
        var template = "<sf-selected-items-view sf-items='allItems' sf-identifier-field='identifierField'></sf-selected-items-view>";

        commonMethods.compileDirective(template, scope);
        var s = scope.$$childHead;

        var item = s.sfItems[1];

        expect(s.bindIdentifierField(item)).toBe(scope.allItems[1].Id);

    });

    it('[NPetrova] / should apply custom identifier if the identifier is an object with property Value.', function () {
        scope.allItems = allItems;
        scope.identifierField = "Type";
        var template = "<sf-selected-items-view sf-items='allItems' sf-identifier-field='identifierField'></sf-selected-items-view>";

        commonMethods.compileDirective(template, scope);
        var s = scope.$$childHead;

        var item = s.sfItems[2];

        expect(s.bindIdentifierField(item)).toBe(scope.allItems[2].Type.Value);

    });

    it('[NPetrova] / should unselect selected item if the item is clicked.', function () {
        var selectedItems = [];
        selectedItems.push(allItems[0]);

        scope.allItems = allItems;
        scope.selectedItems = selectedItems;
        var template = "<sf-selected-items-view sf-items='allItems' sf-selected-items='selectedItems'></sf-selected-items-view>";

        commonMethods.compileDirective(template, scope);
        var s = scope.$$childHead;

        var item = scope.selectedItems[0];

        expect(s.sfSelectedItems).toBeDefined();
        expect(s.sfSelectedItems.length).toBe(1);
        expect(s.isItemSelected(item.Id)).toBe(true);

        s.itemClicked(item);

        expect(s.sfSelectedItems).toBeDefined();
        expect(s.sfSelectedItems.length).toBe(0);
        expect(s.isItemSelected(item.Id)).toBe(false);
    });

    it('[NPetrova] / should select item on item click if no items are selected.', function () {
        scope.allItems = allItems;
        var template = "<sf-selected-items-view sf-items='allItems'></sf-selected-items-view>";

        commonMethods.compileDirective(template, scope);
        var s = scope.$$childHead;

        var item = scope.allItems[1];

        expect(s.sfSelectedItems).not.toBeDefined();
        expect(s.isItemSelected(item.Id)).toBe(false);

        s.itemClicked(item);

        expect(s.sfSelectedItems).toBeDefined();
        expect(s.sfSelectedItems.length).toBe(1);
        expect(s.isItemSelected(item.Id)).toBe(true);
    });

    it('[NPetrova] / should select unselected item if the item is clicked.', function () {
        var selectedItems = [];
        selectedItems.push(allItems[0]);

        scope.allItems = allItems;
        scope.selectedItems = selectedItems;
        var template = "<sf-selected-items-view sf-items='allItems' sf-selected-items='selectedItems'></sf-selected-items-view>";

        commonMethods.compileDirective(template, scope);
        var s = scope.$$childHead;

        var selectedItem = scope.selectedItems[0];
        var unselectedItem = scope.allItems[2];

        expect(s.sfSelectedItems).toBeDefined();
        expect(s.sfSelectedItems.length).toBe(1);
        expect(s.isItemSelected(selectedItem.Id)).toBe(true);
        expect(s.isItemSelected(unselectedItem.Id)).toBe(false);

        s.itemClicked(unselectedItem);

        expect(s.sfSelectedItems).toBeDefined();
        expect(s.sfSelectedItems.length).toBe(2);
        expect(s.isItemSelected(selectedItem.Id)).toBe(true);
        expect(s.isItemSelected(unselectedItem.Id)).toBe(true);
    });

    it('[NPetrova] / should select unselected item and unselect selected if the items are clicked.', function () {
        var selectedItems = [];
        selectedItems.push(allItems[0]);

        scope.allItems = allItems;
        scope.selectedItems = selectedItems;
        var template = "<sf-selected-items-view sf-items='allItems' sf-selected-items='selectedItems'></sf-selected-items-view>";

        commonMethods.compileDirective(template, scope);
        var s = scope.$$childHead;

        var selectedItem = scope.selectedItems[0];
        var unselectedItem = scope.allItems[2];

        expect(s.sfSelectedItems).toBeDefined();
        expect(s.sfSelectedItems.length).toBe(1);
        expect(s.isItemSelected(selectedItem.Id)).toBe(true);
        expect(s.isItemSelected(unselectedItem.Id)).toBe(false);

        s.itemClicked(unselectedItem);
        s.itemClicked(selectedItem);

        expect(s.sfSelectedItems).toBeDefined();
        expect(s.sfSelectedItems.length).toBe(1);
        expect(s.isItemSelected(selectedItem.Id)).toBe(false);
        expect(s.isItemSelected(unselectedItem.Id)).toBe(true);
    });


});