/* Tests for user selector */
describe("user selector", function () {
    var scope,
        ITEMS_COUNT = 10;

    //Will be returned from the service mock.
    var dataItem = {
        UserID: '4c003fb0-2a77-61ec-be54-ff00007864f4',
        UserName: "user",
        ProviderName: 'provider1'
    };

    var dataItem2 = {
        UserID: '4c003fb0-2a77-61ec-be54-ff11117864f4',
        UserName: "filtered",
        ProviderName: 'provider2'
    };

    var dataItem3 = {
        UserID: '4c003fb0-2a77-61ec-1254-ff11117864f4',
        UserName: 'test user',
        ProviderName: 'provider2'
    };

    var dataItems = {
        Items: [dataItem, dataItem2, dataItem3],
        TotalCount: 3
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

    var providers = [{
            "NumOfUsers": 3,
            "UserProviderName": "",
            "UserProviderTitle": "All Users"
        },
        {
            "NumOfUsers": 1,
            "UserProviderName": "provider1",
            "UserProviderTitle": "provder 1"
        },
        {
            "NumOfUsers": 2,
            "UserProviderName": "provider2",
            "UserProviderTitle": "provder 2"
        }];

    var serviceResult;
    var $q;
    var provide;

    //Mock users service. It returns promises.
    var usersService = {
        getUsers: jasmine.createSpy('sfUsersService.getUsers').andCallFake(function (provider, skip, take, filter) {
            if ($q) {
                serviceResult = $q.defer();
            }

            var result = [dataItem, dataItem2, dataItem3];

            if (filter) {
                result = result.filter(function (item) {
                    return item.UserName.indexOf(filter) >= 0;
                });
            }
            if (provider) {
                result = result.filter(function (item) {
                    return item.ProviderName === provider;
                });
            }

            serviceResult.resolve({
                Items: result,
                TotalCount: result.length
            });

            return {
                promise: serviceResult.promise,
            };
        }),
        getSpecificUsers: jasmine.createSpy('sfUsersService.getSpecificUsers').andCallFake(function (ids, provider) {
            if ($q) {
                serviceResult = $q.defer();
            }
            var items = [dataItem, dataItem2, dataItem3].filter(function (item) {
                return ids.indexOf(item.UserID) >= 0;
            });

            serviceResult.resolve({
                Items: items,
                TotalCount: items.length
            });

            return  {
                promise: serviceResult.promise
            };
        }),
        getUserProviders: jasmine.createSpy('sfUsersService.getUserProviders').andCallFake(
            function () {
                var deferred = $q.defer();
                deferred.resolve({
                    Items: providers
                });

                return {
                    promise: deferred.promise,
                };
            })
    };

    var usersServiceMockReturnMoreThan5Items = {
        getSpecificUsers: jasmine.createSpy('sfUsersService.getSpecificUsers').andCallFake(function (ids, provider) {
            if ($q) {
                serviceResult = $q.defer();
            }
            serviceResult.resolve(customDataItems);

            return {
                promise: serviceResult.promise
            };
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

            //Provider
            expect(args[0]).toBe('OpenAccessDataProvider');

            //Skip
            expect(args[1]).toBe(0);

            //Take
            expect(args[2]).toBe(20);

            //Filter
            expect(args[3]).toBeFalsy();
        });

        it('[GMateev] / should retrieve selected users from the service when the selector is loaded.', function () {
            var template = "<sf-list-selector sf-user-selector sf-selected-item-id='selectedId'/>";

            scope.selectedId = dataItem.UserID;

            commonMethods.compileDirective(template, scope);

            var args = getUsersServiceGetSpecificItemsArgs();

            //only the ids are passed
            expect(args.length).toBe(1);

            //Item id
            expect(args[0]).toEqualArrayOfValues([dataItem.UserID]);
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
            expect(s.items.length).toEqual(3);
            expect(s.items[0].Id).toEqual(dataItem.UserID);
            expect(s.items[1].Id).toEqual(dataItem2.UserID);
            expect(s.items[2].Id).toEqual(dataItem3.UserID);

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
            scope.provider = "";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.items).toBeDefined();
            expect(s.items[0].Id).toEqual(dataItem.UserID);

            //Apply filter
            s.$apply(function () {
                s.filter.searchString = 'filter';
                s.filter.search(s.filter.searchString);
            });

            var args = getUsersServiceGetItemsArgs();

            //Provider value is 'All Users' value ('') from the provider dropdown
            expect(args[0]).toBe('');

            //Skip
            expect(args[1]).toBe(0);

            //Take
            expect(args[2]).toBe(20);

            //Filter
            expect(args[3]).toBe('filter');

            expect(s.items).toBeDefined();
            expect(s.items[0].Id).toEqual(dataItem2.UserID);
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
            expect(s.items.length).toBe(3);
            expect(s.items[0].Id).toEqual(dataItem2.UserID);
            expect(s.items[1].Id).toEqual(dataItem.UserID);
            expect(s.items[2].Id).toEqual(dataItem3.UserID);
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

        it('[NPetrova] / should load selected item on the top when the provider is changed if the selected item is from this provider', function () {
            var template = "<sf-list-selector sf-user-selector />";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            var s = scope.$$childHead;

            expect(s.sfProvider).toBe(providers[0].UserProviderName);

            //Select 'test user' in the selector
            s.itemClicked(2, s.items[2]);

            s.providerChanged(providers[2].UserProviderName);
            s.$digest();

            //The selected item must be on the top when provider2 is selected
            expect(s.sfProvider).toBe(providers[2].UserProviderName);
            expect(s.items).toBeDefined();
            expect(s.items.length).toBe(2);
            expect(s.items[0].UserName).toBe(dataItem3.UserName);
            expect(s.items[1].UserName).toBe(dataItem2.UserName);

            s.providerChanged(providers[1].UserProviderName);
            s.$digest();

            //The selected item must not be on the top when provider1 is selected
            expect(s.sfProvider).toBe(providers[1].UserProviderName);
            expect(s.items).toBeDefined();
            expect(s.items.length).toBe(1);
            expect(s.items[0].UserName).toBe(dataItem.UserName);

            s.providerChanged(providers[0].UserProviderName);
            s.$digest();

            //The selected item must be on the top when 'All Users' provider is selected
            expect(s.sfProvider).toBe(providers[0].UserProviderName);
            expect(s.items).toBeDefined();
            expect(s.items.length).toBe(3);
            expect(s.items[0].UserName).toBe(dataItem3.UserName);
            expect(s.items[1].UserName).toBe(dataItem.UserName);
            expect(s.items[2].UserName).toBe(dataItem2.UserName);
        });

        it('[NPetrova] / should load items correctly when search is applied.', function () {
            var template = "<sf-list-selector sf-user-selector sf-selected-item-id='selectedItemId' />";
            scope.selectedItemId = dataItem3.Id;

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            var s = scope.$$childHead;

            expect(s.items).toBeDefined();
            expect(s.items.length).toBe(3);
            expect(s.items[0].UserName).toBe(dataItem3.UserName);
            expect(s.items[1].UserName).toBe(dataItem.UserName);
            expect(s.items[2].UserName).toBe(dataItem2.UserName);

            //search for 'user'
            s.filter.searchString = 'user';
            s.filter.search(s.filter.searchString);
            s.$digest();

            expect(s.items).toBeDefined();
            expect(s.items.length).toBe(2);
            expect(s.items[0].UserName).toBe(dataItem.UserName);
            expect(s.items[1].UserName).toBe(dataItem3.UserName);

            //change provider to provider1 (the selected item is not from this provider) while still search string is applied
            s.providerChanged(providers[1].UserProviderName);
            s.$digest();

            expect(s.sfProvider).toBe(providers[1].UserProviderName);
            expect(s.items).toBeDefined();
            expect(s.items.length).toBe(1);
            expect(s.items[0].UserName).toBe(dataItem.UserName);

            //change provider to provider2 (the selected item is from this provider) while still search string is applied
            s.providerChanged(providers[2].UserProviderName);
            s.$digest();

            expect(s.sfProvider).toBe(providers[2].UserProviderName);
            expect(s.items).toBeDefined();
            expect(s.items.length).toBe(1);
            expect(s.items[0].UserName).toBe(dataItem3.UserName);

            //remove search string
            s.filter.searchString = '';
            s.filter.search(s.filter.searchString);
            s.$digest();

            expect(s.sfProvider).toBe(providers[2].UserProviderName);
            expect(s.items).toBeDefined();
            expect(s.items.length).toBe(2);
            expect(s.items[0].UserName).toBe(dataItem3.UserName);
            expect(s.items[1].UserName).toBe(dataItem2.UserName);
        });
    });

    describe('in multi selection mode', function () {
        var items = [dataItem, dataItem2, dataItem3];
        var ids = [dataItem.UserID, dataItem2.UserID, dataItem3.UserID];

        it('[GeorgiMateev] / should rebind the items when the provider is changed.', function () {
            var template = "<sf-list-selector sf-user-selector sf-multiselect='true' />";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            var s = scope.$$childHead;
            s.providerChanged('changedProvider');

            var args = getUsersServiceGetItemsArgs();

            //Provider
            expect(args[0]).toBe('changedProvider');

            //Skip
            expect(args[1]).toBe(0);

            //Take
            expect(args[2]).toBe(20);

            //Filter
            expect(args[3]).toBeFalsy();
        });

        it('[GMateev] / should retrieve user items from the service when the selector is opened.', function () {
            var template = "<sf-list-selector sf-user-selector sf-multiselect='true' sf-provider='provider'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            var args = getUsersServiceGetItemsArgs();

            //Provider
            expect(args[0]).toBe('OpenAccessDataProvider');

            //Skip
            expect(args[1]).toBe(0);

            //Take
            expect(args[2]).toBe(20);

            //Filter
            expect(args[3]).toBeFalsy();
        });

        it('[GMateev] / should retrieve selected users from the service when the selector is loaded.', function () {
            var template = "<sf-list-selector sf-user-selector sf-multiselect='true' sf-provider='provider' sf-selected-ids='selectedIds'/>";

            scope.selectedIds = ids;

            commonMethods.compileDirective(template, scope);

            var args = getUsersServiceGetSpecificItemsArgs();

            //Item id
            expect(args[0]).toEqualArrayOfValues([dataItem.UserID, dataItem2.UserID, dataItem3.UserID]);
        });

        it('[GMateev] / should assign value to "selected-items" when "selected-ids" are provided.', function () {
            var template = "<sf-list-selector sf-user-selector sf-multiselect='true' sf-selected-items='selectedItems' sf-selected-ids='selectedIds'/>";

            scope.selectedIds = ids;

            commonMethods.compileDirective(template, scope);

            expect(scope.selectedIds).toBeDefined();
            expect(scope.selectedIds.length).toBe(3);
            expect(scope.selectedIds).toEqualArrayOfValues(ids);

            expect(scope.selectedItems).toBeDefined();
            expect(scope.selectedItems.length).toEqual(3);
            expect(scope.selectedItems).toEqualArrayOfObjects(items, ['UserID', 'UserName', 'ProviderName']);
        });

        it('[GMateev] / should assign value to "selected-ids" when "selected-items" is provided.', function () {
            var template = "<sf-list-selector sf-user-selector sf-multiselect='true' sf-selected-items='selectedItems' sf-selected-ids='selectedIds'/>";

            scope.selectedItems = items;

            commonMethods.compileDirective(template, scope);

            expect(scope.selectedIds).toBeDefined();
            expect(scope.selectedIds.length).toBe(3);
            expect(scope.selectedIds).toEqualArrayOfValues(ids);

            expect(scope.selectedItems).toBeDefined();
            expect(scope.selectedItems.length).toEqual(3);
            expect(scope.selectedItems).toEqualArrayOfObjects(items, ['UserID', 'UserName', 'ProviderName']);
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
            expect(s.items.length).toBe(3);
            expect(s.items[0].Id).toEqual(dataItem.UserID);
            expect(s.items[1].Id).toEqual(dataItem2.UserID);
            expect(s.items[2].Id).toEqual(dataItem3.UserID);

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
            expect(scope.selectedIds).toEqualArrayOfValues([dataItem.UserID, dataItem2.UserID]);

            expect(scope.selectedItems).toBeDefined();
            expect(scope.selectedItems.length).toEqual(2);
            expect(scope.selectedItems).toEqualArrayOfObjects([dataItem, dataItem2], ['UserID', 'UserName', 'ProviderName']);
        });

        it('[GMateev] / should mark items as selected when the dialog is opened.', function () {
            var template = "<sf-list-selector sf-user-selector sf-multiselect='true' sf-selected-ids='selectedIds'/>";

            scope.selectedIds = ids;

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(3);
            expect(s.selectedItemsInTheDialog).toEqualArrayOfObjects(items, ['UserID', 'UserName', 'ProviderName']);
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
            expect(s.selectedItemsInTheDialog.length).toEqual(3);
            expect(s.selectedItemsInTheDialog).toEqualArrayOfObjects(items, ['UserID', 'UserName', 'ProviderName']);

            //Select item in the selector
            s.itemClicked(0, s.items[0]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(2);
            expect(s.selectedItemsInTheDialog).toEqualArrayOfObjects([dataItem2, dataItem3], ['UserID', 'UserName', 'ProviderName']);
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
            expect(scope.selectedItems).toEqualArrayOfObjects(customDataItems.Items, ['UserID', 'UserName']);

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