(function () {
    angular.module('sfServices').factory('sfFileUrlService', ['$http', '$q', 'serverContext', function ($http, $q, serverContext) {
        var serviceUrl = serverContext.getRootedUrl('RestApi/files-api');

        var getExtension = function (str) {
            var dotIdx = str.lastIndexOf('.');
            if (dotIdx === -1)
                return null;

            return str.substr(dotIdx + 1);
        };

        var getFiles = function (extension, path, skip, take) {
            path = path || '';

            var url = serviceUrl + '?extension=' + extension;

            if (path) {
                url = url + '&path=' + encodeURIComponent(path);
            }

            if (skip) {
                url = url + '&skip=' + skip;
            }

            if (take) {
                url = url + '&take=' + take;
            }

            if (path.length > 0 && path.charAt(path.length - 1) !== '/') {
                path = path + '/';
            }

            var deferred = $q.defer();
            $http.get(url).
                success(function (data, status, headers, config) {
                    var rootedPath = '~/' + path;
                    var items = [];
                    if (data.Items && data.Items.length > 0) {
                        for (var i = 0; i < data.Items.length; i++) {
                            var item = data.Items[i];
                            var label = item.Name;

                            if (label.indexOf('\\') === 0)
                                label = label.substring(1);

                            items.push({
                                label: label,
                                path: path,
                                url: rootedPath + label,
                                isFolder: item.IsFolder,
                                hasChildren: item.HasChildren,
                                extension: getExtension(item.Name)
                            });
                        }
                    }

                    deferred.resolve(items);
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
