/* Tests for calendar selector */
describe("calendar selector", function () {
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

    var serviceResult;
    var $q;
    var provide;

    //Mock calendar service. It returns promises.
    var calendarService = {
        getItems: jasmine.createSpy('sfCalendarService.getItems').andCallFake(function (provider, skip, take, filter) {
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
        getSpecificItems: jasmine.createSpy('sfCalendarService.getSpecificItems').andCallFake(function (ids, provider) {
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
        }),
        getItem: jasmine.createSpy('sfCalendarService.getItem').andCallFake(function (itemId, provider) {
            if ($q) {
                serviceResult = $q.defer();
            }

            var result = {};
            if (itemId === dataItem.Id) {
                result.Item = dataItem;
            }
            else {
                result.Item = dataItem2;
            }
            serviceResult.resolve(result);
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
        $provide.value('sfCalendarService', calendarService);

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

        //Sets default calendarService mock.
        provide.value('sfCalendarService', calendarService);
    });

    var getCalendarServiceGetItemsArgs = function () {
        var mostRecent = calendarService.getItems.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        return mostRecent.args;
    };

    var getCalendarServiceGetSpecificItemsArgs = function () {
        var mostRecent = calendarService.getSpecificItems.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        return mostRecent.args;
    };

    describe('in single selection mode', function () {
        it('[EGaneva] / should retrieve calendars from the service when the selector is opened.', function () {
            var template = "<sf-list-selector sf-calendar-selector sf-provider='provider'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            var args = getCalendarServiceGetItemsArgs();

            //Provider
            expect(args[0]).toBe('OpenAccessDataProvider');

            //Skip
            expect(args[1]).toBe(0);

            //Take
            expect(args[2]).toBe(20);

            //Filter
            expect(args[3]).toBeFalsy();
        });

        it('[EGaneva] / should retrieve selected calendars from the service when the selector is loaded.', function () {
            var template = "<sf-list-selector sf-calendar-selector sf-provider='provider' sf-selected-item-id='selectedId'/>";

            scope.selectedId = dataItem.Id;

            commonMethods.compileDirective(template, scope);

            var args = getCalendarServiceGetSpecificItemsArgs();

            //Item id
            expect(args[0]).toEqualArrayOfValues([dataItem.Id]);

            //Provider
            expect(args[1]).toBe('OpenAccessDataProvider');
        });

        it('[EGaneva] / should assign value to "selected-item" when "selected-item-id" is provided.', function () {
            var template = "<sf-list-selector sf-calendar-selector sf-selected-item='selectedItem' sf-selected-item-id='selectedId'/>";

            scope.selectedId = dataItem.Id;

            commonMethods.compileDirective(template, scope);

            expect(scope.selectedId).toBeDefined();
            expect(scope.selectedId).toEqual(dataItem.Id);

            expect(scope.selectedItem).toBeDefined();
            expect(scope.selectedItem.Id).toEqual(dataItem.Id);
        });

        it('[EGaneva] / should assign value to "selected-item-id" when "selected-item" is provided.', function () {
            var template = "<sf-list-selector sf-calendar-selector sf-selected-item='selectedItem' sf-selected-item-id='selectedId'/>";

            scope.selectedItem = dataItem;

            commonMethods.compileDirective(template, scope);

            expect(scope.selectedId).toBeDefined();
            expect(scope.selectedId).toEqual(dataItem.Id);

            expect(scope.selectedItem).toBeDefined();
            expect(scope.selectedItem.Id).toEqual(dataItem.Id);
        });

        it('[EGaneva] / should select calendar item when Done button is pressed.', function () {
            var template = "<sf-list-selector sf-calendar-selector sf-selected-item='selectedItem' sf-selected-item-id='selectedId'/>";

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

        it('[EGaneva] / should filter items when text is typed in the filter box.', function () {
            var template = "<sf-list-selector sf-calendar-selector sf-provider='provider'/>";

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

            var args = getCalendarServiceGetItemsArgs();

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

        it('[EGaneva] / should move the selected item to be first in the list of all items.', function () {
            var template = "<sf-list-selector sf-calendar-selector sf-selected-item-id='selectedId'/>";

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

        it('[EGaneva] / should mark item as selected when the dialog is opened.', function () {
            var template = "<sf-list-selector sf-calendar-selector sf-selected-item-id='selectedId'/>";

            scope.selectedId = dataItem.Id;

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog[0].Id).toEqual(dataItem.Id);
        });

        it('[EGaneva] / should select only one item in the opened dialog.', function () {
            var template = "<sf-list-selector sf-calendar-selector/>";

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

        it('[EGaneva] / should deselect the item if it is clicked and it is already selected.', function () {
            var template = "<sf-list-selector sf-calendar-selector sf-selected-item-id='selectedId'/>";

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

        it('[EGaneva] / selectButtonText and changeButtonText should be undefined when no attributes are passed.', function () {
            var template = "<sf-list-selector sf-calendar-selector />";

            commonMethods.compileDirective(template, scope);

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectButtonText).not.toBeDefined();
            expect(s.changeButtonText).not.toBeDefined();
        });

        it('[EGaneva] / should replace the select button text with the one from the attributes.', function () {
            var template = "<sf-list-selector sf-calendar-selector sf-select-button-text='Select a calendar...'/>";

            commonMethods.compileDirective(template, scope);

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectButtonText).toBeDefined();
            expect(s.selectButtonText).toBe('Select a calendar...');
            expect(s.changeButtonText).not.toBeDefined();
        });

        it('[EGaneva] / should replace the select and change buttons text with the one from the attributes.', function () {
            var template = "<sf-list-selector sf-calendar-selector sf-select-button-text='Select a calendar...' sf-change-button-text='Change the calendar...'/>";

            commonMethods.compileDirective(template, scope);

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectButtonText).toBeDefined();
            expect(s.selectButtonText).toBe('Select a calendar...');
            expect(s.changeButtonText).toBeDefined();
            expect(s.changeButtonText).toBe('Change the calendar...');
        });
    });
});