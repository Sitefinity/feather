/* Tests for lists selector */
describe("lists selector", function () {
    var scope,
        ITEMS_COUNT = 10;

    //Will be returned from the service mock.
    var dataItem = {
        Id: '4c003fb0-2a77-61ec-be54-ff00007864f4',
        Title: { Value: 'Dummy' }
    };

    var dataItem2 = {
        Id: '4c003fb0-2a77-61ec-be54-ff11117864f4',
        Title: { Value: 'Filtered' },
        Filter: true
    };

    var dataItems = {
        Items: [dataItem, dataItem2],
        TotalCount: 2
    };

    var customDataItems = {
        Items: [],
        TotalCount: ITEMS_COUNT
    };

    for (var i = 0; i < ITEMS_COUNT; i++) {
        customDataItems.Items[i] = {
            Id: '4c003fb0-2a77-61ec-be54-ff00007864f' + i,
            Title: { Value: 'Dummy' + i }
        };
    }

    var serviceResult;
    var $q;
    var provide;

    //Mock lists item service. It returns promises.
    var listsService = {
        getItems: jasmine.createSpy('sfListsService.getItems').andCallFake(function (provider, skip, take, filter) {
            if ($q) {
                serviceResult = $q.defer();
            }

            if (filter) {
                serviceResult.resolve({
                    Items: [dataItem2],
                    TotalCount: 1
                });
            }
            else {
                serviceResult.resolve(dataItems);
            }

            return serviceResult.promise;
        }),
        getSpecificItems: jasmine.createSpy('sfListsService.getSpecificItems').andCallFake(function (ids, provider) {
            if ($q) {
                serviceResult = $q.defer();
            }
            var items = [dataItem, dataItem2].filter(function (item) {
                return ids.indexOf(item.Id) >= 0;
            });

            serviceResult.resolve({
                Items: items,
                TotalCount: items.length
            });

            return serviceResult.promise;
        })
    };

    //Load the module responsible for the modal dialog
    beforeEach(module('modalDialog'));

    //Load the module under test.
    beforeEach(module('sfSelectors'));

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));

    beforeEach(module(function ($provide) {
        //Force angular to use the mock.
        $provide.value('sfListsService', listsService);

        provide = $provide;
    }));

    beforeEach(inject(function ($rootScope, _$q_) {
        //Build the scope with whom the directive will be compiled.
        scope = $rootScope.$new();
        scope.provider = 'OpenAccessDataProvider';

        $q = _$q_;

        serviceResult = _$q_.defer();
    }));

    beforeEach(function () {
        commonMethods.mockServerContextToEnableTemplateCache();
    });

    beforeEach(inject(function (serverContext) {
        serverContext.getFrontendLanguages = function () {
            return ['en', 'de'];
        };
    }));

    afterEach(function () {
        //Tear down.
        var leftOver = $('.testDiv, .modal, .modal-backdrop');
        leftOver.empty();
        leftOver.remove();

        //The selector mutates the selected item when it is retrieved from the service.
        dataItem.Title = { Value: 'Dummy' };
        dataItem2.Title = { Value: 'Filtered' };

        //Sets default listItemService mock.
        provide.value('sfListsService', listsService);
    });

    var getListsServiceGetItemsArgs = function () {
        var mostRecent = listsService.getItems.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        return mostRecent.args;
    };

    var getListsServiceGetItemArgs = function () {
        var mostRecent = listsService.getItem.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        return mostRecent.args;
    };

    var getListsServiceGetSpecificItemsArgs = function () {
        var mostRecent = listsService.getSpecificItems.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        return mostRecent.args;
    };

    describe('in single selection mode', function () {
        it('[Manev] / should retrieve lists items from the service when the selector is opened.', function () {
            var template = "<sf-list-selector sf-lists-selector sf-provider='provider'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            var args = getListsServiceGetItemsArgs();

            //Provider
            expect(args[0]).toBe('OpenAccessDataProvider');

            //Skip
            expect(args[1]).toBe(0);

            //Take
            expect(args[2]).toBe(20);

            //Filter
            expect(args[3]).toBeFalsy();
        });

        it('[Manev] / should retrieve selected lists item from the service when the selector is loaded.', function () {
            var template = "<sf-list-selector sf-lists-selector sf-provider='provider' sf-selected-item-id='selectedId'/>";

            scope.selectedId = dataItem.Id;

            commonMethods.compileDirective(template, scope);

            var args = getListsServiceGetSpecificItemsArgs();

            //Item id
            expect(args[0]).toEqualArrayOfValues([dataItem.Id]);

            //Provider
            expect(args[1]).toBe('OpenAccessDataProvider');
        });

        it('[Manev] / should assign value to "selected-item" when "selected-item-id" is provided.', function () {
            var template = "<sf-list-selector sf-lists-selector sf-selected-item='selectedItem' sf-selected-item-id='selectedId'/>";

            scope.selectedId = dataItem.Id;

            commonMethods.compileDirective(template, scope);

            expect(scope.selectedId).toBeDefined();
            expect(scope.selectedId).toEqual(dataItem.Id);

            expect(scope.selectedItem).toBeDefined();
            expect(scope.selectedItem.Id).toEqual(dataItem.Id);
        });

        it('[Manev] / should assign value to "selected-item-id" when "selected-item" is provided.', function () {
            var template = "<sf-list-selector sf-lists-selector sf-selected-item='selectedItem' sf-selected-item-id='selectedId'/>";

            scope.selectedItem = dataItem;

            commonMethods.compileDirective(template, scope);

            expect(scope.selectedId).toBeDefined();
            expect(scope.selectedId).toEqual(dataItem.Id);

            expect(scope.selectedItem).toBeDefined();
            expect(scope.selectedItem.Id).toEqual(dataItem.Id);
        });

        it('[Manev] / should select lists item when Done button is pressed.', function () {
            var template = "<sf-list-selector sf-lists-selector sf-selected-item='selectedItem' sf-selected-item-id='selectedId'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            //mock the call to the modal service.
            s.$modalInstance = { close: function () { } };

            expect(s.sfSelectedItem).toBeFalsy();
            expect(s.sfSelectedItemId).toBeFalsy();

            expect(s.items).toBeDefined();
            expect(s.items[0].Id).toEqual(dataItem.Id);
            expect(s.items[1].Id).toEqual(dataItem2.Id);

            //Select item in the selector
            s.itemClicked(0, s.items[0]);

            expect(s.sfSelectedItem).toBeFalsy();
            expect(s.sfSelectedItemId).toBeFalsy();

            //Close the dialog (Done button clicked)
            s.doneSelecting();

            expect(s.sfSelectedItem).toBeDefined();
            expect(s.sfSelectedItem.Id).toEqual(dataItem.Id);

            expect(s.sfSelectedItemId).toBeDefined();
            expect(s.sfSelectedItemId).toEqual(dataItem.Id);
        });

        it('[Manev] / should filter items when text is typed in the filter box.', function () {
            var template = "<sf-list-selector sf-lists-selector sf-provider='provider'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.items).toBeDefined();
            expect(s.items[0].Id).toEqual(dataItem.Id);
            expect(s.items[0].Filter).toBeFalsy();

            //Apply filter
            s.$apply(function () {
                s.filter.searchString = 'filter';
                s.filter.search(s.filter.searchString);
            });

            var args = getListsServiceGetItemsArgs();

            //Provider
            expect(args[0]).toBe('OpenAccessDataProvider');

            //Skip
            expect(args[1]).toBe(0);

            //Take
            expect(args[2]).toBe(20);

            //Filter
            expect(args[3]).toBe('filter');

            expect(s.items).toBeDefined();
            expect(s.items[0].Id).toEqual(dataItem2.Id);
            expect(s.items[0].Filter).toBe(true);
            expect(s.filter.searchString).toBe('filter');
        });

        it('[Manev] / should move the selected item to be first in the list of all items.', function () {
            var template = "<sf-list-selector sf-lists-selector sf-selected-item-id='selectedId'/>";

            scope.selectedId = dataItem2.Id;

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.items).toBeDefined();
            expect(s.items.length).toBe(2);
            expect(s.items[0].Id).toEqual(dataItem2.Id);
            expect(s.items[1].Id).toEqual(dataItem.Id);
        });

        it('[Manev] / should mark item as selected when the dialog is opened.', function () {
            var template = "<sf-list-selector sf-lists-selector sf-selected-item-id='selectedId'/>";

            scope.selectedId = dataItem.Id;

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog[0].Id).toEqual(dataItem.Id);
        });

        it('[Manev] / should select only one item in the opened dialog.', function () {
            var template = "<sf-list-selector sf-lists-selector/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            //Select item in the selector
            s.itemClicked(0, s.items[0]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog[0].Id).toEqual(dataItem.Id);

            //Select second item in the selector
            s.itemClicked(1, s.items[1]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog[0].Id).toEqual(dataItem2.Id);
        });

        it('[Manev] / should deselect the item if it is clicked and it is already selected.', function () {
            var template = "<sf-list-selector sf-lists-selector sf-selected-item-id='selectedId'/>";

            scope.selectedId = dataItem.Id;

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog[0].Id).toEqual(dataItem.Id);

            //Select item in the selector
            s.itemClicked(0, s.items[0]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(0);
        });

        it('[Manev] / should show the default text values for the select and change buttons when no attributes are passed.', function () {
            var template = "<sf-list-selector sf-lists-selector />";

            commonMethods.compileDirective(template, scope);

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectButtonText).not.toBeDefined();
            expect(s.changeButtonText).not.toBeDefined();
        });

        it('[Manev] / should replace the select button text with the one from the attributes.', function () {
            var template = "<sf-list-selector sf-lists-selector sf-select-button-text='Select a lists...'/>";

            commonMethods.compileDirective(template, scope);

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectButtonText).toBeDefined();
            expect(s.selectButtonText).toBe('Select a lists...');
            expect(s.changeButtonText).not.toBeDefined();
        });

        it('[Manev] / should replace the select and change buttons text with the one from the attributes.', function () {
            var template = "<sf-list-selector sf-lists-selector sf-select-button-text='Select a lists...' sf-change-button-text='Change the lists...'/>";

            commonMethods.compileDirective(template, scope);

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectButtonText).toBeDefined();
            expect(s.selectButtonText).toBe('Select a lists...');
            expect(s.changeButtonText).toBeDefined();
            expect(s.changeButtonText).toBe('Change the lists...');
        });
    });

    describe('[Manev] / in multi selection mode', function () {
        var items = [dataItem, dataItem2];
        var ids = [dataItem.Id, dataItem2.Id];

        it('should retrieve lists items from the service when the selector is opened.', function () {
            var template = "<sf-list-selector sf-lists-selector sf-multiselect='true' sf-provider='provider'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            var args = getListsServiceGetItemsArgs();

            //Provider
            expect(args[0]).toBe('OpenAccessDataProvider');

            //Skip
            expect(args[1]).toBe(0);

            //Take
            expect(args[2]).toBe(20);

            //Filter
            expect(args[3]).toBeFalsy();
        });

        it('should retrieve selected lists items from the service when the selector is loaded.', function () {
            var _template = "<sf-list-selector sf-lists-selector sf-multiselect='true' sf-provider='provider' sf-selected-ids='selectedIds'/>";

            scope.selectedIds = ids;

            commonMethods.compileDirective(_template, scope);

            var args = getListsServiceGetSpecificItemsArgs();

            //Item id
            expect(args[0]).toEqualArrayOfValues([dataItem.Id, dataItem2.Id]);

            //Provider
            expect(args[1]).toBe('OpenAccessDataProvider');
        });

        it('should assign value to "selected-items" when "selected-ids" are provided.', function () {
            var template = "<sf-list-selector sf-lists-selector sf-multiselect='true' sf-selected-items='selectedItems' sf-selected-ids='selectedIds'/>";

            scope.selectedIds = ids;

            commonMethods.compileDirective(template, scope);

            expect(scope.selectedIds).toBeDefined();
            expect(scope.selectedIds.length).toBe(2);
            expect(scope.selectedIds).toEqualArrayOfValues(ids);

            expect(scope.selectedItems).toBeDefined();
            expect(scope.selectedItems.length).toEqual(2);
            expect(scope.selectedItems).toEqualArrayOfObjects(items, ['Id', 'Title']);
        });

        it('should assign value to "selected-ids" when "selected-items" is provided.', function () {
            var template = "<sf-list-selector sf-lists-selector sf-multiselect='true' sf-selected-items='selectedItems' sf-selected-ids='selectedIds'/>";

            scope.selectedItems = items;

            commonMethods.compileDirective(template, scope);

            expect(scope.selectedIds).toBeDefined();
            expect(scope.selectedIds.length).toBe(2);
            expect(scope.selectedIds).toEqualArrayOfValues(ids);

            expect(scope.selectedItems).toBeDefined();
            expect(scope.selectedItems.length).toEqual(2);
            expect(scope.selectedItems).toEqualArrayOfObjects(items, ['Id', 'Title']);
        });

        it('should select lists items when Done button is pressed.', function () {
            var template = "<sf-list-selector sf-lists-selector sf-multiselect='true' sf-selected-items='selectedItems' sf-selected-ids='selectedIds'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            //mock the call to the modal service.
            s.$modalInstance = { close: function () { } };

            expect(s.sfSelectedItems).toBeFalsy();
            expect(s.sfSelectedIds).toBeFalsy();

            expect(s.items).toBeDefined();
            expect(s.items[0].Id).toEqual(dataItem.Id);
            expect(s.items[1].Id).toEqual(dataItem2.Id);

            //Select item in the selector
            s.itemClicked(0, s.items[0]);
            s.itemClicked(1, s.items[1]);

            expect(s.sfSelectedItems).toBeFalsy();
            expect(s.sfSelectedIds).toBeFalsy();

            //Close the dialog (Done button clicked)
            s.doneSelecting();

            scope.$digest();

            expect(scope.selectedIds).toBeDefined();
            expect(scope.selectedIds.length).toBe(2);
            expect(scope.selectedIds).toEqualArrayOfValues(ids);

            expect(scope.selectedItems).toBeDefined();
            expect(scope.selectedItems.length).toEqual(2);
            expect(scope.selectedItems).toEqualArrayOfObjects(items, ['Id', 'Title']);
        });

        it('should mark items as selected when the dialog is opened.', function () {
            var template = "<sf-list-selector sf-lists-selector sf-multiselect='true' sf-selected-ids='selectedIds'/>";

            scope.selectedIds = ids;

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(2);
            expect(s.selectedItemsInTheDialog).toEqualArrayOfObjects(items, ['Id', 'Title']);
        });

        it('should select many items in the opened dialog.', function () {
            var template = "<sf-list-selector sf-lists-selector sf-multiselect='true'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            //Select item in the selector
            s.itemClicked(0, s.items[0]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog[0].Id).toEqual(dataItem.Id);

            //Select second item in the selector
            s.itemClicked(1, s.items[1]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(2);
            expect(s.selectedItemsInTheDialog[1].Id).toEqual(dataItem2.Id);
        });

        it('should deselect an item if it is clicked and it is already selected.', function () {
            var template = "<sf-list-selector sf-lists-selector sf-multiselect='true' sf-selected-ids='selectedIds'/>";

            scope.selectedIds = ids;

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(2);
            expect(s.selectedItemsInTheDialog).toEqualArrayOfObjects(items, ['Id', 'Title']);

            //Select item in the selector
            s.itemClicked(0, s.items[0]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog).toEqualArrayOfObjects([dataItem2], ['Id', 'Title']);
        });
    });
});