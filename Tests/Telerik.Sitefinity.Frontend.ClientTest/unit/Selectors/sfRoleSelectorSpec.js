describe('role selector', function () {
    var $q;
    var $rootScope;

    //Will be returned from the service mock.
    var dataItem = {
        Id: '4c003fb0-2a77-61ec-be54-ff00007864f4',
        Name: 'user role',
        ProviderName: 'provider1'
    };

    var dataItem2 = {
        Id: '4c003fb0-2a77-61ec-be54-ff11117864f4',
        Name: 'admin',
        ProviderName: 'provider2'
    };

    var dataItem3 = {
        Id: '4c003fb0-2a77-61ec-1254-ff11117864f4',
        Name: 'designer role',
        ProviderName: 'provider2'
    };

    var providers = [{
            "NumOfRoles": 3,
            "RoleProviderName": "",
            "RoleProviderTitle": "All roles"
        },
        {
            "NumOfRoles": 1,
            "RoleProviderName": "provider1",
            "RoleProviderTitle": "provder 1"
        },
        {
            "NumOfRoles": 2,
            "RoleProviderName": "provider2",
            "RoleProviderTitle": "provder 2"
        }];

    var dataItems =  [dataItem, dataItem2, dataItem3];

    var rolesService = {
        getRoles: jasmine.createSpy('sfRolesService').andCallFake(
            function (provider, skip, take, search, rolesToHide) {
                var deferred = $q.defer();
                
                var items = dataItems;
                if (provider) {
                    items = items.filter(function (item) {
                        return item.ProviderName === provider;
                    });
                }
                if (search) {
                    items = items.filter(function (item) {
                        return item.Name.indexOf(search) >= 0;
                    });
                }

                deferred.resolve({
                    Items: items,
                    TotalCount: dataItems.length
                });

                return {
                    promise: deferred.promise,
                };
            }),
        getSpecificRoles: jasmine.createSpy('sfRolesServie.getSpecificRoles').andCallFake(
            function (ids, provider) {
                var deferred = $q.defer();
                var items = [dataItem, dataItem2, dataItem3].filter(function (item) {
                    return ids.indexOf(item.Id) >= 0;
                });

                deferred.resolve({
                    Items: items,
                    TotalCount: items.length
                });

                return {
                    promise: deferred.promise,
                };
            }),
        getRoleProviders: jasmine.createSpy('sfRolesService.getRoleProviders').andCallFake(
            function (commaSeperatedAbilities) {
                var deferred = $q.defer();
                deferred.resolve({
                    Items: providers
                });

                return {
                    promise: deferred.promise,
                };
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
        $provide.value('sfRolesService', rolesService);
    }));

    beforeEach(inject(function (_$rootScope_, $httpBackend, _$q_) {
        $rootScope = _$rootScope_;
        $q = _$q_;
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
    });

    var getRolesServiceGetRolesArgs = function () {
        var mostRecent = rolesService.getRoles.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        return mostRecent.args;
    };

    var getRolesServiceGetSpecificRolesArgs = function () {
        var mostRecent = rolesService.getSpecificRoles.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        return mostRecent.args;
    };

    it('[GeorgiMateev] / should hide specified roles.', function () {
        var template = "<sf-list-selector sf-role-selector sf-multiselect='true' sf-hide-roles='Owner, Anonymous, Authenticated, Everyone'/>";
        var scope = $rootScope.$new();

        commonMethods.compileDirective(template, scope);

        $('.openSelectorBtn').click();

        var args = getRolesServiceGetRolesArgs();

        //Provider
        expect(args[0]).toBeFalsy();

        //Skip
        expect(args[1]).toBe(0);

        //Take
        expect(args[2]).toBe(20);

        //Filter
        expect(args[3]).toBeFalsy();

        // rolesToHide
        expect(args[4]).toEqualArrayOfValues(['Owner', 'Anonymous', 'Authenticated', 'Everyone']);
    });

    it('[GeorgiMateev] / should rebind the items when the provider is changed.', function () {
        var template = "<sf-list-selector sf-role-selector sf-multiselect='true' />";
        var scope = $rootScope.$new();

        commonMethods.compileDirective(template, scope);

        $('.openSelectorBtn').click();

        var s = scope.$$childHead;
        s.providerChanged('changedProvider');

        var args = getRolesServiceGetRolesArgs();

        //Provider
        expect(args[0]).toBe('changedProvider');

        //Skip
        expect(args[1]).toBe(0);

        //Take
        expect(args[2]).toBe(20);

        //Filter
        expect(args[3]).toBeFalsy();
    });

    describe('in multi selection mode', function () {
        var items = [dataItem, dataItem2];
        var ids = [dataItem.Id, dataItem2.Id];

        it('[GeorgiMateev] / should retrieve roles from the service when the selector is opened.', function () {
            var template = "<sf-list-selector sf-role-selector sf-multiselect='true' sf-provider='provider'/>";
            var scope = $rootScope.$new();

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            var args = getRolesServiceGetRolesArgs();

            //Provider
            expect(args[0]).toBeFalsy();

            //Skip
            expect(args[1]).toBe(0);

            //Take
            expect(args[2]).toBe(20);

            //Filter
            expect(args[3]).toBeFalsy();

            // rolesToHide
            expect(args[4]).toEqualArrayOfValues([]);
        });

        it('[GeorgiMateev] / should retrieve selected roles from the service when the selector is loaded.', function () {
            var template = "<sf-list-selector sf-role-selector sf-multiselect='true' sf-provider='provider' sf-selected-ids='selectedIds'/>";
            var scope = $rootScope.$new();
            scope.selectedIds = ids;

            commonMethods.compileDirective(template, scope);

            var args = getRolesServiceGetSpecificRolesArgs();

            //Item id
            expect(args[0]).toEqualArrayOfValues([dataItem.Id, dataItem2.Id]);
        });

        it('[GeorgiMateev] / should select roles when Done button is pressed.', function () {
            var template = "<sf-list-selector sf-role-selector sf-multiselect='true' sf-selected-items='selection.selectedItems' sf-selected-ids='selection.selectedIds'></sf-list-selector>";
            var scope = $rootScope.$new();
            scope.selection = {};

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

            s.$digest();
            scope.$digest();

            expect(scope.selection.selectedIds).toBeDefined();
            expect(scope.selection.selectedIds.length).toBe(2);
            expect(scope.selection.selectedIds).toEqualArrayOfValues(ids);

            expect(scope.selection.selectedItems).toBeDefined();
            expect(scope.selection.selectedItems.length).toEqual(2);
            expect(scope.selection.selectedItems).toEqualArrayOfObjects(items, ['Id', 'Title']);
        });

        it('[GeorgiMateev] / should mark items as selected when the dialog is opened.', function () {
            var template = "<sf-list-selector sf-role-selector sf-multiselect='true' sf-selected-ids='selectedIds'/>";
            var scope = $rootScope.$new();
            scope.selectedIds = ids;

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(2);
            expect(s.selectedItemsInTheDialog).toEqualArrayOfObjects(items, ['Id', 'Title']);
        });

        it('[GeorgiMateev] / should select many items in the opened dialog.', function () {
            var template = "<sf-list-selector sf-role-selector sf-multiselect='true'/>";

            var scope = $rootScope.$new();

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

        it('[GeorgiMateev] / should deselect an item if it is clicked and it is already selected.', function () {
            var template = "<sf-list-selector sf-role-selector sf-multiselect='true' sf-selected-ids='selectedIds'/>";
            var scope = $rootScope.$new();
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

    describe('in single selection mode', function () {
        it('[NPetrova] / should load selected item on the top when the provider is changed if the selected item is from this provider', function () {
            var template = "<sf-list-selector sf-role-selector />";
            var scope = $rootScope.$new();

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            var s = scope.$$childHead;

            expect(s.sfProvider).toBe(providers[0].RoleProviderName);

            //Select 'designer' role in the selector
            s.itemClicked(2, s.items[2]);

            s.providerChanged(providers[2].RoleProviderName);
            s.$digest();

            //The selected item must be on the top when provider2 is selected
            expect(s.sfProvider).toBe(providers[2].RoleProviderName);
            expect(s.items).toBeDefined();
            expect(s.items.length).toBe(2);
            expect(s.items[0].Name).toBe(dataItem3.Name);
            expect(s.items[1].Name).toBe(dataItem2.Name);

            s.providerChanged(providers[1].RoleProviderName);
            s.$digest();

            //The selected item must not be on the top when provider1 is selected
            expect(s.sfProvider).toBe(providers[1].RoleProviderName);
            expect(s.items).toBeDefined();
            expect(s.items.length).toBe(1);
            expect(s.items[0].Name).toBe(dataItem.Name);

            s.providerChanged(providers[0].RoleProviderName);
            s.$digest();

            //The selected item must be on the top when 'All roles' provider is selected
            expect(s.sfProvider).toBe(providers[0].RoleProviderName);
            expect(s.items).toBeDefined();
            expect(s.items.length).toBe(3);
            expect(s.items[0].Name).toBe(dataItem3.Name);
            expect(s.items[1].Name).toBe(dataItem.Name);
            expect(s.items[2].Name).toBe(dataItem2.Name);
        });

        it('[NPetrova] / should load items correctly when search is applied.', function () {
            var template = "<sf-list-selector sf-role-selector sf-selected-item-id='selectedItemId' />";
            var scope = $rootScope.$new();
            scope.selectedItemId = dataItem3.Id;

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            var s = scope.$$childHead;

            expect(s.items).toBeDefined();
            expect(s.items.length).toBe(3);
            expect(s.items[0].Name).toBe(dataItem3.Name);
            expect(s.items[1].Name).toBe(dataItem.Name);
            expect(s.items[2].Name).toBe(dataItem2.Name);

            //search for 'role'
            s.filter.searchString = 'role';
            s.filter.search(s.filter.searchString);
            s.$digest();

            expect(s.items).toBeDefined();
            expect(s.items.length).toBe(2);
            expect(s.items[0].Name).toBe(dataItem.Name);
            expect(s.items[1].Name).toBe(dataItem3.Name);

            //change provider to provider1 (the selected item is not from this provider) while still search string is applied
            s.providerChanged(providers[1].RoleProviderName);
            s.$digest();

            expect(s.sfProvider).toBe(providers[1].RoleProviderName);
            expect(s.items).toBeDefined();
            expect(s.items.length).toBe(1);
            expect(s.items[0].Name).toBe(dataItem.Name);

            //change provider to provider2 (the selected item is from this provider) while still search string is applied
            s.providerChanged(providers[2].RoleProviderName);
            s.$digest();

            expect(s.sfProvider).toBe(providers[2].RoleProviderName);
            expect(s.items).toBeDefined();
            expect(s.items.length).toBe(1);
            expect(s.items[0].Name).toBe(dataItem3.Name);

            //remove search string
            s.filter.searchString = '';
            s.filter.search(s.filter.searchString);
            s.$digest();

            expect(s.sfProvider).toBe(providers[2].RoleProviderName);
            expect(s.items).toBeDefined();
            expect(s.items.length).toBe(2);
            expect(s.items[0].Name).toBe(dataItem3.Name);
            expect(s.items[1].Name).toBe(dataItem2.Name);
        });
    });
});