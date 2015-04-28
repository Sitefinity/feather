describe('sfGenericItemsService', function () {
    beforeEach(module('sfServices'));

    var appPath = 'http://mysite.com:9999/myapp';
    var dataServiceBaseUrl = 'http://mysite.com:9999/myapp/Sitefinity/Services/Common/GenericItemsService.svc';
    var dummyItemType = "Telerik.Sitefinity.DynamicTypes.Model.TestModule.SomeType";

    var dataItems = {
        Items: [{
            Id: '4c003fb0-2a77-61ec-be54-ff00007864f4'
        }],
        TotalCount: 1
    }; 
    
    var errorResponse = {
        Detail: 'Error'
    };

    var dataService;

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
        dataService = $injector.get('sfGenericItemsService');
    }));

    /* Helper methods */
    var assertItems = function (params) {
        var data;
        dataService.getItems(params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    };

    var expectGetItemsServiceCall = function (params) {
        var filterParam = '';
        if(params.filter) {
            filterParam = "filter=" + params.filter + "&";
        }

        var providerParam = "";
        if (params.provider) {
            providerParam = "provider=" + params.provider + "&";
        }

        var allProvidersParam = "";
        if (params.allProviders) {
            allProvidersParam = "allProviders=" + params.allProviders + "&";
        }

        var ignoreAdminUsersParam = "";
        if (params.ignoreAdminUsers) {
            ignoreAdminUsersParam = "ignoreAdminUsers=" + params.ignoreAdminUsers + "&";
        }

        var servicePathPattern = '/?{0}{1}{2}itemSurrogateType={3}&itemType={4}&{5}skip={6}&take={7}';
        var url = dataServiceBaseUrl + servicePathPattern.format(
            allProvidersParam,
            ignoreAdminUsersParam,
            filterParam,
            params.itemSurrogateType,
            params.itemType,
            providerParam,            
            params.skip,
            params.take);

        $httpBackend.expectGET(url).respond(dataItems);
    };

    it('[GeorgiMateev] / should retrieve items with paging.', function () {
        var params = {
            skip: 20,
            take: 20,
            itemType: dummyItemType,
            itemSurrogateType: dummyItemType,
            allProviders: true
        };

        expectGetItemsServiceCall(params);

        assertItems(params);
    });

    it('[GeorgiMateev] / should retrieve items with provider.', function () {
        var params = {
            skip: 20,
            take: 20,
            itemType: dummyItemType,
            itemSurrogateType: dummyItemType,
            provider: 'OpenAccess'
        };

        expectGetItemsServiceCall(params);

        assertItems(params);
    });

    it('[GeorgiMateev] / should retrieve items without admin users.', function () {
        var params = {
            skip: 20,
            take: 20,
            itemType: dummyItemType,
            itemSurrogateType: dummyItemType,
            ignoreAdminUsers: true,
            allProviders: true,
        };

        expectGetItemsServiceCall(params);

        assertItems(params);
    });

    it('[GeorgiMateev] / should retrieve items with filter.', function () {
        var params = {
            skip: 20,
            take: 20,
            itemType: dummyItemType,
            itemSurrogateType: dummyItemType,
            filter: 'someFilter'
        };

        expectGetItemsServiceCall(params);

        assertItems(params);
    });
});