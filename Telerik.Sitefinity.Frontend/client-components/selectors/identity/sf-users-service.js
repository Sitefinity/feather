angular.module('sfServices')
.factory('sfUsersService', ['serviceHelper', 'serverContext', '$http', '$q',
function (serviceHelper, serverContext, $http, $q) {
    var serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/Security/Users.svc/');
    var identifier = 'UserName';

    function getResource(itemId) {
        var url = serviceUrl;
        if (itemId && itemId !== serviceHelper.emptyGuid()) {
            url = url + itemId + '/';
        }

        return serviceHelper.getResource(url);
    }

    function getError(error, status, headers, config) {
        var result;
        if (config && config.timeout && config.timeout.$$state && config.timeout.$$state.value) {
            result = {
                statusText: config.timeout.$$state.value
            };
        }
        else {
            result = {
                error: error,
                status: status,
                headers: headers,
                config: config
            };
        }

        return result;
    }

    function getUsers(provider, skip, take, search) {
        var canceller = $q.defer();

        var cancel = function (reason) {
            canceller.resolve(reason);
        };

        var filter = serviceHelper.filterBuilder()
            .searchFilter(search, null, identifier)
            .getFilter();

        var deferred = $q.defer();

        $http({
            url: serviceUrl,
            method: "GET",
            params: {
                skip: skip,
                take: take,
                provider: provider,
                forAllProviders: provider ? false : true,
                filter: filter
            },
            timeout: canceller.promise
        })
        .success(function (result) {
            deferred.resolve(result);
        })
        .error(function (error, status, headers, config) {
            deferred.reject(getError(error, status, headers, config));
        });

        return {
            promise: deferred.promise,
            cancel: cancel
        };
    }

    function getSpecificUsers(ids, provider) {
        var canceller = $q.defer();

        var cancel = function (reason) {
            canceller.resolve(reason);
        };

        var filter = serviceHelper.filterBuilder()
            .specificItemsFilter(ids)
            .getFilter();

        var deferred = $q.defer();

        $http({
            url: serviceUrl,
            method: "GET",
            params: {
                provider: provider,
                filter: filter,
                forAllProviders: provider ? false : true
            },
            timeout: canceller.promise
        })
        .success(function (result) {
            deferred.resolve(result);
        })
        .error(function (error, status, headers, config) {
            deferred.reject(getError(error, status, headers, config));
        });

        return {
            promise: deferred.promise,
            cancel: cancel
        };
    }

    function getUserProviders() {
        var canceller = $q.defer();

        var cancel = function (reason) {
            canceller.resolve(reason);
        };

        var endpoint = 'GetUserProviders';
        var url = serviceUrl + endpoint + '/';

        var deferred = $q.defer();

        $http({
            url: url,
            method: "GET",
            timeout: canceller.promise
        })
        .success(function (result) {
            deferred.resolve(result);
        })
        .error(function (error, status, headers, config) {
            deferred.reject(getError(error, status, headers, config));
        });

        return {
            promise: deferred.promise,
            cancel: cancel
        };
    }

    return {
        getUsers: getUsers,
        getSpecificUsers: getSpecificUsers,
        getUserProviders: getUserProviders
    };
}]);