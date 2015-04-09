describe('tests for sfUsersService', function () {
    beforeEach(module('sfServices'));

    var itemType = 'Telerik.Sitefinity.Security.Model.User';
    var itemSurrogateType = 'Telerik.Sitefinity.Security.Web.Services.WcfMembershipUser';
    var sortExpression = 'UserName';

    var dataService;
    var $q;
    var $rootScope;

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

    var genericItemsServiceMock = {
        getItems: jasmine.createSpy('sfGenericItemsService.getItems')
            .andCallFake(function (options) {
                var defered = $q.defer();

                defered.resolve(dataItems);

                return defered.promise;
            })
    };

    beforeEach(module(function ($provide) {
        $provide.value('sfGenericItemsService', genericItemsServiceMock);
    }));

    beforeEach(inject(function (sfUsersService, _$q_, _$rootScope_) {
        dataService = sfUsersService;
        $q = _$q_;
        $rootScope = _$rootScope_;
    }));

    function assertItems (params) {
        var data;
        dataService.getUsers.apply(dataService, params).then(function (res) {
            data = res;
        });

        //Needed to resolve the promise.
        $rootScope.$digest();

        expect(data).toEqualData(dataItems);        
    }

    function getMockServiceGetItemsArgs () {
        var mostRecent = genericItemsServiceMock.getItems.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        return mostRecent.args;
    }

    function getFilter (search, searchField) {
        searchField = searchField || 'Title';

        if (search) {
            return "(" + searchField + ".ToUpper().Contains(\"" + search + "\".ToUpper()))";
        }

        return "";
    }

    it('[GeorgiMateev] / should call generic items service.', function () {
        var params = [true, 'OpenAccess', true, 0, 20];

        dataService.getUsers.apply(dataService, params);

        var args = getMockServiceGetItemsArgs();
        var options = args[0];

        expect(options.ignoreAdminUsers).toBe(true);

        expect(options.providerName).toBe('OpenAccess');

        expect(options.allProviders).toBe(true);

        expect(options.skip).toBe(0);

        expect(options.take).toBe(20);
    });

    it('[GeorgiMateev] / should call generic items service with the right types.', function () {
        var params = [true, null, true, 0, 20];

        dataService.getUsers.apply(dataService, params);

        var args = getMockServiceGetItemsArgs();
        var options = args[0];

        expect(options.itemType).toBe(itemType);
        expect(options.itemSurrogateType).toBe(itemSurrogateType);
    });

    it('[GeorgiMateev] / should call generic items service with the right sort expression.', function () {
        var params = [true, null, true, 0, 20];

        dataService.getUsers.apply(dataService, params);

        var args = getMockServiceGetItemsArgs();
        var options = args[0];

        expect(options.sortExpression).toBe(sortExpression);
    });

    it('[GeorgiMateev] / should call generic items service with filter.', function () {
        var params = [true, null, true, 0, 20, 'someFilter'];

        dataService.getUsers.apply(dataService, params);

        var args = getMockServiceGetItemsArgs();
        var options = args[0];

        var expectedFilter = getFilter('someFilter');
        expect(options.filter).toBe(expectedFilter);
    });

    it('[GeorgiMateev] / should return data items.', function () {
        var params = [true, null, true, 0, 20];

        assertItems(params);
    });
});