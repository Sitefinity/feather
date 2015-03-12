angular.module('sfServices')
    .factory('sfRolesService', ['serviceHelper', 'serverContext', '$http', '$q',
    function (serviceHelper, serverContext, $http, $q) {
        var serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/Security/Roles.svc/');
        var identifierField = 'Name';

        function getResource (itemId) {
            var url = serviceUrl;
            if (itemId && itemId !== serviceHelper.emptyGuid()) {
                url = url + itemId + '/';
            }

            return serviceHelper.getResource(url);
        }

        function getRoles(provider, skip, take, search, rolesToHide) {
            var canceller = $q.defer();

            var cancel = function (reason) {
                canceller.resolve(reason);
            };

            var filter = serviceHelper.filterBuilder()
                    .searchFilter(search, null, identifierField)
                    .and()
                    .differFilter(rolesToHide, identifierField)
                    .getFilter();

            var deferred = $q.defer();

            $http({
                url: serviceUrl,
                method: "GET",
                params: {
                    provider: provider,
                    skip: skip,
                    take: take,
                    filter: filter
                },
                timeout: canceller.promise
            })
            .success(function (result) {
                deferred.resolve(result);
            })
            .error(function (error, status, headers, config) {
                deferred.reject({
                    statusText: config.timeout.$$state.value
                });
            });

            return {
                promise: deferred.promise,
                cancel: cancel
            };
        }

        function getRole (id, provider) {
            return getResource(id).get({
                provider: provider
            })
            .$promise;
        }

        function getSpecificRoles(ids, provider) {
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
                    filter: filter
                },
                timeout: canceller.promise
            })
            .success(function (result) {
                deferred.resolve(result);
            })
            .error(function (error, status, headers, config) {
                deferred.reject({
                    statusText: config.timeout.$$state.value
                });
            });

            return {
                promise: deferred.promise,
                cancel: cancel
            };
        }

        function getRoleProviders (commaSeperatedAbilities) {
            var endpoint = 'GetRoleProviders';
            var url = serviceUrl + endpoint + '/';

            return serviceHelper.getResource(url).get({
                abilities: commaSeperatedAbilities,
                addAppRoles: true
            })
            .$promise;
        }

        return {
            getRoles: getRoles,
            getRole: getRole,
            getSpecificRoles: getSpecificRoles,
            getRoleProviders: getRoleProviders
        };
}]);