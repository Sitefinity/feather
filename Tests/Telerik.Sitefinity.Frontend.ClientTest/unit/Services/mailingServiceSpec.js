/* Tests for sf-mailing-list-service.js */
describe('sfMailingListService', function () {
    var $httpBackend;
    var dataService;
    var dataItems = {
        Items: [{
            Id: '4c003fb0-2a77-61ec-be54-ff00007864f4'
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
        dataService = $injector.get('sfMailingListService');
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

        expect(data).toEqualData(dataItems);
    };

    var assertItem = function (params) {
        var data;
        dataService.getItem.apply(dataService, params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    };

    /* Tests */
    it('[Manev] / should retrieve all items.', function () {

        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Newsletters/MailingList.svc/?filter=(Title.ToUpper().Contains(%22search%22.ToUpper()))&provider=provider&skip=0&sortExpression=sortExpression&take=20')
                    .respond(dataItems);

        assertItems(['provider', 0, 20, 'search', 'sortExpression']);
    });

    it('[Manev] / should retrieve specific items.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Newsletters/MailingList.svc/?filter=(Id%3D4c003fb0-2a77-61ec-be54-ff00007864f4)&provider=provider&skip=0&take=100')
                    .respond(dataItems);

        assertSpecificItems([['4c003fb0-2a77-61ec-be54-ff00007864f4'], 'provider']);
    });

    it('[Manev] / should retrieve item.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Newsletters/MailingList.svc/4c003fb0-2a77-61ec-be54-ff00007864f4/?provider=provider')
                    .respond(dataItems);

        assertItem(['4c003fb0-2a77-61ec-be54-ff00007864f4', 'provider']);
    });
});
