/* Tests for data-service.js */
describe('dataService', function () {
    if (!String.prototype.format) {
        String.prototype.format = function () {
            var newStr = this;

            for (var i = 0; i < arguments.length; i++) {
                var pattern = new RegExp("\\{"+ i +"\\}", "g");
                newStr = newStr.replace(pattern, arguments[i]);
            }

            return newStr;
        }
    };    

    beforeEach(module('services'));

    var appPath = 'http://mysite.com:9999/myapp';
    var dataServiceBaseUrl = 'http://mysite.com:9999/myapp/Sitefinity/Services/DynamicModules/Data.svc';
    var dummyItemType = "Telerik.Sitefinity.DynamicTypes.Model.TestModule.SomeType";

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

    beforeEach(inject(function ($injector) {
        // Set up the mock http service responses
        $httpBackend = $injector.get('$httpBackend');
        dataService = $injector.get('dataService');
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
    };

    var getFilter = function (search, searchField) {
        searchField = searchField || 'Title';

        if (search) {
            return "(" + searchField + ".ToUpper().Contains(%22"+ search +"%22.ToUpper()))";
        }

        return "";
    }

    var expectGetItemsServiceCall = function (itemType, provider, skip, take, search, searchField) {
        itemType = itemType || dummyItemType;

        var filter = getFilter(search, searchField);

        var filterParam = "";
        //if (filter) {
            filterParam = "filter=" + filter + "&";
        //}

        var providerParam = "";
        if (provider) {
            providerParam = "provider=" + provider + "&";
        }

        var servicePathPattern = '?{0}itemSurrogateType={1}&itemType={1}&{2}skip={3}&take={4}';
        var url = dataServiceBaseUrl + servicePathPattern.format(filterParam, itemType, providerParam, skip, take);

        $httpBackend.expectGET(url).respond(dataItems);
    };

    var constructGetItemServiceUrl = function (itemId, itemType, provider) {
        itemType = itemType || dummyItemType;

        var providerParam = "";
        if (provider) {
            providerParam = "&provider=" + provider;
        }

        var servicePathPattern = '/{0}?itemType={1}{2}';
        var url = dataServiceBaseUrl + servicePathPattern.format(itemId, itemType, providerParam);

        return url;
    }

    var expectGetItemServiceCall = function (itemId, itemType, provider) {
        var url = constructGetItemServiceUrl(itemId, itemType, provider);

        $httpBackend.expectGET(url).respond(dataItems);
    };

    var expectGetItemServiceError = function (itemId, itemType, provider) {
        var url = constructGetItemServiceUrl(itemId, itemType, provider);

        $httpBackend.expectGET(url).respond(500, errorResponse);
    }

    /* Tests */
    it('[GMateev] / should retrieve items without filter and paging.', function () {
        var params = [dummyItemType, null, 0, 20, null, null];

        expectGetItemsServiceCall.apply(this, params);

        asserItems(params);
    });

    it('[GMateev] / should retrieve items with paging.', function () {
        var params = [dummyItemType, null, 20, 20, null, null];

        expectGetItemsServiceCall.apply(this, params);

        asserItems(params);
    });

    it('[GMateev] / should retrieve items with provider.', function () {
        var params = [dummyItemType, 'OpenAccessDataProvider', 0, 20, null, null];

        expectGetItemsServiceCall.apply(this, params);

        asserItems(params);
    });

    it('[GMateev] / should retrieve items with filter.', function () {
        var params = [dummyItemType, null, 0, 20, 'keyword', null];

        expectGetItemsServiceCall.apply(this, params);

        asserItems(params);
    });

    it('[GMateev] / should return single item.', function () {
        var itemId = '4c003fb0-2a77-61ec-be54-ff00007864f4';
        var provider = 'OpenAccessDataProvider';
        var params = [itemId, dummyItemType, provider];

        expectGetItemServiceCall.apply(this, params);

        asserItem(params);
    });

    it('[GMateev] / should return error.', function () {
        var id = '4c003fb0-2a77-61ec-be54-ff0000000000';
        var provider = 'OpenAccessDataProvider';

        var params = [id, dummyItemType, provider];

        expectGetItemServiceError.apply(this, params);

        assertError(params);
    });   
});