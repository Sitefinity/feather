/* Tests for sf-search-service.js */
describe('sfSearchService', function () {
    var $httpBackend;
    var dataService;
    var dataItems = {
        Items: [{
            ID: '4c003fb0-2a77-61ec-be54-ff00007864f4',
            Title: "search index 1"
        }],
        TotalCount: 1
    }; 
    
    var errorResponse = {
        Detail: 'Error'
    };

    var appPath = 'http://mysite.com:9999/myapp';

    beforeEach(module('sfServices'));

    beforeEach(module(function ($provide) {
        var serverContext = {
            getRootedUrl: function (path) {
                return appPath + '/' + path;
            },
            getUICulture: function () {
                return null;
            }
        };
        $provide.value('serverContext', serverContext);
    }));

    beforeEach(inject(function ($injector) {
        // Set up the mock http service responses
        $httpBackend = $injector.get('$httpBackend');
        dataService = $injector.get('sfSearchService');
    }));    

    beforeEach(function () {
        // Used to compare objects returned by $resource
        this.addMatchers({
            toEqualData: function (expected) {
                return angular.equals(this.actual, expected);
            }
        });
    });

    /* Helper methods */
    var assertItems = function (params) {
        var data;
        dataService.getSearchIndexes.apply(dataService, params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    };

    var assertError = function (params) {
        var data;
        dataService.getSearchIndexes.apply(dataService, params).then(function (res) {
            data = res;
        }, function (err) {
            data = err;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data.data).toEqualData(errorResponse);
    };

    /* Tests */
    it('[NPetrova] / should retrieve items without filter and paging.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Publishing/PublishingService.svc/pipes/?providerName=SearchPublishingProvider&pipeTypeName=SearchIndex&filter=&sort=Title+ASC')
        .respond(dataItems);
        
        assertItems([null, null, null]);
    });

    it('[NPetrova] / should retrieve items with paging.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Publishing/PublishingService.svc/pipes/?providerName=SearchPublishingProvider&pipeTypeName=SearchIndex&filter=&skip=30&sort=Title+ASC&take=10')
        .respond(dataItems);

        assertItems([30, 10, null]);
    });

    it('[NPetrova] / should retrieve items with filter.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Publishing/PublishingService.svc/pipes/?providerName=SearchPublishingProvider&pipeTypeName=SearchIndex&filter=(Title.ToUpper().Contains(%22index1%22.ToUpper()))&sort=Title+ASC')
        .respond(dataItems);

        assertItems([null, null, "index1"]);
    });

    it('[NPetrova] / should return error.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Publishing/PublishingService.svc/pipes/?providerName=SearchPublishingProvider&pipeTypeName=SearchIndex&filter=&skip=error&sort=Title+ASC')
        .respond(500, errorResponse);

        assertError(["error", null, null]);
    });   
});