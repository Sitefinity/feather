/* Tests for sf-lists-service.js */
describe('sfListsService', function () {
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
        dataService = $injector.get('sfListsService');
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

    /* Tests */
    it('[Manev] / should retrieve items.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Lists/ListService.svc/?filter=&skip=0&sortExpression=DateCreated+DESC&take=20')
        .respond(dataItems);
        
        assertItems([null, 0, 20, null]);
    });

    it('[Manev] / should retrieve specific items.', function () {
        $httpBackend.expectPUT('http://mysite.com:9999/myapp/restapi/lists-api/items/')
        .respond(dataItems);

        assertSpecificItems([null, 0, 20, null]);
    });
});
