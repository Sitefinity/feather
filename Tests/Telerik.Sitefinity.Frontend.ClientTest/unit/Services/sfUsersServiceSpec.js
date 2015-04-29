describe('tests for sfUsersService', function () {
    beforeEach(module('sfServices'));

    var appPath = 'http://mysite.com:9999/myapp';
    var serviceBaseUrl = 'http://mysite.com:9999/myapp/Sitefinity/Services/Security/Users.svc';

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

    var identifier = 'UserName';

    var $httpBackend;
    var service;
    var serviceHelper;

    //Will be returned from the service mock.
    var dataItem = {
        Id: '4c003fb0-2a77-61ec-be54-ff00007864f4',
        Title: { Value: 'Dummy' }
    };

    var dataItem2 = {
        Id: '4c003fb0-2a77-61ec-be54-ff11117864f4',
        Title: { Value: 'Filtered' },
        Filter: true
    };

    var dataItems = {
        Items: [dataItem, dataItem2],
        TotalCount: 2
    };

    var errorResponse = {
        Detail: 'Error'
    };

    beforeEach(inject(function ($injector) {
        // Set up the mock http service responses
        $httpBackend = $injector.get('$httpBackend');
        service = $injector.get('sfUsersService');
        serviceHelper = $injector.get('serviceHelper');
    }));

    /* Helper methods */
    var assertItems = function (params) {
        var data;
        service.getUsers.apply(service, params).promise.then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    };

    var assertProviders = function () {
        var data;
        service.getUserProviders.apply(service).promise.then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualArrayOfObjects({
            Items: [{
                UserProviderName: 'user'
            }]
        }, 'UserProviderName');
    };

    var assertSpecificItems = function (params) {
        var data;
        service.getSpecificUsers.apply(service, params).promise.then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualArrayOfObjects(dataItems, 'Id');
    };

    var assertError = function (params) {
        var data;
        service.getUsers.apply(service, params).promise.then(function (res) {
            data = res;
        }, function (err) {
            data = err;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data.error).toEqualData(errorResponse);
    };

    var getFilter = function (search) {
        if (search) {
            return "(" + identifier + ".ToUpper().Contains(%22" + search + "%22.ToUpper()))";
        }

        return "";
    };

    var constructGetItemsServiceUrl = function (provider, skip, take, search) {
        var filter = getFilter(search);
        var filterParam = "filter=" + filter + "&";

        var providerParam = "";
        if (provider) {
            providerParam = "provider=" + provider + "&";
        }

        var forAllProviders = provider ? false : true;

        var servicePathPattern = '/?{0}forAllProviders={1}&{2}skip={3}&take={4}';
        var url = serviceBaseUrl + servicePathPattern.format(filterParam, forAllProviders, providerParam, skip, take);

        return url;
    };

    var expectGetItemsServiceCall = function (provider, skip, take, search) {
        var url = constructGetItemsServiceUrl(provider, skip, take, search);

        $httpBackend.expectGET(url).respond(dataItems);
    };

    var expectGetSpecificItemsServiceCall = function (ids, provider) {
        var filter = serviceHelper.filterBuilder()
                    .specificItemsFilter(ids)
                    .getFilter();

        // Achieve similar encoding as the one in the Angular's $resource.
        var encodedFilter = encodeURIComponent(filter).replace(/%20/g, '+');

        var forAllProviders = provider ? false : true;

        var servicePathPattern = '/?filter={0}&forAllProviders={1}&provider={2}';
        var url = serviceBaseUrl + servicePathPattern.format(encodedFilter, forAllProviders, provider);

        $httpBackend.expectGET(url).respond({
            Items: [{
                Id: '4c003fb0-2a77-61ec-be54-ff00007864f4'
            },
            {
                Id: '4c003fb0-2a77-61ec-be54-ff11117864f4'
            }],
            TotalCount: 2
        });
    };

    var expectGetItemsServiceError = function (provider, skip, take, search) {
        var url = constructGetItemsServiceUrl(provider, skip, take, search);

        $httpBackend.expectGET(url).respond(500, errorResponse);
    };

    var expectGetUserProvidersServiceCall = function () {
        var url = serviceBaseUrl + '/GetUserProviders/';

        $httpBackend.expectGET(url).respond({
            Items: [{
                UserProviderName: 'user'
            }]
        });
    };

    /* Tests */
    it('[GeorgiMateev] / should retrieve users without filter and paging.', function () {
        var params = [null, 0, 20, null];

        expectGetItemsServiceCall.apply(this, params);

        assertItems(params);
    });

    it('[GeorgiMateev] / should retrieve users with paging.', function () {
        var params = [null, 20, 20, null];

        expectGetItemsServiceCall.apply(this, params);

        assertItems(params);
    });

    it('[GeorgiMateev] / should retrieve users with provider.', function () {
        var params = ['OpenAccessDataProvider', 0, 20, null];

        expectGetItemsServiceCall.apply(this, params);

        assertItems(params);
    });

    it('[GeorgiMateev] / should retrieve items with search filter.', function () {
        var params = [null, 0, 20, 'keyword'];

        expectGetItemsServiceCall.apply(this, params);

        assertItems(params);
    });

    it('[GeorgiMateev] / should return error.', function () {
        var params = [null, 0, 20, 'keyword'];

        expectGetItemsServiceError.apply(this, params);

        assertError(params);
    });

    it('[GeorgiMateev] / should return specific items with provider.', function () {
        var ids = ['4c003fb0-2a77-61ec-be54-ff00007864f4', '4c003fb0-2a77-61ec-be54-ff11117864f4'];
        var provider = 'OpenAccessDataProvider';

        var params = [ids, provider];

        expectGetSpecificItemsServiceCall.apply(this, params);

        assertSpecificItems(params);
    });

    it('[GeorgiMateev] / should return providers for users.', function () {
        expectGetUserProvidersServiceCall.apply(this);

        assertProviders();
    });
});