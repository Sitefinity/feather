/* Tests for news selector */
describe("news selector", function () {
    var scope;

    //Will be returned from the service mock.
    var dataItems = {
        Items: [{
            Id: '4c003fb0-2a77-61ec-be54-ff00007864f4'
        }],
        TotalCount: 1
    };

    var serviceResult;

    //Mock news item service. It returns promises.
    var newsItemService = {
        getItems: jasmine.createSpy('newsItemService.getItems').andCallFake(function () {
            serviceResult.resolve(dataItems);
            return serviceResult.promise;
        }),
        getItem: jasmine.createSpy('newsItemService.getItem').andCallFake(function () {
            serviceResult.resolve(dataItems);
            return serviceResult.promise;
        }),
    };

    //This is the id of the cached templates in $templateCache. The external templates are cached by a karma/grunt preprocessor.
    var newsSelectorTemplatePath = 'Selectors/news-selector.html';
    var listSelectorTemplatePath = 'Selectors/list-selector.html';

    //Load themodule under test.
    beforeEach(module('selectors'));

    //Load the module that contains the cached tempaltes.
    beforeEach(module('templates'));

    beforeEach(module(function ($provide) {
        //Force angular to use the mock.
        $provide.value('newsItemService', newsItemService);
    }));

    beforeEach(inject(function ($rootScope, $httpBackend, $q, $templateCache) {
        //Build the scope with whom the directive will be compiled.
        scope = $rootScope.$new();
        scope.provider = 'OpenAccessDataProvider';

        serviceResult = $q.defer();

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
        $('.testDiv').empty();
        $('.testDiv').remove();
    });

    /* Helper methods */
    var compileDirective = function (template, container) {
        var cntr = container || 'body';

        inject(function ($compile) {
            directiveElement = $compile(template)(scope);
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
});