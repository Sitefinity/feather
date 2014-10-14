/* Tests for news-item-service.js */
describe('newsItemService', function () {
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

    beforeEach(module('services'));

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
        dataService = $injector.get('newsItemService');
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
    var asserItems = function (params) {
        var data;
        dataService.getItems.apply(dataService, params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    };

    var asserItem = function (params) {
        var data;
        dataService.getItem.apply(dataService, params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    };

    var assertError = function (params) {
        var data;
        dataService.getItem.apply(dataService, params).then(function (res) {
            data = res;
        }, function (err) {
            data = err;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data.data).toEqualData(errorResponse);
    }

    /* Tests */
    it('[GMateev] / should retrieve items without filter and paging.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Content/NewsItemService.svc?filter=Visible%3D%3Dtrue+AND+Status%3D%3Dlive&itemSurrogateType=Telerik.Sitefinity.GenericContent.Model.Content&itemType=Telerik.Sitefinity.GenericContent.Model.Content&skip=0&take=20')
        .respond(dataItems);
        
        asserItems([null, 0, 20, null]);
    });

    it('[GMateev] / should retrieve items with paging.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Content/NewsItemService.svc?filter=Visible%3D%3Dtrue+AND+Status%3D%3Dlive&itemSurrogateType=Telerik.Sitefinity.GenericContent.Model.Content&itemType=Telerik.Sitefinity.GenericContent.Model.Content&skip=20&take=20')
        .respond(dataItems);

        asserItems([null, 20, 20, null]);
    });

    it('[GMateev] / should retrieve items with provider.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Content/NewsItemService.svc?filter=Visible%3D%3Dtrue+AND+Status%3D%3Dlive&itemSurrogateType=Telerik.Sitefinity.GenericContent.Model.Content&itemType=Telerik.Sitefinity.GenericContent.Model.Content&provider=OpenAccessDataProvider&skip=0&take=20')
        .respond(dataItems);

        asserItems(['OpenAccessDataProvider', 0, 20, null]);
    });

    it('[GMateev] / should retrieve items with filter.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Content/NewsItemService.svc?filter=Visible%3D%3Dtrue+AND+Status%3D%3Dlive+AND+(Title.ToUpper().Contains(%22keyword%22.ToUpper()))&itemSurrogateType=Telerik.Sitefinity.GenericContent.Model.Content&itemType=Telerik.Sitefinity.GenericContent.Model.Content&skip=0&take=20')
        .respond(dataItems);

        asserItems([null, 0, 20, 'keyword']);
    });

    it('[GMateev] / should return single item.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Content/NewsItemService.svc/4c003fb0-2a77-61ec-be54-ff00007864f4?provider=OpenAccessDataProvider&published=true')
        .respond(dataItems);

        asserItem(['4c003fb0-2a77-61ec-be54-ff00007864f4', 'OpenAccessDataProvider']);
    });

    it('[GMateev] / should return error.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Content/NewsItemService.svc/4c003fb0-2a77-61ec-be54-ff0000000000?published=true')
        .respond(500, errorResponse);

        var id = '4c003fb0-2a77-61ec-be54-ff0000000000';

        assertError([id]);
    });   
});