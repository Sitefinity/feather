/* Tests for news selector */
describe("news selector", function () {
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

    //Mock news item service. It returns promises.
    var newsItemService = {
        getItems: jasmine.createSpy('sfNewsItemService.getItems').andCallFake(function (provider, skip, take, filter) {
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
        getSpecificItems: jasmine.createSpy('sfNewsItemService.getSpecificItems').andCallFake(function (ids, provider) {
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
        getItem: jasmine.createSpy('sfNewsItemService.getItem').andCallFake(function (itemId, provider) {
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

    var newsItemServiceMockReturnMoreThan5Items = {
        getSpecificItems: jasmine.createSpy('sfNewsItemService.getSpecificItems').andCallFake(function (ids, provider) {
            if ($q) {
                serviceResult = $q.defer();
            }
            serviceResult.resolve(customDataItems);

            return serviceResult.promise;
        }),
    };

    //This is the id of the cached templates in $templateCache. The external templates are cached by a karma/grunt preprocessor.
    var newsSelectorTemplatePath = 'client-components/selectors/news/sf-news-selector.html';
    var listSelectorTemplatePath = 'client-components/selectors/common/sf-list-selector.html';
    var bubblesSelectionTemplatePath = 'client-components/selectors/common/sf-bubbles-selection.html';
    var listGroupSelectionTemplatePath = 'client-components/selectors/common/sf-list-group-selection.html';

    //Load the module responsible for the modal dialog
    beforeEach(module('modalDialog'));

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

    beforeEach(module(function ($provide) {
        //Force angular to use the mock.
        $provide.value('sfNewsItemService', newsItemService);

        provide = $provide;
    }));

    beforeEach(inject(function ($rootScope, $httpBackend, _$q_, $templateCache, _$timeout_) {
        //Build the scope with whom the directive will be compiled.
        scope = $rootScope.$new();
        scope.provider = 'OpenAccessDataProvider';

        $q = _$q_;
        $timeout = _$timeout_;

        serviceResult = _$q_.defer();

        //Prevent failing of the template request.
        $httpBackend.whenGET(listSelectorTemplatePath);
        $httpBackend.whenGET(newsSelectorTemplatePath).respond({});
        $httpBackend.whenGET(bubblesSelectionTemplatePath).respond({});
        $httpBackend.whenGET(listGroupSelectionTemplatePath).respond({});
    }));

    beforeEach(function () {
        this.addMatchers({
            // Used to compare arrays of primitive values
            toEqualArrayOfValues: function (expected) {
                var valid = true;
                for (var i = 0; i < expected.length; i++) {
                    if (expected[i] !== this.actual[i]) {
                        valid = false;
                        break;
                    }
                }
                return valid;
            },

            // Used to compare arrays of data items with Id and Title
            toEqualArrayOfDataItems: function (expected) {
                var valid = true;
                for (var i = 0; i < expected.length; i++) {
                    var id = this.actual[i].item ? this.actual[i].item.Id : this.actual[i].Id;
                    var title = this.actual[i].item ? this.actual[i].item.Title : this.actual[i].Title;
                    if (expected[i].Id !== id || expected[i].Title !== title) {
                        valid = false;
                        break;
                    }
                }
                return valid;
            },
        });
    });

    afterEach(function () {
        //Tear down.
        var leftOver = $('.testDiv, .modal, .modal-backdrop');
        leftOver.empty();
        leftOver.remove();

        //The selector mutates the selected item when it is retrieved from the service.
        dataItem.Title = { Value: 'Dummy' };
        dataItem2.Title = { Value: 'Filtered' };

        //Sets default newsItemService mock.
        provide.value('sfNewsItemService', newsItemService);
    });

    /* Helper methods */
    var compileDirective = function (template, container) {
        var cntr = container || 'body';

        inject(function ($compile) {
            var directiveElement = $compile(template)(scope);
            $(cntr).append($('<div/>').addClass('testDiv')
                .append(directiveElement));
        });

        // $digest is necessary to finalize the directive generation
        scope.$digest();
    };

    var getNewsServiceGetItemsArgs = function () {
        var mostRecent = newsItemService.getItems.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        return mostRecent.args;
    };

    var getNewsServiceGetItemArgs = function () {
        var mostRecent = newsItemService.getItem.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        return mostRecent.args;
    };

    var getNewsServiceGetSpecificItemsArgs = function () {
        var mostRecent = newsItemService.getSpecificItems.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        return mostRecent.args;
    }

    describe('in single selection mode', function () {
        it('[GMateev] / should retrieve news items from the service when the selector is opened.', function () {
            var template = "<sf-list-selector sf-news-selector provider='provider'/>";

            compileDirective(template);

            $('.openSelectorBtn').click();

            var args = getNewsServiceGetItemsArgs();

            //Provider
            expect(args[0]).toBe('OpenAccessDataProvider');

            //Skip
            expect(args[1]).toBe(0);

            //Take
            expect(args[2]).toBe(20);

            //Filter
            expect(args[3]).toBeFalsy();
        });

        it('[GMateev] / should retrieve selected news item from the service when the selector is loaded.', function () {
            var template = "<sf-list-selector sf-news-selector provider='provider' selected-item-id='selectedId'/>";

            scope.selectedId = dataItem.Id;

            compileDirective(template);

            var args = getNewsServiceGetSpecificItemsArgs();

            //Item id
            expect(args[0]).toEqualArrayOfValues([dataItem.Id]);

            //Provider
            expect(args[1]).toBe('OpenAccessDataProvider');
        });

        it('[GMateev] / should assign value to "selected-item" when "selected-item-id" is provided.', function () {
            var template = "<sf-list-selector sf-news-selector selected-item='selectedItem' selected-item-id='selectedId'/>";

            scope.selectedId = dataItem.Id;

            compileDirective(template);

            expect(scope.selectedId).toBeDefined();
            expect(scope.selectedId).toEqual(dataItem.Id);

            expect(scope.selectedItem).toBeDefined();
            expect(scope.selectedItem.Id).toEqual(dataItem.Id);
        });

        it('[GMateev] / should assign value to "selected-item-id" when "selected-item" is provided.', function () {
            var template = "<sf-list-selector sf-news-selector selected-item='selectedItem' selected-item-id='selectedId'/>";

            scope.selectedItem = dataItem;

            compileDirective(template);

            expect(scope.selectedId).toBeDefined();
            expect(scope.selectedId).toEqual(dataItem.Id);

            expect(scope.selectedItem).toBeDefined();
            expect(scope.selectedItem.Id).toEqual(dataItem.Id);
        });

        it('[GMateev] / should select news item when Done button is pressed.', function () {
            var template = "<sf-list-selector sf-news-selector selected-item='selectedItem' selected-item-id='selectedId'/>";

            compileDirective(template);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            //mock the call to the modal service.
            s.$modalInstance = { close: function () { } };

            expect(s.selectedItem).toBeFalsy();
            expect(s.selectedItemId).toBeFalsy();

            expect(s.items).toBeDefined();
            expect(s.items[0].Id).toEqual(dataItem.Id);
            expect(s.items[1].Id).toEqual(dataItem2.Id);

            //Select item in the selector
            s.itemClicked(0, s.items[0]);

            expect(s.selectedItem).toBeFalsy();
            expect(s.selectedItemId).toBeFalsy();

            //Close the dialog (Done button clicked)
            s.doneSelecting();

            expect(s.selectedItem).toBeDefined();
            expect(s.selectedItem.Id).toEqual(dataItem.Id);

            expect(s.selectedItemId).toBeDefined();
            expect(s.selectedItemId).toEqual(dataItem.Id);
        });

        it('[GMateev] / should filter items when text is typed in the filter box.', function () {
            var template = "<sf-list-selector sf-news-selector provider='provider'/>";

            compileDirective(template);

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

            var args = getNewsServiceGetItemsArgs();

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

        it('[GMateev] / should move the selected item to be first in the list of all items.', function () {
            var template = "<sf-list-selector sf-news-selector selected-item-id='selectedId'/>";

            scope.selectedId = dataItem2.Id;

            compileDirective(template);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.items).toBeDefined();
            expect(s.items.length).toBe(2);
            expect(s.items[0].Id).toEqual(dataItem2.Id);
            expect(s.items[1].Id).toEqual(dataItem.Id);
        });

        it('[GMateev] / should mark item as selected when the dialog is opened.', function () {
            var template = "<sf-list-selector sf-news-selector selected-item-id='selectedId'/>";

            scope.selectedId = dataItem.Id;

            compileDirective(template);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog[0].item.Id).toEqual(dataItem.Id);
        });

        it('[GMateev] / should select only one item in the opened dialog.', function () {
            var template = "<sf-list-selector sf-news-selector/>";

            compileDirective(template);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            //Select item in the selector
            s.itemClicked(0, s.items[0]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog[0].item.Id).toEqual(dataItem.Id);

            //Select second item in the selector
            s.itemClicked(1, s.items[1]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog[0].item.Id).toEqual(dataItem2.Id);
        });

        it('[GMateev] / should deselect the item if it is clicked and it is already selected.', function () {
            var template = "<sf-list-selector sf-news-selector selected-item-id='selectedId'/>";

            scope.selectedId = dataItem.Id;

            compileDirective(template);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog[0].item.Id).toEqual(dataItem.Id);

            //Select item in the selector
            s.itemClicked(0, s.items[0]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(0);
        });
    });

    describe('in multi selection mode', function () {
        var items = [dataItem, dataItem2];
        var ids = [dataItem.Id, dataItem2.Id];

        it('[GMateev] / should retrieve news items from the service when the selector is opened.', function () {
            var template = "<sf-list-selector sf-news-selector multiselect='true' provider='provider'/>";

            compileDirective(template);

            $('.openSelectorBtn').click();

            var args = getNewsServiceGetItemsArgs();

            //Provider
            expect(args[0]).toBe('OpenAccessDataProvider');

            //Skip
            expect(args[1]).toBe(0);

            //Take
            expect(args[2]).toBe(20);

            //Filter
            expect(args[3]).toBeFalsy();
        });

        it('[GMateev] / should retrieve selected news items from the service when the selector is loaded.', function () {
            var template = "<sf-list-selector sf-news-selector multiselect='true' provider='provider' selected-ids='selectedIds'/>";

            scope.selectedIds = ids;

            compileDirective(template);

            var args = getNewsServiceGetSpecificItemsArgs();

            //Item id
            expect(args[0]).toEqualArrayOfValues([dataItem.Id, dataItem2.Id]);

            //Provider
            expect(args[1]).toBe('OpenAccessDataProvider');
        });

        it('[GMateev] / should assign value to "selected-items" when "selected-ids" are provided.', function () {
            var template = "<sf-list-selector sf-news-selector multiselect='true' selected-items='selectedItems' selected-ids='selectedIds'/>";

            scope.selectedIds = ids;

            compileDirective(template);

            expect(scope.selectedIds).toBeDefined();
            expect(scope.selectedIds.length).toBe(2);
            expect(scope.selectedIds).toEqualArrayOfValues(ids);

            expect(scope.selectedItems).toBeDefined();
            expect(scope.selectedItems.length).toEqual(2);
            expect(scope.selectedItems).toEqualArrayOfDataItems(items);
        });

        it('[GMateev] / should assign value to "selected-ids" when "selected-items" is provided.', function () {
            var template = "<sf-list-selector sf-news-selector multiselect='true' selected-items='selectedItems' selected-ids='selectedIds'/>";

            scope.selectedItems = items;

            compileDirective(template);

            expect(scope.selectedIds).toBeDefined();
            expect(scope.selectedIds.length).toBe(2);
            expect(scope.selectedIds).toEqualArrayOfValues(ids);

            expect(scope.selectedItems).toBeDefined();
            expect(scope.selectedItems.length).toEqual(2);
            expect(scope.selectedItems).toEqualArrayOfDataItems(items);
        });

        it('[GMateev] / should select news items when Done button is pressed.', function () {
            var template = "<sf-list-selector sf-news-selector multiselect='true' selected-items='selectedItems' selected-ids='selectedIds'/>";

            compileDirective(template);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            //mock the call to the modal service.
            s.$modalInstance = { close: function () { } };

            expect(s.selectedItems).toBeFalsy();
            expect(s.selectedIds).toBeFalsy();

            expect(s.items).toBeDefined();
            expect(s.items[0].Id).toEqual(dataItem.Id);
            expect(s.items[1].Id).toEqual(dataItem2.Id);

            //Select item in the selector
            s.itemClicked(0, s.items[0]);
            s.itemClicked(1, s.items[1]);

            expect(s.selectedItems).toBeFalsy();
            expect(s.selectedIds).toBeFalsy();

            //Close the dialog (Done button clicked)
            s.doneSelecting();

            scope.$digest();

            expect(scope.selectedIds).toBeDefined();
            expect(scope.selectedIds.length).toBe(2);
            expect(scope.selectedIds).toEqualArrayOfValues(ids);

            expect(scope.selectedItems).toBeDefined();
            expect(scope.selectedItems.length).toEqual(2);
            expect(scope.selectedItems).toEqualArrayOfDataItems(items);
        });

        it('[GMateev] / should mark items as selected when the dialog is opened.', function () {
            var template = "<sf-list-selector sf-news-selector multiselect='true' selected-ids='selectedIds'/>";

            scope.selectedIds = ids;

            compileDirective(template);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(2);
            expect(s.selectedItemsInTheDialog).toEqualArrayOfDataItems(items);
        });

        it('[GMateev] / should select many items in the opened dialog.', function () {
            var template = "<sf-list-selector sf-news-selector multiselect='true'/>";

            compileDirective(template);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            //Select item in the selector
            s.itemClicked(0, s.items[0]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog[0].item.Id).toEqual(dataItem.Id);

            //Select second item in the selector
            s.itemClicked(1, s.items[1]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(2);
            expect(s.selectedItemsInTheDialog[1].item.Id).toEqual(dataItem2.Id);
        });

        it('[GMateev] / should deselect an item if it is clicked and it is already selected.', function () {
            var template = "<sf-list-selector sf-news-selector multiselect='true' selected-ids='selectedIds'/>";

            scope.selectedIds = ids;

            compileDirective(template);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(2);
            expect(s.selectedItemsInTheDialog).toEqualArrayOfDataItems(items);

            //Select item in the selector
            s.itemClicked(0, s.items[0]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog).toEqualArrayOfDataItems([dataItem2]);
        });
    });

    describe('in multi selection mode and more than 5 items selected by default', function () {
        it('[manev] / should show link indicating that there are 5 items more.', function () {

            provide.value('sfNewsItemService', newsItemServiceMockReturnMoreThan5Items);

            var ids = customDataItems.Items.map(function (i) { return i.Id; });

            var template = "<sf-list-selector sf-news-selector multiselect='true' selected-items='selectedItems' selected-ids='selectedIds'/>";

            scope.selectedIds = ids;
            compileDirective(template);

            expect(scope.selectedItems).toBeDefined();
            expect(scope.selectedItems.length).toBe(10);
            expect(scope.selectedItems).toEqualArrayOfDataItems(customDataItems.Items);

            var moreLink = $(".small");

            expect(moreLink.length).toBe(1);
            expect(moreLink.html()).toBe("and 5 more");
            expect(moreLink.css("display")).not.toBe("none");
        });

        it('[manev] / should show link indicating that there are 5 items more.', function () {

            var ids = [dataItem.Id, dataItem2.Id];

            var template = "<sf-list-selector sf-news-selector multiselect='true' selected-items='selectedItems' selected-ids='selectedIds'/>";

            scope.selectedIds = ids;
            compileDirective(template);

            var moreLink = $(".small");
            expect(moreLink.css("display")).toBe("none");
        });
    });
});