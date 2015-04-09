/* Tests for user selector */
describe("user selector", function () {
    var scope,
        ITEMS_COUNT = 10;

    //Will be returned from the service mock.
    var dataItem = {
        UserID: '4c003fb0-2a77-61ec-be54-ff00007864f4',
        UserName: "user"
    };

    var dataItem2 = {
        UserID: '4c003fb0-2a77-61ec-be54-ff11117864f4',
        UserName: "user filtered",
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
            UserID: '4c003fb0-2a77-61ec-be54-ff00007864f' + i,
            UserName: "user " + i
        };
    }

    var serviceResult;
    var $q;
    var provide;

    //Mock users service. It returns promises.
    var usersService = {
        getUsers: jasmine.createSpy('sfUsersService.getUsers').andCallFake(function (ignoreAdminUsers, provider, allProviders, skip, take, filter) {
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
        getSpecificUsers: jasmine.createSpy('sfUsersService.getSpecificUsers').andCallFake(function (ids, ignoreAdminUsers, provider, allProviders) {
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

    var usersServiceMockReturnMoreThan5Items = {
        getSpecificUsers: jasmine.createSpy('sfUsersService.getSpecificUsers').andCallFake(function (ids, ignoreAdminUsers, provider, allProviders) {
            if ($q) {
                serviceResult = $q.defer();
            }
            serviceResult.resolve(customDataItems);

            return serviceResult.promise;
        }),
    };

    //Load the module responsible for the modal dialog
    beforeEach(module('modalDialog'));

    //Load the module under test.
    beforeEach(module('sfSelectors'));

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));

    beforeEach(module(function ($provide) {
        //Force angular to use the mock.
        $provide.value('sfUsersService', usersService);

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
           return [];
        };
    }));

    afterEach(function () {
        //Tear down.
        var leftOver = $('.testDiv, .modal, .modal-backdrop');
        leftOver.empty();
        leftOver.remove();

        //The selector mutates the selected item when it is retrieved from the service.
        dataItem.UserName = { Value: 'Dummy' };
        dataItem2.UserName = { Value: 'Filtered' };

        //Sets default usersService mock.
        provide.value('sfUsersService', usersService);
    });

    var getUsersServiceGetItemsArgs = function () {
        var mostRecent = usersService.getUsers.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        return mostRecent.args;
    };

    var getUsersServiceGetSpecificItemsArgs = function () {
        var mostRecent = usersService.getSpecificUsers.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        return mostRecent.args;
    };

    describe('in single selection mode', function () {
        it('[GMateev] / should retrieve users from the service when the selector is opened.', function () {
            var template = "<sf-list-selector sf-user-selector sf-provider='provider'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            var args = getUsersServiceGetItemsArgs();

            //Ignore admin users
            expect(args[0]).toBeFalsy();

            //Provider
            expect(args[1]).toBe('OpenAccessDataProvider');

            //All providers
            expect(args[2]).toBe(true);

            //Skip
            expect(args[3]).toBe(0);

            //Take
            expect(args[4]).toBe(20);

            //Filter
            expect(args[5]).toBeFalsy();
        });

        it('[GMateev] / should retrieve selected users from the service when the selector is loaded.', function () {
            var template = "<sf-list-selector sf-user-selector sf-provider='provider' sf-selected-item-id='selectedId'/>";

            scope.selectedId = dataItem.UserID;

            commonMethods.compileDirective(template, scope);

            var args = getUsersServiceGetSpecificItemsArgs();

            //Item id
            expect(args[0]).toEqualArrayOfValues([dataItem.UserID]);

            //Ignore admin users
            expect(args[1]).toBeFalsy();

            //Provider
            expect(args[2]).toBe('OpenAccessDataProvider');

            //All providers
            expect(args[3]).toBe(true);
        });

        it('[GMateev] / should assign value to "selected-item" when "selected-item-id" is provided.', function () {
            var template = "<sf-list-selector sf-user-selector sf-selected-item='selectedItem' sf-selected-item-id='selectedId'/>";

            scope.selectedId = dataItem.UserID;

            commonMethods.compileDirective(template, scope);

            expect(scope.selectedId).toBeDefined();
            expect(scope.selectedId).toEqual(dataItem.UserID);

            expect(scope.selectedItem).toBeDefined();
            expect(scope.selectedItem.Id).toEqual(dataItem.UserID);
        });

        it('[GMateev] / should assign value to "selected-item-id" when "selected-item" is provided.', function () {
            var template = "<sf-list-selector sf-user-selector sf-selected-item='selectedItem' sf-selected-item-id='selectedId'/>";

            scope.selectedItem = dataItem;

            commonMethods.compileDirective(template, scope);

            expect(scope.selectedId).toBeDefined();
            expect(scope.selectedId).toEqual(dataItem.UserID);

            expect(scope.selectedItem).toBeDefined();
            expect(scope.selectedItem.Id).toEqual(dataItem.UserID);
        });

        it('[GMateev] / should select users when Done button is pressed.', function () {
            var template = "<sf-list-selector sf-user-selector sf-selected-item='selectedItem' sf-selected-item-id='selectedId'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            //mock the call to the modal service.
            s.$modalInstance = { close: function () { } };

            expect(s.sfSelectedItem).toBeFalsy();
            expect(s.sfSelectedItemId).toBeFalsy();

            expect(s.items).toBeDefined();
            expect(s.items[0].Id).toEqual(dataItem.UserID);
            expect(s.items[1].Id).toEqual(dataItem2.UserID);

            //Select item in the selector
            s.itemClicked(0, s.items[0]);

            expect(s.sfSelectedItem).toBeFalsy();
            expect(s.sfSelectedItemId).toBeFalsy();

            //Close the dialog (Done button clicked)
            s.doneSelecting();

            expect(s.sfSelectedItem).toBeDefined();
            expect(s.sfSelectedItem.Id).toEqual(dataItem.UserID);

            expect(s.sfSelectedItemId).toBeDefined();
            expect(s.sfSelectedItemId).toEqual(dataItem.UserID);
        });

        it('[GMateev] / should filter items when text is typed in the filter box.', function () {
            var template = "<sf-list-selector sf-user-selector sf-provider='provider'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.items).toBeDefined();
            expect(s.items[0].Id).toEqual(dataItem.UserID);
            expect(s.items[0].Filter).toBeFalsy();

            //Apply filter
            s.$apply(function () {
                s.filter.searchString = 'filter';
                s.filter.search(s.filter.searchString);
            });

            var args = getUsersServiceGetItemsArgs();

            //Ignore admin users
            expect(args[0]).toBeFalsy();

            //Provider
            expect(args[1]).toBe('OpenAccessDataProvider');

            //All providers
            expect(args[2]).toBe(true);

            //Skip
            expect(args[3]).toBe(0);

            //Take
            expect(args[4]).toBe(20);

            //Filter
            expect(args[5]).toBe('filter');

            expect(s.items).toBeDefined();
            expect(s.items[0].Id).toEqual(dataItem2.UserID);
            expect(s.items[0].Filter).toBe(true);
            expect(s.filter.searchString).toBe('filter');
        });

        it('[GMateev] / should move the selected item to be first in the list of all items.', function () {
            var template = "<sf-list-selector sf-user-selector sf-selected-item-id='selectedId'/>";

            scope.selectedId = dataItem2.UserID;

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.items).toBeDefined();
            expect(s.items.length).toBe(2);
            expect(s.items[0].Id).toEqual(dataItem2.UserID);
            expect(s.items[1].Id).toEqual(dataItem.UserID);
        });

        it('[GMateev] / should mark item as selected when the dialog is opened.', function () {
            var template = "<sf-list-selector sf-user-selector sf-selected-item-id='selectedId'/>";

            scope.selectedId = dataItem.UserID;

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog[0].Id).toEqual(dataItem.UserID);
        });

        it('[GMateev] / should select only one item in the opened dialog.', function () {
            var template = "<sf-list-selector sf-user-selector/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            //Select item in the selector
            s.itemClicked(0, s.items[0]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog[0].Id).toEqual(dataItem.UserID);

            //Select second item in the selector
            s.itemClicked(1, s.items[1]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog[0].Id).toEqual(dataItem2.UserID);
        });

        it('[GMateev] / should deselect the item if it is clicked and it is already selected.', function () {
            var template = "<sf-list-selector sf-user-selector sf-selected-item-id='selectedId'/>";

            scope.selectedId = dataItem.UserID;

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog[0].Id).toEqual(dataItem.UserID);

            //Select item in the selector
            s.itemClicked(0, s.items[0]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(0);
        });

        it('[NPetrova] / selectButtonText and changeButtonText should be undefined when no attributes are passed.', function () {
            var template = "<sf-list-selector sf-user-selector />";

            commonMethods.compileDirective(template, scope);

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectButtonText).not.toBeDefined();
            expect(s.changeButtonText).not.toBeDefined();
        });

        it('[NPetrova] / should replace the select button text with the one from the attributes.', function () {
            var template = "<sf-list-selector sf-user-selector sf-select-button-text='Select a user...'/>";

            commonMethods.compileDirective(template, scope);

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectButtonText).toBeDefined();
            expect(s.selectButtonText).toBe('Select a user...');
            expect(s.changeButtonText).not.toBeDefined();
        });

        it('[NPetrova] / should replace the select and change buttons text with the one from the attributes.', function() {
            var template = "<sf-list-selector sf-user-selector sf-select-button-text='Select a user...' sf-change-button-text='Change the user...'/>";

            commonMethods.compileDirective(template, scope);

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectButtonText).toBeDefined();
            expect(s.selectButtonText).toBe('Select a user...');
            expect(s.changeButtonText).toBeDefined();
            expect(s.changeButtonText).toBe('Change the user...');
        });
    });

    describe('in multi selection mode', function () {
        var items = [dataItem, dataItem2];
        var ids = [dataItem.UserID, dataItem2.UserID];

        it('[GMateev] / should retrieve user items from the service when the selector is opened.', function () {
            var template = "<sf-list-selector sf-user-selector sf-multiselect='true' sf-provider='provider'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            var args = getUsersServiceGetItemsArgs();

            //Ignore admin users
            expect(args[0]).toBeFalsy();

            //Provider
            expect(args[1]).toBe('OpenAccessDataProvider');

            //All providers
            expect(args[2]).toBe(true);

            //Skip
            expect(args[3]).toBe(0);

            //Take
            expect(args[4]).toBe(20);

            //Filter
            expect(args[5]).toBeFalsy();
        });

        it('[GMateev] / should retrieve selected users from the service when the selector is loaded.', function () {
            var template = "<sf-list-selector sf-user-selector sf-multiselect='true' sf-provider='provider' sf-selected-ids='selectedIds'/>";

            scope.selectedIds = ids;

            commonMethods.compileDirective(template, scope);

            var args = getUsersServiceGetSpecificItemsArgs();

            //Item id
            expect(args[0]).toEqualArrayOfValues([dataItem.UserID, dataItem2.UserID]);

            //Ignore admin users
            expect(args[1]).toBeFalsy();

            //Provider
            expect(args[2]).toBe('OpenAccessDataProvider');

            //All providers
            expect(args[3]).toBe(true);
        });

        it('[GMateev] / should assign value to "selected-items" when "selected-ids" are provided.', function () {
            var template = "<sf-list-selector sf-user-selector sf-multiselect='true' sf-selected-items='selectedItems' sf-selected-ids='selectedIds'/>";

            scope.selectedIds = ids;

            commonMethods.compileDirective(template, scope);

            expect(scope.selectedIds).toBeDefined();
            expect(scope.selectedIds.length).toBe(2);
            expect(scope.selectedIds).toEqualArrayOfValues(ids);

            expect(scope.selectedItems).toBeDefined();
            expect(scope.selectedItems.length).toEqual(2);
            expect(scope.selectedItems).toEqualArrayOfObjects(items, ['Id', 'UserName']);
        });

        it('[GMateev] / should assign value to "selected-ids" when "selected-items" is provided.', function () {
            var template = "<sf-list-selector sf-user-selector sf-multiselect='true' sf-selected-items='selectedItems' sf-selected-ids='selectedIds'/>";

            scope.selectedItems = items;

            commonMethods.compileDirective(template, scope);

            expect(scope.selectedIds).toBeDefined();
            expect(scope.selectedIds.length).toBe(2);
            expect(scope.selectedIds).toEqualArrayOfValues(ids);

            expect(scope.selectedItems).toBeDefined();
            expect(scope.selectedItems.length).toEqual(2);
            expect(scope.selectedItems).toEqualArrayOfObjects(items, ['Id', 'UserName']);
        });

        it('[GMateev] / should select users when Done button is pressed.', function () {
            var template = "<sf-list-selector sf-user-selector sf-multiselect='true' sf-selected-items='selectedItems' sf-selected-ids='selectedIds'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            //mock the call to the modal service.
            s.$modalInstance = { close: function () { } };

            expect(s.sfSelectedItems).toBeFalsy();
            expect(s.sfSelectedIds).toBeFalsy();

            expect(s.items).toBeDefined();
            expect(s.items[0].Id).toEqual(dataItem.UserID);
            expect(s.items[1].Id).toEqual(dataItem2.UserID);

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
            expect(scope.selectedItems).toEqualArrayOfObjects(items, ['Id', 'UserName']);
        });

        it('[GMateev] / should mark items as selected when the dialog is opened.', function () {
            var template = "<sf-list-selector sf-user-selector sf-multiselect='true' sf-selected-ids='selectedIds'/>";

            scope.selectedIds = ids;

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(2);
            expect(s.selectedItemsInTheDialog).toEqualArrayOfObjects(items, ['Id', 'UserName']);
        });

        it('[GMateev] / should select many items in the opened dialog.', function () {
            var template = "<sf-list-selector sf-user-selector sf-multiselect='true'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            //Select item in the selector
            s.itemClicked(0, s.items[0]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog[0].Id).toEqual(dataItem.UserID);

            //Select second item in the selector
            s.itemClicked(1, s.items[1]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(2);
            expect(s.selectedItemsInTheDialog[1].Id).toEqual(dataItem2.UserID);
        });

        it('[GMateev] / should deselect an item if it is clicked and it is already selected.', function () {
            var template = "<sf-list-selector sf-user-selector sf-multiselect='true' sf-selected-ids='selectedIds'/>";

            scope.selectedIds = ids;

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(2);
            expect(s.selectedItemsInTheDialog).toEqualArrayOfObjects(items, ['Id', 'UserName']);

            //Select item in the selector
            s.itemClicked(0, s.items[0]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog).toEqualArrayOfObjects([dataItem2], ['Id', 'UserName']);
        });
    });

    describe('in multi selection mode and more than 5 items selected by default', function () {
        it('[manev] / should show link indicating that there are 5 items more.', function () {

            provide.value('sfUsersService', usersServiceMockReturnMoreThan5Items);

            var ids = customDataItems.Items.map(function (i) { return i.UserID; });

            var template = "<sf-list-selector sf-user-selector sf-multiselect='true' sf-selected-items='selectedItems' sf-selected-ids='selectedIds'/>";

            scope.selectedIds = ids;
            commonMethods.compileDirective(template, scope);

            expect(scope.selectedItems).toBeDefined();
            expect(scope.selectedItems.length).toBe(10);
            expect(scope.selectedItems).toEqualArrayOfObjects(customDataItems.Items, ['Id', 'UserName']);

            var moreLink = $(".small");

            expect(moreLink.length).toBe(1);
            expect(moreLink.html()).toBe("and 5 more");
            expect(moreLink.css("display")).not.toBe("none");
        });

        it('[manev] / should show link indicating that there are 5 items more.', function () {

            var ids = [dataItem.UserID, dataItem2.UserID];

            var template = "<sf-list-selector sf-user-selector sf-multiselect='true' sf-selected-items='selectedItems' sf-selected-ids='selectedIds'/>";

            scope.selectedIds = ids;
            commonMethods.compileDirective(template, scope);

            var moreLink = $(".small");
            expect(moreLink.css("display")).toBe("none");
        });
    });
});