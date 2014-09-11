var sitefinity = sitefinity || {};

(function () {
    var applicationPath = '{{applicationPath}}';

    if (applicationPath.length === 0 || applicationPath.charAt(applicationPath.length - 1) !== '/')
        applicationPath = applicationPath + '/';

    var services = {
        contentItemServiceUrl: applicationPath + 'Sitefinity/Services/Content/ContentItemService.svc/',
        userServiceUrl: applicationPath + 'Sitefinity/Services/Security/Users.svc/',
        providerServiceUrl: applicationPath + 'Sitefinity/Services/DataSourceService/',
        genericDataServiceUrl: applicationPath + 'restapi/sitefinity/generic-data/',
        flatTaxonServiceUrl: applicationPath + 'Sitefinity/Services/Taxonomies/FlatTaxon.svc/', 
        taxonomyServiceUrl: applicationPath + 'Sitefinity/Services/Taxonomies/Taxonomy.svc/'
    };

    sitefinity.services = {
        getContentItemServiceUrl: function () {
            return services.contentItemServiceUrl;
        },

        getUserServiceUrl: function () {
            return services.userServiceUrl;
        },

        getProviderServiceUrl: function () {
            return services.providerServiceUrl;
        },

        getGenericDataServiceUrl: function () {
            return services.genericDataServiceUrl;
        },

        getFlatTaxonServiceUrl: function () {
            return services.flatTaxonServiceUrl;
        },

        getTaxonomyServiceUrl: function () {
            return services.taxonomyServiceUrl;
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
})();