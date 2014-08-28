/* Tests for generic-data-service.js */
describe('genericDataService', function () {
    beforeEach(module('services'));

    //Mock sitefinity global variable
    beforeEach(function () {
        spyOn(sitefinity, 'getRootedUrl').andCallFake(function (path) {
            if (path.length > 0 && path.charAt(0) === '/') {
                path = path.substring(1, path.length);
            }

            return 'http://mysite.com:9999/myapp/' + path;
        });
    });

    var $httpBackend;
    var genericDataService;
    var dataItems = {
        Items: [{
            Id: '4c003fb0-2a77-61ec-be54-ff00007864f4'
        }],
        TotalCount: 1
    }; 
    
    var errorResponse = {
        ResponseStatus: {
            Message: "Not found."
        }
    };

    beforeEach(inject(function ($injector) {
        // Set up the mock http service responses
        $httpBackend = $injector.get('$httpBackend');
        genericDataService = $injector.get('genericDataService');
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
        genericDataService.getItems.apply(genericDataService, params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    };

    var asserItem = function (params) {
        var data;
        genericDataService.getItem.apply(genericDataService, params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    };

    var asserError = function (params) {
        var data;
        genericDataService.getItems.apply(genericDataService, params).then(function (res) {
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
        $httpBackend.expectGET('http://mysite.com:9999/myapp/restapi/sitefinity/generic-data/data-items?ItemType=Telerik.Sitefinity.News.Model.NewsItem&filter=VISIBLE+%3D+true+AND+STATUS+%3D+LIVE')
        .respond(dataItems);
        var itemType = 'Telerik.Sitefinity.News.Model.NewsItem';
        
        asserItems([itemType]);
    });

    it('[GMateev] / should retrieve items with paging.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/restapi/sitefinity/generic-data/data-items?ItemType=Telerik.Sitefinity.News.Model.NewsItem&filter=VISIBLE+%3D+true+AND+STATUS+%3D+LIVE&skip=20&take=20')
        .respond(dataItems);

        var itemType = 'Telerik.Sitefinity.News.Model.NewsItem';

        asserItems([itemType, null, 20, 20]);
    });

    it('[GMateev] / should retrieve items with provider.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/restapi/sitefinity/generic-data/data-items?ItemProvider=default&ItemType=Telerik.Sitefinity.News.Model.NewsItem&filter=VISIBLE+%3D+true+AND+STATUS+%3D+LIVE')
        .respond(dataItems);

        var itemType = 'Telerik.Sitefinity.News.Model.NewsItem';

        asserItems([itemType, "default"]);
    });

    it('[GMateev] / should retrieve items with filter.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/restapi/sitefinity/generic-data/data-items?ItemType=Telerik.Sitefinity.News.Model.NewsItem&filter=VISIBLE+%3D+true+AND+STATUS+%3D+LIVE+AND+(Title.ToUpper().Contains(%22testfilter%22.ToUpper()))')
        .respond(dataItems);

        var itemType = 'Telerik.Sitefinity.News.Model.NewsItem';

        asserItems([itemType, null, null, null, 'testfilter']);
    });

    it('[GMateev] / should return single item.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/restapi/sitefinity/generic-data/data-items?ItemId=4c003fb0-2a77-61ec-be54-ff00007864f4&ItemType=Telerik.Sitefinity.News.Model.NewsItem')
        .respond(dataItems);

        var id = '4c003fb0-2a77-61ec-be54-ff00007864f4';
        var itemType = 'Telerik.Sitefinity.News.Model.NewsItem';

        asserItem([id, itemType]);
    });

    it('[GMateev] / should return error.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/restapi/sitefinity/generic-data/data-items?ItemType=notfound&filter=VISIBLE+%3D+true+AND+STATUS+%3D+LIVE')
        .respond(404, errorResponse);

        var itemType = 'notfound';

        asserError([itemType]);
    });   
});