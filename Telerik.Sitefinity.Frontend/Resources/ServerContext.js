﻿var sitefinity = sitefinity || {};

(function () {
    var applicationPath = document.getElementById('sf-application-path').value;
    var currentPackage = document.getElementById('sf-current-package').value;
    var frontendLanguages = JSON.parse(document.getElementById('sf-frontend-languages').value);
    var currentFrontendRootNodeId = document.getElementById('sf-currentfrontend-root-node').value;
    var currentUserId = document.getElementById('sf-current-user-id').value;
    var categoriesTaxonomyId = "e5cd6d69-1543-427b-ad62-688a99f5e7d4";
    var isMultisiteMode = document.getElementById('sf-is-multisite-mode').value;
    var siteId = document.getElementById('sf-site-id').value;
    var damSupportedMediaTypesInputs = document.getElementsByName('sf-dam-supported-media-types') || [];

    if (applicationPath.length === 0 || applicationPath.charAt(applicationPath.length - 1) !== '/')
        applicationPath = applicationPath + '/';

    var services = {
        contentItemServiceUrl: applicationPath + 'Sitefinity/Services/Content/ContentItemService.svc/',
        userServiceUrl: applicationPath + 'Sitefinity/Services/Security/Users.svc/',
        providerServiceUrl: applicationPath + 'Sitefinity/Services/DataSourceService/',
        flatTaxonServiceUrl: applicationPath + 'Sitefinity/Services/Taxonomies/FlatTaxon.svc/',
        taxonomyServiceUrl: applicationPath + 'Sitefinity/Services/Taxonomies/Taxonomy.svc/',
        newsItemServiceUrl: applicationPath + 'Sitefinity/Services/Content/NewsItemService.svc/',
        dataServiceUrl: applicationPath + 'Sitefinity/Services/DynamicModules/Data.svc/',
        personalizationServiceUrl: applicationPath + 'RestApi/Sitefinity/personalizations/controls/'
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

        getFlatTaxonServiceUrl: function () {
            return services.flatTaxonServiceUrl;
        },

        getTaxonomyServiceUrl: function () {
            return services.taxonomyServiceUrl;
        },

        getNewsItemServiceUrl: function () {
            return services.newsItemServiceUrl;
        },

        getDataServiceUrl: function () {
            return services.dataServiceUrl;
        },

        getPersonalizationServiceUrl: function () {
            return services.personalizationServiceUrl;
        }
    };

    sitefinity.getEmbeddedResourceUrl = function (assemblyName, resourcePath) {
        var url = sitefinity.appendPackageParameter(applicationPath + 'Frontend-Assembly/' + encodeURIComponent(assemblyName) + '/' + resourcePath);
        return url;
    };

    sitefinity.getRootedUrl = function (path) {
        if (path.length > 0 && path.charAt(0) === '/') {
            path = path.substring(1, path.length);
        }

        return applicationPath + path;
    };

    sitefinity.appendPackageParameter = function (url) {
        if (!url || !currentPackage)
            return url;

        var parameter = 'package=' + currentPackage;
        if (url.indexOf('?' + parameter) == -1 && url.indexOf('&' + parameter) == -1) {
            var separator = url.indexOf('?') == -1 ? '?' : '&';
            return url + separator + parameter;
        }
        else {
            return url;
        }
    };

    sitefinity.getFrontendLanguages = function () {
        return frontendLanguages;
    };

    sitefinity.getCurrentFrontendRootNodeId = function () {
        return currentFrontendRootNodeId;
    };

    sitefinity.setCurrentFrontendRootNodeId = function (value) {
        currentFrontendRootNodeId = value;
    };

    sitefinity.getCategoriesTaxonomyId = function () {
        return categoriesTaxonomyId;
    };

    sitefinity.getCurrentUserId = function () {
        return currentUserId;
    };

    sitefinity.isMultisiteEnabled = function () {
        if (isMultisiteMode.toLowerCase() == "true")
            return true;
        else if (isMultisiteMode.toLowerCase() == "false")
            return false;
        else {
            return Boolean(isMultisiteMode);
        }
    };

    sitefinity.getSiteId = function () {
        return siteId;
    };

    sitefinity.damSupportedMediaTypes = function () {
        var damSupportedMediaTypes = [];
        if (damSupportedMediaTypesInputs) {
            damSupportedMediaTypesInputs.forEach(x => {
                if (x && x.value) {
                    damSupportedMediaTypes.push(x.value);
                }
            });
        }

        return damSupportedMediaTypes;
    }
})();