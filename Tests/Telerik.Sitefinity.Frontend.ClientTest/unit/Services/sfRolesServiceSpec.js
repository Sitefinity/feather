describe('sfRolesService', function () {
     beforeEach(module('sfServices'));

    var appPath = 'http://mysite.com:9999/myapp';
    var serviceBaseUrl = 'http://mysite.com:9999/myapp/Sitefinity/Services/Security/Roles.svc';

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

    var $httpBackend;
    var service;
    var dataItems = {
        Items: [{
            Id: '4c003fb0-2a77-61ec-be54-ff00007864f4'
        }],
        TotalCount: 1
    };
    var serviceHelper;
    
    var errorResponse = {
        Detail: 'Error'
    };

    beforeEach(inject(function ($injector) {
        // Set up the mock http service responses
        $httpBackend = $injector.get('$httpBackend');
        service = $injector.get('sfRolesService');
        serviceHelper = $injector.get('serviceHelper');
    }));

    /* Helper methods */
    var asserItems = function (params) {
        var data;
        service.getRoles.apply(service, params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    };

    var asserSpecificItems = function (params) {
        var data;
        service.getSpecificRoles.apply(service, params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualArrayOfObjects(dataItems, 'Id');
    };

    var asserItem = function (params) {
        var data;
        service.getRole.apply(service, params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    };

    var assertError = function (params) {
        var data;
        service.getRole.apply(service, params).then(function (res) {
            data = res;
        }, function (err) {
            data = err;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data.data).toEqualData(errorResponse);
    };

    var getFilter = function (search, searchField) {
        searchField = searchField || 'Title';

        if (search) {
            return "(" + searchField + ".ToUpper().Contains(%22" + search + "%22.ToUpper()))";
        }

        return "";
    };

    var constructGetItemServiceUrl = function (itemId, provider) {
        var providerParam = "";
        if (provider) {
            providerParam = "provider=" + provider;
        }

        var servicePathPattern = '/{0}/?{1}';
        var url = serviceBaseUrl + servicePathPattern.format(itemId, providerParam);

        return url;
    };

    var expectGetItemsServiceCall = function (provider, skip, take, search, searchField) {
        var filter = getFilter(search, searchField);

        var filterParam = "";
        //if (filter) {
            filterParam = "filter=" + filter + "&";
        //}

        var providerParam = "";
        if (provider) {
            providerParam = "provider=" + provider + "&";
        }

        var servicePathPattern = '/?{0}{1}skip={2}&take={3}';
        var url = serviceBaseUrl + servicePathPattern.format(filterParam, providerParam, skip, take);

        $httpBackend.expectGET(url).respond(dataItems);
    };

    var expectGetSpecificItemsServiceCall = function (ids, provider) {
        var filter = serviceHelper.filterBuilder()
                    .specificItemsFilter(ids)
                    .getFilter();

        // Achieve similar encoding as the one in the Angular's $resource.
        var encodedFilter = encodeURIComponent(filter).replace(/%20/g, '+');

        var servicePathPattern = '/?filter={0}&provider={1}';
        var url = serviceBaseUrl + servicePathPattern.format(encodedFilter, provider);

        $httpBackend.expectGET(url).respond({
            Items: [{
                Id: '4c003fb0-2a77-61ec-be54-ff0000000000'
            },
            {
                Id: '4c003fb0-2a77-61ec-be54-ff1111111111'
            }],
            TotalCount: 2
        });
    };

    var expectGetItemServiceCall = function (itemId, provider) {
        var url = constructGetItemServiceUrl(itemId, provider);

        $httpBackend.expectGET(url).respond(dataItems);
    };

    var expectGetItemServiceError = function (itemId, provider) {
        var url = constructGetItemServiceUrl(itemId, provider);

        $httpBackend.expectGET(url).respond(500, errorResponse);
    };

    /* Tests */
    it('[GeorgiMateev] / should retrieve roles without filter and paging.', function () {
        var params = [null, 0, 20, null, null];

        expectGetItemsServiceCall.apply(this, params);

        asserItems(params);
    });

    it('[GeorgiMateev] / should retrieve roles with paging.', function () {
        var params = [null, 20, 20, null, null];

        expectGetItemsServiceCall.apply(this, params);

        asserItems(params);
    });

    it('[GeorgiMateev] / should retrieve roles with provider.', function () {
        var params = ['OpenAccessDataProvider', 0, 20, null, null];

        expectGetItemsServiceCall.apply(this, params);

        asserItems(params);
    });

    it('[GeorgiMateev] / should retrieve items with filter.', function () {
        var params = [null, 0, 20, 'keyword', 'Name'];

        expectGetItemsServiceCall.apply(this, params);

        asserItems(params);
    });

    it('[GeorgiMateev] / should return single role.', function () {
        var itemId = '4c003fb0-2a77-61ec-be54-ff00007864f4';
        var provider = 'OpenAccessDataProvider';
        var params = [itemId, provider];

        expectGetItemServiceCall.apply(this, params);

        asserItem(params);
    });

    it('[GeorgiMateev] / should return error.', function () {
        var id = '4c003fb0-2a77-61ec-be54-ff0000000000';
        var provider = 'OpenAccessDataProvider';

        var params = [id, provider];

        expectGetItemServiceError.apply(this, params);

        assertError(params);
    });

    it('[GeorgiMateev] / should return specific items with provider.', function () {
        var ids = ['4c003fb0-2a77-61ec-be54-ff0000000000', '4c003fb0-2a77-61ec-be54-ff1111111111'];
        var provider = 'OpenAccessDataProvider';

        var params = [ids, provider];

        expectGetSpecificItemsServiceCall.apply(this, params);

        asserSpecificItems(params);
    });
});