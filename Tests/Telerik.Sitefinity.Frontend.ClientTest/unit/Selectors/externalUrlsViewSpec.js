/* Tests for external urls view */
describe("external urls view", function () {
    var scope;

    var sfExternalPages = [
            { TitlesPath: "title 1", ExternalPageId: "4c003fb0-2a77-61ec-be54-ff00007864f4", Url: "url 1" },
            { TitlesPath: "title 2", ExternalPageId: "4c003fb0-2a77-61ec-be54-ff00007864f3", Url: "url 2" },
            { TitlesPath: "title 3", ExternalPageId: "4c003fb0-2a77-61ec-be54-ff10007864f5", Url: "url 3" },
            { TitlesPath: "title 4", ExternalPageId: "4c003fb0-2a77-61ec-be54-ff10007864f6", Url: "url 4" }
    ];

    //Load the module under test.
    beforeEach(module('sfSelectors'));

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));

    beforeEach(inject(function ($rootScope, $httpBackend, _$q_, $templateCache, _$timeout_) {
        //Build the scope with whom the directive will be compiled.
        scope = $rootScope.$new();
    }));

    beforeEach(function () {
        commonMethods.mockServerContextToEnableTemplateCache();
    });

    afterEach(function () {
        //Tear down.

        //the collection is changed after reordering
        sfExternalPages = [
            { TitlesPath: "title 1", ExternalPageId: "4c003fb0-2a77-61ec-be54-ff00007864f4", Url: "url 1" },
            { TitlesPath: "title 2", ExternalPageId: "4c003fb0-2a77-61ec-be54-ff00007864f3", Url: "url 2" },
            { TitlesPath: "title 3", ExternalPageId: "4c003fb0-2a77-61ec-be54-ff10007864f5", Url: "url 3" },
            { TitlesPath: "title 4", ExternalPageId: "4c003fb0-2a77-61ec-be54-ff10007864f6", Url: "url 4" }
        ];
    });

    it('[EGaneva] / should copy sfExternalPages in sfExternalPagesInDialog  collection.', function () {
        scope.sfExternalPages = sfExternalPages;
        var template = '<sf-external-urls-view sf-external-pages="sfExternalPages" sf-open-externals-in-new-tab="sfOpenExternalsInNewTab" sf-selected-items="selectedItemsInTheDialog"></sf-external-urls-view>';

        commonMethods.compileDirective(template, scope);
        var s = scope.$$childHead;

        expect(s.sfExternalPagesInDialog).toBeDefined();
        expect(s.sfExternalPages.length).toBe(scope.sfExternalPages.length);
        expect(s.sfExternalPagesInDialog.length).toBe(scope.sfExternalPages.length);
        for (var i = 0; i < scope.sfExternalPages.length; i++) {
            expect(s.sfExternalPages[i].ExternalPageId).toBe(scope.sfExternalPages[i].ExternalPageId);
            expect(s.sfExternalPagesInDialog[i].ExternalPageId).toBe(scope.sfExternalPages[i].ExternalPageId);
        }
    });

    it('[EGaneva] / should unselect selected item if the item is clicked.', function () {
        var selectedItems = [];
        selectedItems.push(sfExternalPages[0]);

        scope.sfExternalPages = sfExternalPages;
        scope.sfSelectedItems = selectedItems;
        var template = '<sf-external-urls-view sf-external-pages="sfExternalPages" sf-open-externals-in-new-tab="sfOpenExternalsInNewTab" sf-selected-items="sfSelectedItems"></sf-external-urls-view>';

        commonMethods.compileDirective(template, scope);
        var s = scope.$$childHead;

        var item = scope.sfSelectedItems[0];

        expect(s.sfSelectedItems).toBeDefined();
        expect(s.sfSelectedItems.length).toBe(1);
        expect(s.isItemSelected(item.ExternalPageId, item.Status)).toBe(true);

        s.itemClicked(item);

        expect(s.sfSelectedItems).toBeDefined();
        expect(s.sfSelectedItems.length).toBe(0);
        expect(s.isItemSelected(item.ExternalPageId, item.Status)).toBe(false);
    });

    it('[EGaneva] / should select item on item click if no items are selected.', function () {
        scope.sfExternalPages = sfExternalPages;
        var template = '<sf-external-urls-view sf-external-pages="sfExternalPages" sf-open-externals-in-new-tab="sfOpenExternalsInNewTab" sf-selected-items="selectedItemsInTheDialog"></sf-external-urls-view>';

        commonMethods.compileDirective(template, scope);
        var s = scope.$$childHead;

        var item = scope.sfExternalPages[1];

        expect(s.sfSelectedItems).not.toBeDefined();
        expect(s.isItemSelected(item.ExternalPageId, item.Status)).toBe(false);

        s.itemClicked(item);

        expect(s.sfSelectedItems).toBeDefined();
        expect(s.sfSelectedItems.length).toBe(1);
        expect(s.isItemSelected(item.ExternalPageId, item.Status)).toBe(true);
    });

    it('[EGaneva] / should select unselected item if the item is clicked.', function () {
        var selectedItems = [];
        selectedItems.push(sfExternalPages[0]);

        scope.sfExternalPages = sfExternalPages;
        scope.selectedItems = selectedItems;
        var template = '<sf-external-urls-view sf-external-pages="sfExternalPages" sf-open-externals-in-new-tab="sfOpenExternalsInNewTab" sf-selected-items="selectedItems"></sf-external-urls-view>';

        commonMethods.compileDirective(template, scope);
        var s = scope.$$childHead;

        var selectedItem = scope.selectedItems[0];
        var unselectedItem = scope.sfExternalPages[2];

        expect(s.sfSelectedItems).toBeDefined();
        expect(s.sfSelectedItems.length).toBe(1);
        expect(s.isItemSelected(selectedItem.ExternalPageId, selectedItem.Status)).toBe(true);
        expect(s.isItemSelected(unselectedItem.ExternalPageId, unselectedItem.Status)).toBe(false);

        s.itemClicked(unselectedItem);

        expect(s.sfSelectedItems).toBeDefined();
        expect(s.sfSelectedItems.length).toBe(2);
        expect(s.isItemSelected(selectedItem.ExternalPageId, selectedItem.Status)).toBe(true);
        expect(s.isItemSelected(unselectedItem.ExternalPageId, unselectedItem.Status)).toBe(true);
    });

    it('[EGaneva] / newly added item should look selected, but should not be added to selected item collection and should not be persisted.', function () {
        var selectedItems = [];

        scope.sfExternalPages = sfExternalPages;
        scope.selectedItems = selectedItems;
        var template = '<sf-external-urls-view sf-external-pages="sfExternalPages" sf-open-externals-in-new-tab="sfOpenExternalsInNewTab" sf-selected-items="selectedItems"></sf-external-urls-view>';

        commonMethods.compileDirective(template, scope);
        var s = scope.$$childHead;

        s.addItem();

        expect(s.sfExternalPages).toBeDefined();
        expect(s.sfExternalPages.length).toBe(4);
        expect(s.sfExternalPagesInDialog).toBeDefined();
        expect(s.sfExternalPagesInDialog.length).toBe(5);

        var newlyAddedItem = s.sfExternalPagesInDialog[4];

        expect(s.isItemSelected(newlyAddedItem.ExternalPageId, newlyAddedItem.Status)).toBe(true);
        expect(s.sfSelectedItems).toBeDefined();
        expect(s.sfSelectedItems.length).toBe(0);
    });

    it('[EGaneva] / adding title to newly added should add it to the selected item collection, if checked.', function () {
        var selectedItems = [];

        scope.sfExternalPages = sfExternalPages;
        scope.selectedItems = selectedItems;
        var template = '<sf-external-urls-view sf-external-pages="sfExternalPages" sf-open-externals-in-new-tab="sfOpenExternalsInNewTab" sf-selected-items="selectedItems"></sf-external-urls-view>';

        commonMethods.compileDirective(template, scope);
        var s = scope.$$childHead;

        s.addItem();
        var newlyAddedItem = s.sfExternalPagesInDialog[4];

        expect(s.isItemSelected(newlyAddedItem.ExternalPageId, newlyAddedItem.Status)).toBe(true);
        expect(s.sfSelectedItems.length).toBe(0);

        newlyAddedItem.TitlesPath = "newly added title";
        s.itemChanged(newlyAddedItem);
        s.$apply();
        expect(s.isItemSelected(newlyAddedItem.ExternalPageId, newlyAddedItem.Status)).toBe(true);
        expect(s.sfSelectedItems.length).toBe(1);
        expect(s.sfSelectedItems[0].ExternalPageId).toBe(newlyAddedItem.ExternalPageId);
        expect(s.sfSelectedItems[0].TitlesPath).toBe(newlyAddedItem.TitlesPath);
        expect(s.sfExternalPages.length).toBe(5);
        expect(s.sfExternalPagesInDialog.length).toBe(5);

        newlyAddedItem.Url = "newly added url";
        s.$apply();
        expect(s.sfSelectedItems[0].Url).toBe(newlyAddedItem.Url);
    });

    it('[EGaneva] / removing title to newly added should remove it from the selected item collection, even if checked.', function () {
        var selectedItems = [];
        selectedItems.push(sfExternalPages[3]);

        scope.sfExternalPages = sfExternalPages;
        scope.selectedItems = selectedItems;
        var template = '<sf-external-urls-view sf-external-pages="sfExternalPages" sf-open-externals-in-new-tab="sfOpenExternalsInNewTab" sf-selected-items="selectedItems"></sf-external-urls-view>';

        commonMethods.compileDirective(template, scope);
        var s = scope.$$childHead;

        var itemToEdit = s.sfExternalPagesInDialog[3];

        expect(s.isItemSelected(itemToEdit.ExternalPageId, itemToEdit.Status)).toBe(true);
        expect(s.sfSelectedItems.length).toBe(1);

        itemToEdit.TitlesPath = "";
        itemToEdit.Url = "";
        s.itemChanged(itemToEdit);
        s.$apply();
        expect(itemToEdit.Status).toBe('new');
        expect(s.isItemSelected(itemToEdit.ExternalPageId, itemToEdit.Status)).toBe(true);
        expect(s.sfSelectedItems.length).toBe(0);
        expect(s.sfExternalPages.length).toBe(3);
        expect(s.sfExternalPagesInDialog.length).toBe(4);
    });

    it('[EGaneva] / removing external url should remove it also from selected item collection.', function () {
        var selectedItems = [];
        selectedItems.push(sfExternalPages[3]);

        scope.sfExternalPages = sfExternalPages;
        scope.selectedItems = selectedItems;
        var template = '<sf-external-urls-view sf-external-pages="sfExternalPages" sf-open-externals-in-new-tab="sfOpenExternalsInNewTab" sf-selected-items="selectedItems"></sf-external-urls-view>';

        commonMethods.compileDirective(template, scope);
        var s = scope.$$childHead;

        var itemToRemove = s.sfExternalPagesInDialog[3];

        expect(s.isItemSelected(itemToRemove.ExternalPageId, itemToRemove.Status)).toBe(true);
        expect(s.sfSelectedItems.length).toBe(1);
        expect(s.sfExternalPages.length).toBe(4);
        expect(s.sfExternalPagesInDialog.length).toBe(4);

        s.removeItem(3, itemToRemove);
        s.$apply();

        expect(s.sfSelectedItems.length).toBe(0);
        expect(s.sfExternalPages.length).toBe(3);
        expect(s.sfExternalPagesInDialog.length).toBe(3);
    });

    it('[EGaneva] /if no external links are available isListEmpty should be true.', function () {

        var template = '<sf-external-urls-view sf-selected-items="selectedItems"></sf-external-urls-view>';

        commonMethods.compileDirective(template, scope);
        var s = scope.$$childHead;

        expect(s.isListEmpty()).toBe(true);
        expect(s.sfExternalPages.length).toBe(0);
        expect(s.sfExternalPagesInDialog.length).toBe(0);
    });
});