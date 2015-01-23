/* Tests for sf-search-service.js */
describe('sfLanguageService', function () {
    var $httpBackend;
    var dataService;
    var dataItems = {
        "BackendCultures": [{
            "Culture": "en",
            "DisplayName": "English",
            "FieldSuffix": "",
            "IsDefault": true,
            "Key": "english-en",
            "ShortName": "en",
            "SitesNames": [],
            "SitesUsingCultureAsDefault": [],
            "UICulture": "en"
        }],
        "Cultures": [{
            "Culture": "en",
            "DisplayName": "English",
            "FieldSuffix": "",
            "IsDefault": true,
            "Key": "english-en",
            "ShortName": "en",
            "SitesNames": [],
            "SitesUsingCultureAsDefault": [],
            "UICulture": "en"
        }, {
            "Culture": "en-US",
            "DisplayName": "English (United States)",
            "FieldSuffix": "",
            "IsDefault": false,
            "Key": "english (united states)-en-us",
            "ShortName": "en-US",
            "SitesNames": ["\/"],
            "SitesUsingCultureAsDefault": ["\/"],
            "UICulture": "en-US"
        }, {
            "Culture": "tr-TR",
            "DisplayName": "Turkish (Turkey)",
            "FieldSuffix": "",
            "IsDefault": false,
            "Key": "turkish (turkey)-tr-tr",
            "ShortName": "tr-TR",
            "SitesNames": ["\/"],
            "SitesUsingCultureAsDefault": [],
            "UICulture": "tr-TR"
        }, {
            "Culture": "ar-MA",
            "DisplayName": "Arabic (Morocco)",
            "FieldSuffix": "",
            "IsDefault": false,
            "Key": "arabic (morocco)-ar-ma",
            "ShortName": "ar-MA",
            "SitesNames": ["\/"],
            "SitesUsingCultureAsDefault": [],
            "UICulture": "ar-MA"
        }, {
            "Culture": "sr-Cyrl-BA",
            "DisplayName": "Serbian (Cyrillic, Bosnia and Herzegovina)",
            "FieldSuffix": "",
            "IsDefault": false,
            "Key": "serbian (cyrillic, bosnia and herzegovina)-sr-cyrl-ba",
            "ShortName": "sr-Cyrl-BA",
            "SitesNames": ["\/"],
            "SitesUsingCultureAsDefault": [],
            "UICulture": "sr-Cyrl-BA"
        }],
        "DefaultLocalizationStrategy": "SubFolderUrlLocalizationStrategy",
        "DefaultStrategySettings": [],
        "MonolingualCulture": null,
        "SubdomainStrategySettings": [{
            "DisplayName": "English",
            "IsDefault": false,
            "Key": "english-en",
            "Setting": null
        }, {
            "DisplayName": "English (United States)",
            "IsDefault": false,
            "Key": "english (united states)-en-us",
            "Setting": null
        }, {
            "DisplayName": "Turkish (Turkey)",
            "IsDefault": false,
            "Key": "turkish (turkey)-tr-tr",
            "Setting": null
        }, {
            "DisplayName": "Arabic (Morocco)",
            "IsDefault": false,
            "Key": "arabic (morocco)-ar-ma",
            "Setting": null
        }, {
            "DisplayName": "Serbian (Cyrillic, Bosnia and Herzegovina)",
            "IsDefault": false,
            "Key": "serbian (cyrillic, bosnia and herzegovina)-sr-cyrl-ba",
            "Setting": null
        }]
    };

    var errorResponse = {
        Detail: 'Error'
    };

    var appPath = 'http://mysite.com:9999/myapp';

    beforeEach(module('sfServices'));

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

    beforeEach(inject(function ($injector) {
        // Set up the mock http service responses
        $httpBackend = $injector.get('$httpBackend');
        dataService = $injector.get('sfLanguageService');
    }));

    /* Helper methods */
    var assertItems = function (params) {
        var data;
        dataService.getLocalizationSettings.apply(dataService, params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    };

    /* Tests */
    it('[EGaneva] / should retrieve items.', function () {
        $httpBackend.expectGET('http://mysite.com:9999/myapp/Sitefinity/Services/Configuration/ConfigSectionItems.svc/localization/?includeSitesNames=true')
        .respond(dataItems);

        assertItems([]);
    });
});