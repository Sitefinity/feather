/* Tests for sf-search-service.js */
describe('sfMultiSiteService', function () {
    var $httpBackend;
    var dataService;
    var dataItems = {
        Items: [{
            "CultureDisplayNames": null,
            "Id": "344b7567-6965-43c9-88b6-028bbe6f4c9d",
            "IsAllowedConfigureModules": false,
            "IsAllowedCreateEdit": false,
            "IsAllowedSetPermissions": false,
            "IsAllowedStartStop": false,
            "IsDefault": true,
            "IsDeleteable": false,
            "IsOffline": false,
            "Name": "\/",
            "SiteMapRootNodeId": "f669d9a7-009d-4d83-ddaa-000000000002",
            "SiteUrl": null,
            "UIStatus": "Online"
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
            },
            getCurrentUserId: function () {
                return '36e9e47f-0d78-6425-ae98-ff0000fc9faf';
            }
        };
        $provide.value('serverContext', serverContext);
    }));

    beforeEach(inject(function ($injector) {
        // Set up the mock http service responses
        $httpBackend = $injector.get('$httpBackend');
        dataService = $injector.get('sfMultiSiteService');
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
        dataService.getSitesForUserPromise.apply(dataService, params).then(function (res) {
            data = res;
        });
        
        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
        expect(dataService.getSites()).toEqualData(dataItems.Items);
    };

    var assertError = function (params) {
        var data;
        dataService.getSitesForUserPromise.apply(dataService, params).then(function (res) {
            data = res;
        }, function (err) {
            data = err;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data.data).toEqualData(errorResponse);
    };

    /* Tests */
    it('[EGaneva] / should retrieve items.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Multisite/Multisite.svc/user/36e9e47f-0d78-6425-ae98-ff0000fc9faf/sites/?sortExpression=Name')
        .respond(dataItems);
        
        assertItems([{ sortExpression: 'Name' }]);
    });

    it('[EGaneva] / should retrieve items with paging.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Multisite/Multisite.svc/user/36e9e47f-0d78-6425-ae98-ff0000fc9faf/sites/?skip=2&take=3')
        .respond(dataItems);

        assertItems([{ skip: 2, take: 3 }]);
    });

    it('[EGaneva] / should return error.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Multisite/Multisite.svc/user/36e9e47f-0d78-6425-ae98-ff0000fc9faf/sites/?skip=error')
        .respond(500, errorResponse);

        assertError([{ skip: 'error'}]);
    });

    it('[EGaneva] / should retrieve site by rootnodeid.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Multisite/Multisite.svc/user/36e9e47f-0d78-6425-ae98-ff0000fc9faf/sites/?sortExpression=Name')
                .respond(dataItems);

        assertItems([{ sortExpression: 'Name' }]);

        var site = dataService.getSiteByRootNoteId('f669d9a7-009d-4d83-ddaa-000000000002');

        expect(site).toEqualData(dataItems.Items[0]);
    });

    it('[EGaneva] / should add handler.', function () {
        var spyObject = { init: function () { } };

        spyOn(spyObject, 'init');
        dataService.addHandler(spyObject.init);

        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Multisite/Multisite.svc/user/36e9e47f-0d78-6425-ae98-ff0000fc9faf/sites/?sortExpression=Name')
        .respond(dataItems);

        assertItems([{ sortExpression: 'Name' }]);

        expect(spyObject.init).toHaveBeenCalled();
    });
});