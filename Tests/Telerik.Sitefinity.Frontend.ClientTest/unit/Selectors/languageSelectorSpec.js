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
            "SitesNames": ["site1", "site2"],
            "SitesUsingCultureAsDefault": ["site1"],
            "UICulture": "en"
        }, {
            "Culture": "de",
            "DisplayName": "German",
            "FieldSuffix": "",
            "IsDefault": false,
            "Key": "german-de",
            "ShortName": "de",
            "SitesNames": ["site2", "site1"],
            "SitesUsingCultureAsDefault": ["site2"],
            "UICulture": "en-US"
        }, {
            "Culture": "tr-TR",
            "DisplayName": "Turkish (Turkey)",
            "FieldSuffix": "",
            "IsDefault": false,
            "Key": "turkish (turkey)-tr-tr",
            "ShortName": "tr-TR",
            "SitesNames": ["site2"],
            "SitesUsingCultureAsDefault": [],
            "UICulture": "tr-TR"
        }]
    };

    var fakeSites = [{
        "CultureDisplayNames": null,
        "Id": "344b7567-6965-43c9-88b6-028bbe6f4c9d",
        "IsAllowedConfigureModules": false,
        "IsAllowedCreateEdit": false,
        "IsAllowedSetPermissions": false,
        "IsAllowedStartStop": false,
        "IsDefault": true,
        "IsDeleteable": false,
        "IsOffline": false,
        "Name": "site1",
        "SiteMapRootNodeId": "f669d9a7-009d-4d83-ddaa-000000000001",
        "SiteUrl": null,
        "UIStatus": "Online"
    }, {
        "CultureDisplayNames": null,
        "Id": "11376ecf-94fd-622b-8416-ff0000d46a77",
        "IsAllowedConfigureModules": false,
        "IsAllowedCreateEdit": false,
        "IsAllowedSetPermissions": false,
        "IsAllowedStartStop": false,
        "IsDefault": false,
        "IsDeleteable": false,
        "IsOffline": false,
        "Name": "site2",
        "SiteMapRootNodeId": "f669d9a7-009d-4d83-ddaa-000000000002",
        "SiteUrl": null,
        "UIStatus": "Online"
    }];

    var serviceResult;
    var $q;
    var provide;

    //Load the module under test.
    beforeEach(module('sfSelectors'));

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));

    afterEach(function () {
        //Tear down.
        var leftOver = $('.testDiv');
        leftOver.empty();
        leftOver.remove();
    });

    describe('multilingual', function () {
        var languageService = {
            getLocalizationSettings: jasmine.createSpy('languageService.getLocalizationSettings').andCallFake(function () {
                if ($q) {
                    serviceResult = $q.defer();
                }

                serviceResult.resolve(customLangItems);

                return serviceResult.promise;
            })
        };

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

        describe('multisite mode enabled', function () {
            beforeEach(inject(function (serverContext) {
                serverContext.isMultisiteEnabled = function () {
                    return true;
                };
            }));

            describe('on page from the default site with default culture', function () {
                beforeEach(inject(function (serverContext) {
                    serverContext.getCurrentFrontendRootNodeId = function () {
                        return "f669d9a7-009d-4d83-ddaa-000000000001";
                    };

                    serverContext.getFrontendLanguages = function () {
                        return ['en', 'de'];
                    };

                    serverContext.getUICulture = function () {
                        return 'en';
                    };
                }));

                beforeEach(function () {
                    scope.sfSite = fakeSites[0];
                });

                it('[EGaneva] / should init default language selector values: site1 and en culture.', function () {
                    var template = '<sf-language-selector sf-site="sfSite" sf-culture="sfCulture"></sf-language-selector>';

                    commonMethods.compileDirective(template, scope);

                    expect(scope.sfCulture).toBeDefined();
                    expect(scope.sfCulture).toEqual(customLangItems.Cultures[0]);
                    expect(scope.sfSite).toEqual(fakeSites[0]);

                    var childScope = scope.$$childHead;

                    expect(childScope.sfCultures).toBeDefined();
                    expect(childScope.sfCultures.length).toEqual(2);
                    expect(childScope.sfCultures[0].Culture).toEqual('en');
                    expect(childScope.sfCultures[1].Culture).toEqual('de');
                });

                it('[NPetrova] / when site is changed to site2 the sfCulture must be en.', function () {
                    var template = '<sf-language-selector sf-site="sfSite" sf-culture="sfCulture"></sf-language-selector>';

                    commonMethods.compileDirective(template, scope);

                    var childScope = scope.$$childHead;

                    expect(childScope.sfSite).toEqual(fakeSites[0]);
                    expect(childScope.sfCulture.Culture).toEqual('en');

                    // change sfSite to site2
                    scope.sfSite = fakeSites[1];

                    commonMethods.compileDirective(template, scope);

                    expect(childScope.sfSite).toEqual(fakeSites[1]);
                    expect(childScope.sfCulture.Culture).toEqual('en');
                });

                it('[NPetrova] / when sfCulture is changed to de and sfSite is changed to site 2 the sfCulture must be en (the current UI culture).', function () {
                    var template = '<sf-language-selector sf-site="sfSite" sf-culture="sfCulture"></sf-language-selector>';

                    commonMethods.compileDirective(template, scope);

                    var childScope = scope.$$childHead;

                    expect(childScope.sfSite).toEqual(fakeSites[0]);
                    expect(childScope.sfCulture.Culture).toEqual('en');

                    // change sfCultur to de
                    scope.sfCulture = customLangItems[1];
                    // change sfSite to site 2
                    scope.sfSite = fakeSites[1];

                    commonMethods.compileDirective(template, scope);

                    expect(childScope.sfSite).toEqual(fakeSites[1]);
                    expect(childScope.sfCulture.Culture).toEqual('en');
                });
            });

            describe('on page from the not default site with culture that is not default and does not exist on the default site', function () {
                beforeEach(inject(function (serverContext) {
                    serverContext.getCurrentFrontendRootNodeId = function () {
                        return "f669d9a7-009d-4d83-ddaa-000000000002";
                    };

                    serverContext.getFrontendLanguages = function () {
                        return ['en', 'de', 'tr-TR'];
                    };

                    serverContext.getUICulture = function () {
                        return 'tr-TR';
                    };
                }));

                beforeEach(function () {
                    scope.sfSite = fakeSites[1];
                });

                it('[NPetrova] / should init default language selector values: site2 and tr-TR culture.', function () {
                    var template = '<sf-language-selector sf-site="sfSite" sf-culture="sfCulture"></sf-language-selector>';

                    commonMethods.compileDirective(template, scope);

                    expect(scope.sfCulture).toBeDefined();
                    expect(scope.sfCulture).toEqual(customLangItems.Cultures[2]);
                    expect(scope.sfSite).toEqual(fakeSites[1]);

                    var childScope = scope.$$childHead;

                    expect(childScope.sfCultures).toBeDefined();
                    expect(childScope.sfCultures.length).toEqual(3);
                    expect(childScope.sfCultures[0].Culture).toEqual('en');
                    expect(childScope.sfCultures[1].Culture).toEqual('de');
                    expect(childScope.sfCultures[2].Culture).toEqual('tr-TR');
                });

                it('[NPetrova] / when site is changed to site1 the sfCulture must be the default culture for site1 (en).', function () {
                    var template = '<sf-language-selector sf-site="sfSite" sf-culture="sfCulture"></sf-language-selector>';

                    commonMethods.compileDirective(template, scope);

                    var childScope = scope.$$childHead;

                    expect(childScope.sfSite).toEqual(fakeSites[1]);
                    expect(childScope.sfCulture.Culture).toEqual('tr-TR');

                    // change sfSite to site1
                    scope.sfSite = fakeSites[0];

                    commonMethods.compileDirective(template, scope);

                    expect(childScope.sfSite).toEqual(fakeSites[0]);
                    // sfCulture must be the default culture for site1 because site1 does not have tr-TR culture (the current UI culture)
                    expect(childScope.sfCulture.Culture).toEqual('en');
                });
            });
        });

        describe('multisite mode is disabled', function () {
            beforeEach(inject(function (serverContext) {
                serverContext.isMultisiteEnabled = function () {
                    return false;
                };

                serverContext.getCurrentFrontendRootNodeId = function () {
                    return "f669d9a7-009d-4d83-ddaa-000000000001";
                };

                serverContext.getFrontendLanguages = function () {
                    return ['en', 'de', 'tr-TR'];
                };

                serverContext.getUICulture = function () {
                    return 'en';
                };
            }));

            it('[NPetrova] / should init default language selector values: undefined sfSite and en culture.', function () {
                var template = '<sf-language-selector sf-site="sfSite" sf-culture="sfCulture"></sf-language-selector>';

                commonMethods.compileDirective(template, scope);

                expect(scope.sfCulture).toBeDefined();
                expect(scope.sfCulture).toEqual(customLangItems.Cultures[0]);
                expect(scope.sfSite).toBeUndefined();

                var childScope = scope.$$childHead;

                expect(childScope.sfCultures).toBeDefined();
                expect(childScope.sfCultures.length).toEqual(3);
                expect(childScope.sfCultures[0].Culture).toEqual('en');
                expect(childScope.sfCultures[1].Culture).toEqual('de');
                expect(childScope.sfCultures[2].Culture).toEqual('tr-TR');
            });
        });
    });

    describe('monolingual', function () {
        var customLangItem = {
            Cultures: [{
                "Culture": "en",
                "DisplayName": "English",
                "FieldSuffix": "",
                "IsDefault": true,
                "Key": "english-en",
                "ShortName": "en",
                "SitesNames": ["site1", "site2"],
                "SitesUsingCultureAsDefault": ["site1"],
                "UICulture": "en"
            }]
        };

        var languageServiceMonolingual = {
            getLocalizationSettings: jasmine.createSpy('languageService.getLocalizationSettings').andCallFake(function () {
                if ($q) {
                    serviceResult = $q.defer();
                }

                serviceResult.resolve(customLangItem);

                return serviceResult.promise;
            })
        };

        beforeEach(module(function ($provide) {
            //Force angular to use the mock.
            $provide.value('sfLanguageService', languageServiceMonolingual);

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
            serverContext.isMultisiteEnabled = function () {
                return false;
            };

            serverContext.getCurrentFrontendRootNodeId = function () {
                return "f669d9a7-009d-4d83-ddaa-000000000001";
            };

            serverContext.getFrontendLanguages = function () {
                return ['en'];
            };

            serverContext.getUICulture = function () {
                return null;
            };
        }));

        it('[NPetrova] / should init default language selector values: undefined sfSite and en csfCulture.', function () {
            var template = '<sf-language-selector sf-site="sfSite" sf-culture="sfCulture"></sf-language-selector>';

            commonMethods.compileDirective(template, scope);

            expect(scope.sfCulture).toBeDefined();
            expect(scope.sfCulture).toEqual(customLangItems.Cultures[0]);
            expect(scope.sfSite).toBeUndefined();

            var childScope = scope.$$childHead;

            expect(childScope.sfCultures).toBeDefined();
            expect(childScope.sfCultures.length).toEqual(1);
            expect(childScope.sfCultures[0].Culture).toEqual('en');
        });
    });
});
