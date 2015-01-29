/* Tests for language selector */
describe("language selector", function () {
    var scope,
        ITEMS_COUNT = 3;

    var customLangItems = {
        Cultures: [{
            "Culture": "en",
            "DisplayName": "English",
            "FieldSuffix": "",
            "IsDefault": true,
            "Key": "english-en",
            "ShortName": "en",
            "SitesNames": ["\/site2"],
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
        }]
    };

    var fakeSite = {
        "CultureDisplayNames": null,
        "Id": "344b7567-6965-43c9-88b6-028bbe6f4c9d",
        "IsAllowedConfigureModules": false,
        "IsAllowedCreateEdit": false,
        "IsAllowedSetPermissions": false,
        "IsAllowedStartStop": false,
        "IsDefault": true,
        "IsDeleteable": false,
        "IsOffline": false,
        "Name": "\/",
        "SiteMapRootNodeId": "f669d9a7-009d-4d83-ddaa-000000000002",
        "SiteUrl": null,
        "UIStatus": "Online"
    };

    var serviceResult;
    var $q;
    var provide;

    var languageService = {
        getLocalizationSettings: jasmine.createSpy('languageService.getLocalizationSettings').andCallFake(function () {
            if ($q) {
                serviceResult = $q.defer();
            }

            serviceResult.resolve(customLangItems);

            return serviceResult.promise;
        })
    };

    //Load the module under test.
    beforeEach(module('sfSelectors'));

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));

    beforeEach(module(function ($provide) {
        //Force angular to use the mock.
        $provide.value('sfLanguageService', languageService);

        provide = $provide;
    }));

    beforeEach(inject(function ($rootScope, $httpBackend, _$q_, $templateCache, _$timeout_) {
        //Build the scope with whom the directive will be compiled.
        scope = $rootScope.$new();

        $q = _$q_;
        $timeout = _$timeout_;

        serviceResult = _$q_.defer();
    }));

    beforeEach(function () {
        commonMethods.mockServerContextToEnableTemplateCache();
    });

    beforeEach(inject(function (serverContext) {
        serverContext.getCurrentFrontendRootNodeId = function () {
            return "850B39AF-4190-412E-9A81-C72B04A34C0F";
        };

        serverContext.getFrontendLanguages = function () {
           return ['en', 'de'];
        };

        serverContext.isMultisiteEnabled = function () {
            return true;
        };
    }));

    afterEach(function () {
        //Tear down.
        var leftOver = $('.testDiv');
        leftOver.empty();
        leftOver.remove();
    });

    describe('check default properties initialization of language selector', function () {
        it('[EGaneva] / should init default language selector values.', function () {

            var template = '<sf-language-selector sf-site="sfSite" sf-culture="sfCulture"></sf-language-selector>';

            scope.sfSite = fakeSite;

            commonMethods.compileDirective(template, scope);

            expect(scope.sfCulture).toBeDefined();
            expect(scope.sfCulture).toEqual(customLangItems.Cultures[1]);
            expect(scope.sfSite).toEqual(fakeSite);

            var childScope = scope.$$childHead;

            expect(childScope.sfCultures).toBeDefined();
            expect(childScope.sfCultures.length).toEqual(2);
            expect(childScope.sfCultures[0].Culture).toEqual('en-US');
            expect(childScope.sfCultures[1].Culture).toEqual('tr-TR');
        });

        it('[EGaneva] / cultures should be empty if no site provided.', function () {

            var template = '<sf-language-selector sf-site="sfSite" sf-culture="sfCulture"></sf-language-selector>';

            commonMethods.compileDirective(template, scope);

            expect(scope.sfCulture).toBeUndefined();
            expect(scope.sfSite).toBeUndefined();

            var childScope = scope.$$childHead;
            expect(childScope.sfCultures).toBeUndefined();
        });
    });
});