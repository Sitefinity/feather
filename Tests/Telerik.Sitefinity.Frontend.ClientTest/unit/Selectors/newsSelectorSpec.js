/* Tests for news selector */
describe("news selector", function () {
    var scope;

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

    var itemContext = {
        Item: dataItem
    };

    var dataItems = {
        Items: [dataItem],
        TotalCount: 1
    };

    var serviceResult;
    var $q;
    var $timeout;

    //Mock news item service. It returns promises.
    var newsItemService = {
        getItems: jasmine.createSpy('newsItemService.getItems').andCallFake(function (provider, skip, take, filter) {
            if ($q) {
                serviceResult = $q.defer();
            }

            if (filter) {
                serviceResult.resolve({
                    Items: [dataItem2],
                    TotalCount: 1
                });
            }
            else if (provider === 'flag') {
                serviceResult.resolve({
                    Items: [dataItem2, dataItem],
                    TotalCount: 2
                });
            }
            else {
                serviceResult.resolve(dataItems);
            }

            return serviceResult.promise;
        }),
        getItem: jasmine.createSpy('newsItemService.getItem').andCallFake(function () {
            serviceResult.resolve(itemContext);
            return serviceResult.promise;
        })
    };

    //This is the id of the cached templates in $templateCache. The external templates are cached by a karma/grunt preprocessor.
    var newsSelectorTemplatePath = 'Selectors/news-selector.html';
    var listSelectorTemplatePath = 'Selectors/list-selector.html';

    //Load the module responsible for the modal dialog
    beforeEach(module('modalDialog'));

    //Load themodule under test.
    beforeEach(module('selectors'));

    //Load the module that contains the cached tempaltes.
    beforeEach(module('templates'));

    beforeEach(module(function ($provide) {
        //Force angular to use the mock.
        $provide.value('newsItemService', newsItemService);
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
    }));

    beforeEach(inject(function ($templateCache) {
        //This method is called by the templateUrl property of the directive's definition object and also when including the news selector view.
        spyOn(sitefinity, 'getEmbeddedResourceUrl').andCallFake(function (assembly, url) {
            if (url.indexOf('news') >= 0) {
                return newsSelectorTemplatePath;
            }
            if (url.indexOf('list') >= 0) {
                return listSelectorTemplatePath;
            }
        });
    }));

    afterEach(function () {
        //Tear down.
        var leftOver = $('.testDiv, .modal, .modal-backdrop');
        leftOver.empty();
        leftOver.remove();

        //The selector mutates the selected item. This behaviour should be fixed.
        dataItem.Title = { Value: 'Dummy'};
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
    }

    var getNewsServiceGetItemsArgs = function () {
        var mostRecent = newsItemService.getItems.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        return mostRecent.args;
    }

    var getNewsServiceGetItemArgs = function () {
        var mostRecent = newsItemService.getItem.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        return mostRecent.args;
    }
    
    it('[GMateev] / should retrieve news items from the service when the selector is opened.', function () {
        var template = "<list-selector news-selector provider='provider'/>";

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
        var template = "<list-selector news-selector provider='provider' selected-item-id='selectedId'/>";

        scope.selectedId = '4c003fb0-2a77-61ec-be54-ff00007864f4';

        compileDirective(template);

        var args = getNewsServiceGetItemArgs();

        //Item id
        expect(args[0]).toBe('4c003fb0-2a77-61ec-be54-ff00007864f4');

        //Provider
        expect(args[1]).toBe('OpenAccessDataProvider');
    });

    it('[GMateev] / should select news item.', function () {
        var template = "<list-selector news-selector provider='provider' selected-item='selectedItem' selected-item-id='selectedId'/>";

        compileDirective(template);

        $('.openSelectorBtn').click();

        //The scope of the selector is isolated, but it's child of the scope used for compilation.
        var s = scope.$$childHead;

        //mock the call to the modal service.
        s.$modalInstance = { close: function () { }};

        expect(s.selectedItem).toBeFalsy();
        expect(s.selectedItemId).toBeFalsy();

        expect(s.items).toBeDefined();
        expect(s.items[0].Id).toEqual(dataItem.Id);

        //Select item in the selector
        s.itemClicked(0, s.items[0]);

        //Close the dialog (Done button clicked)
        s.selectItem();

        expect(s.selectedItem).toBeDefined();
        expect(s.selectedItem.Id).toEqual(dataItem.Id);

        expect(s.selectedItemId).toBeDefined();
        expect(s.selectedItemId).toEqual(dataItem.Id);
    });

    it('[GMateev] / should filter items.', function () {
        var template = "<list-selector news-selector provider='provider'/>";

        compileDirective(template);

        $('.openSelectorBtn').click();

        //The scope of the selector is isolated, but it's child of the scope used for compilation.
        var s = scope.$$childHead;

        expect(s.items).toBeDefined();
        expect(s.items[0].Id).toEqual(dataItem.Id);
        expect(s.items[0].Filter).toBeFalsy();

        //Apply filter
        s.filter.search = 'filter';
        s.reloadItems(s.filter.search);

        $timeout.flush();

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
        expect(s.filter.search).toBe('filter');
    });

    it('[GMateev] / should move the selected item to be first in the list.', function () {
        var template = "<list-selector news-selector provider='provider' selected-item-id='selectedId'/>";

        scope.selectedId = dataItem.Id;
        scope.provider = 'flag';

        compileDirective(template);

        $('.openSelectorBtn').click();

        //The scope of the selector is isolated, but it's child of the scope used for compilation.
        var s = scope.$$childHead;

        expect(s.items).toBeDefined();
        expect(s.items.length).toBe(2);
        expect(s.items[0].Id).toEqual(dataItem.Id);
        expect(s.items[1].Id).toEqual(dataItem2.Id);
    });

    it('[GMateev] / should mark item as selected in the dialog.', function () {
        var template = "<list-selector news-selector provider='provider' selected-item-id='selectedId'/>";

        scope.selectedId = '4c003fb0-2a77-61ec-be54-ff00007864f4';

        compileDirective(template);

        $('.openSelectorBtn').click();

        //The scope of the selector is isolated, but it's child of the scope used for compilation.
        var s = scope.$$childHead;

        expect(s.selectedItemInTheDialog).toBeDefined();
        expect(s.selectedItemInTheDialog.Id).toEqual(dataItem.Id);
    });
});