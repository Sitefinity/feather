var sitefinity = sitefinity || {};

(function () {
    var applicationPath = '/';

    if (applicationPath.length === 0 || applicationPath.charAt(applicationPath.length - 1) !== '/')
        applicationPath = applicationPath + '/';

    var services = {
        contentItemServiceUrl: applicationPath + 'Sitefinity/Services/Content/ContentItemService.svc/',
        userServiceUrl: applicationPath + 'Sitefinity/Services/Security/Users.svc/'
    };

    sitefinity.services = {
        getContentItemServiceUrl: function () {
            return services.contentItemServiceUrl;
        },

        getUserServiceUrl: function () {
            return services.userServiceUrl;
        }
    };

    sitefinity.getEmbeddedResourceUrl = function (assemblyName, resourcePath) {
        return applicationPath + 'Frontend-Assembly/' + encodeURIComponent(assemblyName) + '/' + resourcePath;
    };

    sitefinity.getRootedUrl = function (path) {
        if (path.length > 0 && path.charAt(0) === '/') {
            path = path.substring(1, path.length);
        }

        return applicationPath + path;
    };

    sitefinity.appendPackageParameter = function (url) {
        return url;
    };
})();