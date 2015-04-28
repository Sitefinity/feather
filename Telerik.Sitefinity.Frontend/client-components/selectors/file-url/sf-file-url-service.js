; (function () {
    angular.module('sfServices').factory('sfFileUrlService', ['$http', '$q', 'serverContext', function ($http, $q, serverContext) {
        var serviceUrl = serverContext.getRootedUrl('files-api');

        var getFiles = function (extension, path, skip, take) {
            if (path.charAt(path.length - 1) !== '/') {
                path = path + '/';
            }

            var url = serviceUrl + '?path=' + encodeURIComponent(path) + '&extension=' + extension;
            if (skip) {
                url = url + '&skip=' + skip;
            }

            if (take) {
                url = url + '&take=' + take;
            }

            var deferred = $q.defer();
            $http.get(url).
                success(function (data, status, headers, config) {
                    var rootedPath = '~/' + path;
                    var items = [];
                    for (var item in data.Items) {
                        items.push({
                            label: item.Name,
                            url: rootedPath + item.Name,
                            isFolder: item.IsFolder
                        });
                    }

                    deferred.resolve({
                        items: items,
                        totalCount: data.TotalCount
                    });
                }).
                error(function (data, status, headers, config) {
                    deferred.reject(data);
                });

            return deferred.promise;
        };

        return {
            get: getFiles
        };
    }]);
})();
