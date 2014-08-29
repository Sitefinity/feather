/* Tests for content selector */
describe("content selector", function () {
    var scope;
    var dataItems = {
        Items: [{
            Id: '4c003fb0-2a77-61ec-be54-ff00007864f4'
        }],
        TotalCount: 1
    };

    var serviceResult;

    //Mock generic data service. It returns promises.
    var dataService = {
        getItems: jasmine.createSpy('genericDataService.getItems').andCallFake(function () {
            serviceResult.resolve(dataItems);
            return serviceResult.promise;
        }),
        getItem: jasmine.createSpy('genericDataService.getItem').andCallFake(function () {
            serviceResult.resolve(dataItems);
            return serviceResult.promise;
        }),
    };

    //This is the id of the cached template in $templateCache. The external templates are cached by a karma/grunt preprocessor.
    var templatePath = 'Selectors/content-selector.html';

    beforeEach(module('selectors'));
    beforeEach(module('templates'));

    beforeEach(module(function ($provide) {
        $provide.value('genericDataService', dataService);
    }));

    beforeEach(inject(function ($rootScope, $httpBackend, $q) {
        scope = $rootScope.$new();
        serviceResult = $q.defer();

        //Prevent failing of the template request.
        $httpBackend.whenGET(templatePath);
    }));

    beforeEach(inject(function ($templateCache) {
        //This method is called by the templateUrl property of the directive's definition object.
        spyOn(sitefinity, 'getEmbeddedResourceUrl').andCallFake(function () {
            return templatePath;
        });
    }));

    afterEach(function () {
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

    var getDataServiceGetItemsArgs = function () {
        var mostRecent = dataService.getItems.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        return mostRecent.args;
    }
    
    it('[GMateev] / should retrieve data items from the service when the selector is opened.', function () {
        var template = "<content-selector item-type='Telerik.Sitefinity.News.Model.NewsItem'/>";

        compileDirective(template);

        $('#openSelectorBtn').click();

        var args = getDataServiceGetItemsArgs();
        //ItemType
        expect(args[0]).toBe('Telerik.Sitefinity.News.Model.NewsItem');

        //ItemProvider
        expect(args[1]).toBeUndefined();

        //Skip
        expect(args[2]).toBe(0);

        //Take
        expect(args[3]).toBe(20);

        //Filter
        expect(args[4]).toBeFalsy();
    });
});