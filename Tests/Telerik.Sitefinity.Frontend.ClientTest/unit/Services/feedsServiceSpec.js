/* Tests for sf-feeds-service.js */
describe('sfFeedsService', function () {
    var $httpBackend;
    var dataService;
    var dataItems = {
        Items: [{
            ID: '4c003fb0-2a77-61ec-be54-ff00007864f4',
            Id: '4c003fb0-2a77-61ec-be54-ff00007864f4',
            Title: 'Feed title'
        },
        {
            ID: '4c013fb0-2a77-61ec-be54-ff00007864f4',
            Id: '4c013fb0-2a77-61ec-be54-ff00007864f4',
            Title: 'Feed title2'
        }],
        TotalCount: 2
    };

    var filterItems = {
        Items: [{
            ID: '4c013fb0-2a77-61ec-be54-ff00007864f4',
            Id: '4c013fb0-2a77-61ec-be54-ff00007864f4',
            Title: 'Feed title2'
        }],
        TotalCount: 1
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
        dataService = $injector.get('sfFeedsService');
    }));

    /* Helper methods */
    var assertItems = function (params) {
        var data;
        dataService.getItems.apply(dataService, params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    };

    var assertSpecificItems = function (params) {
        var data;
        dataService.getSpecificItems.apply(dataService, params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(filterItems);
    };

    /* Tests */
    it('[Manev] / should retrieve items.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Publishing/PublishingService.svc/pipes/?filter=&pipeTypeName=RSSOutboundPipe&providerName=OAPublishingProvider&skip=0&take=20')
                    .respond(dataItems);

        assertItems([null, 0, 20, null]);
    });

    it('[Manev] / should retrieve specific items.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Publishing/PublishingService.svc/pipes/?filter=(Id.Equals(%224c013fb0-2a77-61ec-be54-ff00007864f4%22))&pipeTypeName=RSSOutboundPipe&providerName=OAPublishingProvider')
                    .respond(filterItems);

        assertSpecificItems([['4c013fb0-2a77-61ec-be54-ff00007864f4'], null]);
    });
});
