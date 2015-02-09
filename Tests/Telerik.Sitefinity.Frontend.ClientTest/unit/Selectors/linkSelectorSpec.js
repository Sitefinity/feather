/* Tests for link selector */
describe("link selector", function () {
    var scope;
    var serviceResult;
    var $q;
    var provide;
    var ITEMS_COUNT = 4;
    var appPath = 'http://mysite.com:9999/myapp';
    var sfLinkMode;

    var customDataItems = {
        Items: [],
        TotalCount: ITEMS_COUNT
    };

    customDataItems.Items.push({
        Id: '28d7e74c-c789-61c4-9817-ff000095605c',
        Title: { Value: 'First Page' },
        HasChildren: true
    });

    for (var i = 1; i < ITEMS_COUNT + 1; i++) {
        customDataItems.Items[i] = {
            Id: '4c003fb0-2a77-61ec-be54-ff00007864f' + i,
            Title: { Value: 'Dummy' + i },
            HasChildren: true
        };
    }

    var cultures = [{
        "Culture": "en",
        "DisplayName": "English",
        "FieldSuffix": "",
        "IsDefault": true,
        "Key": "english-en",
        "ShortName": "en",
        "SitesNames": ["Site1", "Site2"],
        "SitesUsingCultureAsDefault": ["Site2"],
        "UICulture": "en"
    },{
        "Culture": "de",
        "DisplayName": "German",
        "FieldSuffix": "",
        "IsDefault": false,
        "Key": "german-de",
        "ShortName": "de",
        "SitesNames": ["Site1", "Site2"],
        "SitesUsingCultureAsDefault": ["Site1"],
        "UICulture": "de"
    }];

    var sfSites =
        [{
            "CultureDisplayNames": null,
            "Id": "344b7567-6965-43c9-88b6-028bbe6f4c9d",
            "IsAllowedConfigureModules": false,
            "IsAllowedCreateEdit": false,
            "IsAllowedSetPermissions": false,
            "IsAllowedStartStop": false,
            "IsDefault": true,
            "IsDeleteable": false,
            "IsOffline": false,
            "Name": "Site1",
            "SiteMapRootNodeId": "f669d9a7-009d-4d83-ddaa-000000000002",
            "SiteUrl": null,
            "UIStatus": "Online"
        },
        {
            "CultureDisplayNames": null,
            "Id": "344b7567-6965-43c9-88b6-028bbe6f4c9b",
            "IsAllowedConfigureModules": false,
            "IsAllowedCreateEdit": false,
            "IsAllowedSetPermissions": false,
            "IsAllowedStartStop": false,
            "IsDefault": false,
            "IsDeleteable": false,
            "IsOffline": false,
            "Name": "Site2",
            "SiteMapRootNodeId": "f669d9a7-009d-4d83-ddaa-000000000003",
            "SiteUrl": null,
            "UIStatus": "Online"
        }];


    //Mock site service.
    var siteService = {
        getSites: jasmine.createSpy('siteService.getSites').andCallFake(function (editorContent) {
            return sfSites;
        }),
        getSiteByRootNoteId: jasmine.createSpy('siteService.getSiteByRootNoteId').andCallFake(function (rootNodeId) {
            return sfSites[1];
        }),
        getSitesForUserPromise: jasmine.createSpy('siteService.getSitesForUserPromise').andCallFake(function (editorContent) {
            if ($q) {
                serviceResult = $q.defer();
            }

            serviceResult.resolve({ Items: sfSites });

            return serviceResult.promise;
        })
    };

    var languageService = {
        getLocalizationSettings: jasmine.createSpy('languageService.getLocalizationSettings').andCallFake(function (editorContent) {
            if ($q) {
                serviceResult = $q.defer();
            }

            serviceResult.resolve({ Cultures: cultures });

            return serviceResult.promise;
        }),
    };

    //Mock page service. It returns promises.
    var pagesService = {
        getItems: jasmine.createSpy('sfPageService.getItems').andCallFake(function (parentId, siteId, provider, search) {
            if ($q) {
                serviceResult = $q.defer();
            }
            serviceResult.resolve(customDataItems);

            return serviceResult.promise;
        }),
        getSpecificItems: jasmine.createSpy('sfPageService.getSpecificItems').andCallFake(function (ids, provider) {
            if ($q) {
                serviceResult = $q.defer();
            }
            serviceResult.resolve({ Items: [customDataItems.Items[0]] });

            return serviceResult.promise;
        }),
        getPredecessors: jasmine.createSpy('sfPageService.getPredecessors').andCallFake(function (itemId, provider) {
            if ($q) {
                serviceResult = $q.defer();
            }
            serviceResult.resolve({ Items: [customDataItems.Items[0]] });

            return serviceResult.promise;
        })
    };

    //Load the module responsible for the modal dialog
    beforeEach(module('modalDialog'));

    //Load the module under test.
    beforeEach(module('sfSelectors'));

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));

    beforeEach(module(function ($provide) {
        //Force angular to use the mock.
        //$provide.value('sfLinkService', linkService);
        $provide.value('sfMultiSiteService', siteService);
        $provide.value('sfLanguageService', languageService);
        $provide.value('sfPageService', pagesService);
        provide = $provide;
    }));

    beforeEach(module(function ($provide) {
        var serverContext = {
            getRootedUrl: function (path) {
                return appPath + '/' + path;
            },
            getUICulture: function () {
                return "de";
            },
            getCurrentUserId: function () {
                return '36e9e47f-0d78-6425-ae98-ff0000fc9faf';
            },
            getCurrentFrontendRootNodeId: function () {
                return '36e9e47f-0d78-6425-ae98-ff0000fc9faf';
            },
            isMultisiteEnabled: function () {
                return true;
            }
        };
        $provide.value('serverContext', serverContext);
    }));

    beforeEach(inject(function ($rootScope, $httpBackend, _$q_, $templateCache, _$timeout_, $injector) {
        //Build the scope with whom the directive will be compiled.
        scope = $rootScope.$new();
        scope.provider = 'OpenAccessDataProvider';
        $q = _$q_;
        $timeout = _$timeout_;
        serviceResult = _$q_.defer();
        sfLinkMode = $injector.get('sfLinkMode');
    }));

    beforeEach(function () {
        commonMethods.mockServerContextToEnableTemplateCache();
    });

    beforeEach(inject(function (serverContext) {
        serverContext.getFrontendLanguages = function () {
            return ['en', 'de'];
        };
    }));

    afterEach(function () {
        //Tear down.
        var leftOver = $('.testDiv, .modal, .modal-backdrop');
        leftOver.empty();
        leftOver.remove();
    });

    describe('Test link selector init logic', function () {
        it('[Manev] / Test default property initialization.', function () {

            scope.sfLinkHtml = "";
            scope.selectedItem = "";
            scope.sfEditorContent = "";

            var template = "<sf-link-selector sf-link-html='sfLinkHtml' sf-selected-item='selectedItem' sf-editor-content='sfEditorContent'></sf-link-selector>";

            commonMethods.compileDirective(template, scope);

            var directiveScope = scope.$$childHead;

            expect(directiveScope.sfSelectedItem.mode).toBe(1);
            expect(directiveScope.sfSite).toBe(sfSites[1]);

            var isWebAddressRadioChecked = $('#webAddressRadio').prop('checked');

            expect(isWebAddressRadioChecked).toBe(true);
        });

        it('[Manev] / Test inital set of sfLinkMode.', function () {

            scope.sfLinkHtml = "";
            scope.selectedItem = "";
            scope.sfEditorContent = "";

            var template = "<sf-link-selector sf-link-html='sfLinkHtml' sf-selected-item='selectedItem' sf-editor-content='sfEditorContent'></sf-link-selector>";

            commonMethods.compileDirective(template, scope);

            var directiveScope = scope.$$childHead;

            expect(directiveScope.sfLinkMode).toBe(sfLinkMode);
        });

        it('[Manev] / Test sfLinkHtml provided as a string.', function () {

            var text = "HERE IS THE LINK TO THE FREEDOM.";

            scope.sfLinkHtml = text;
            scope.selectedItem = "";
            scope.sfEditorContent = "";

            var template = "<sf-link-selector sf-link-html='sfLinkHtml' sf-selected-item='selectedItem' sf-editor-content='sfEditorContent'></sf-link-selector>";

            commonMethods.compileDirective(template, scope);

            var directiveScope = scope.$$childHead;

            expect(directiveScope.sfSelectedItem.mode).toBe(1);
            expect(directiveScope.sfSelectedItem.displayText).toBe(text);

            var renderedText = $('#textToDisplay1').val();

            expect(renderedText).toBe(text);
        });

        it('[Manev] / Test sfLinkHtml provided as a jQuery wrapper with valid anchor element with ref and sfref and no lng param.', function () {

            scope.sfLinkHtml = $("<a href='CodeBase/widgettests' sfref='[f669d9a7-009d-4d83-ddaa-000000000002]28d7e74c-c789-61c4-9817-ff000095605c'>LINK</a>");
            scope.selectedItem = "";
            scope.sfEditorContent = "";

            var template = "<sf-link-selector sf-link-html='sfLinkHtml' sf-selected-item='selectedItem' sf-editor-content='sfEditorContent'></sf-link-selector>";

            commonMethods.compileDirective(template, scope);

            var directiveScope = scope.$$childHead;

            expect(directiveScope.sfSelectedItem.mode).toBe(2);
            expect(directiveScope.sfSelectedItem.displayText).toBe('LINK');
            expect(directiveScope.sfSelectedItem.pageId).toBe('28d7e74c-c789-61c4-9817-ff000095605c');
            expect(directiveScope.sfSelectedItem.rootNodeId).toBe('f669d9a7-009d-4d83-ddaa-000000000002');
            expect(directiveScope.sfSite).toBe(sfSites[1]);
        });

        it('[Manev] / Test sfLinkHtml provided as a jQuery wrapper with valid anchor element with ref, no sfref, no target and no lng param.', function () {

            scope.sfLinkHtml = $("<a href='http://www.mysite.com'>LINK</a>");
            scope.selectedItem = "";
            scope.sfEditorContent = "";

            var template = "<sf-link-selector sf-link-html='sfLinkHtml' sf-selected-item='selectedItem' sf-editor-content='sfEditorContent'></sf-link-selector>";

            commonMethods.compileDirective(template, scope);

            var directiveScope = scope.$$childHead;

            expect(directiveScope.sfSelectedItem.mode).toBe(1);
            expect(directiveScope.sfSelectedItem.displayText).toBe('LINK');
            expect(directiveScope.sfSelectedItem.webAddress).toBe('http://www.mysite.com');

            var renderedText = $('#webAddress').val();
            expect(renderedText).toBe('http://www.mysite.com');

            var openInNewWin = $('#openInNewWin1').prop('checked');
            expect(openInNewWin).toBe(false);
        });

        it('[Manev] / Test sfLinkHtml provided as a jQuery wrapper with valid anchor element with ref, target, no sfref, target and no lng param.', function () {

            scope.sfLinkHtml = $("<a href='http://www.mysite.com' target='_blank'>LINK</a>");
            scope.selectedItem = "";
            scope.sfEditorContent = "";

            var template = "<sf-link-selector sf-link-html='sfLinkHtml' sf-selected-item='selectedItem' sf-editor-content='sfEditorContent'></sf-link-selector>";

            commonMethods.compileDirective(template, scope);

            var directiveScope = scope.$$childHead;

            expect(directiveScope.sfSelectedItem.mode).toBe(1);
            expect(directiveScope.sfSelectedItem.displayText).toBe('LINK');
            expect(directiveScope.sfSelectedItem.webAddress).toBe('http://www.mysite.com');

            var openInNewWin = $('#openInNewWin1').prop('checked');
            expect(openInNewWin).toBe(true);
        });

        it('[Manev] / Test sfLinkHtml provided as a jQuery wrapper with valid anchor element with ref and sfref and lng param and no target.', function () {

            scope.sfLinkHtml = $("<a href='CodeBase/widgettests' sfref='[f669d9a7-009d-4d83-ddaa-000000000002|lng:de]28d7e74c-c789-61c4-9817-ff000095605c'>LINK</a>");
            scope.selectedItem = "";
            scope.sfEditorContent = "";

            var template = "<sf-link-selector sf-link-html='sfLinkHtml' sf-selected-item='selectedItem' sf-editor-content='sfEditorContent'></sf-link-selector>";

            commonMethods.compileDirective(template, scope);

            var directiveScope = scope.$$childHead;

            expect(directiveScope.sfSelectedItem.mode).toBe(2);
            expect(directiveScope.sfSelectedItem.displayText).toBe('LINK');
            expect(directiveScope.sfSelectedItem.pageId).toBe('28d7e74c-c789-61c4-9817-ff000095605c');
            expect(directiveScope.sfSelectedItem.rootNodeId).toBe('f669d9a7-009d-4d83-ddaa-000000000002');
            expect(directiveScope.sfSite).toBe(sfSites[1]);
            expect(directiveScope.sfCulture).toBeDefined();
            expect(directiveScope.sfCulture.Culture).toBe('de');

            var ispagesFromThisSiteRadioChecked = $('#pagesFromThisSiteRadio').prop('checked');
            var openInNewWin = $('#openInNewWin2').prop('checked');
            var textToDisplay2 = $('#textToDisplay2').val();

            expect(ispagesFromThisSiteRadioChecked).toBe(true);
            expect(openInNewWin).toBe(false);
            expect(textToDisplay2).toBe('LINK');
        });

        it('[Manev] / Test sfLinkHtml provided as a jQuery wrapper with valid anchor element with ref and sfref and lng param and with target.', function () {

            scope.sfLinkHtml = $("<a href='CodeBase/widgettests' target='_blank' sfref='[f669d9a7-009d-4d83-ddaa-000000000002|lng:de]28d7e74c-c789-61c4-9817-ff000095605c'>here is the link</a>");
            scope.selectedItem = "";
            scope.sfEditorContent = "";

            var template = "<sf-link-selector sf-link-html='sfLinkHtml' sf-selected-item='selectedItem' sf-editor-content='sfEditorContent'></sf-link-selector>";

            commonMethods.compileDirective(template, scope);

            var directiveScope = scope.$$childHead;

            expect(directiveScope.sfSelectedItem.mode).toBe(2);
            expect(directiveScope.sfSelectedItem.displayText).toBe('here is the link');
            expect(directiveScope.sfSelectedItem.pageId).toBe('28d7e74c-c789-61c4-9817-ff000095605c');
            expect(directiveScope.sfSelectedItem.rootNodeId).toBe('f669d9a7-009d-4d83-ddaa-000000000002');
            expect(directiveScope.sfSite).toBe(sfSites[1]);
            expect(directiveScope.sfCulture).toBeDefined();
            expect(directiveScope.sfCulture.Culture).toBe('de');

            var ispagesFromThisSiteRadioChecked = $('#pagesFromThisSiteRadio').prop('checked');
            var openInNewWin = $('#openInNewWin2').prop('checked');
            var textToDisplay2 = $('#textToDisplay2').val();

            expect(ispagesFromThisSiteRadioChecked).toBe(true);
            expect(openInNewWin).toBe(true);
            expect(textToDisplay2).toBe('here is the link');

            var allSelectors = $(document).find('select');

            var selector = allSelectors.filter(function (index) {
                return $(allSelectors[index]).attr('ng-model') === "sfSite";
            })[0];

            expect(selector).toBeDefined();

            expect($(selector).find($('option'))[1].selected).toBe(true);
            expect($(selector).find($('option'))[1].text).toBe('Site2');
        });

        it('[Manev] / Test sfEditorContent default initialization.', function () {

            scope.sfLinkHtml = "";
            scope.selectedItem = "";

            var ids = ['f669d9a7-009d-4d83-ddaa-000000000002', 'f669d9a7-009d-4d83-ddaa-000000000003', 'f669d9a7-009d-4d83-ddaa-000000000004'];

            var titleCount = 0;

            var sfEditorContents = ids.map(function (item) {
                return "<a id='" + item + "'>Link" + (++titleCount) + "</a>";
            });

            scope.sfEditorContent = sfEditorContents.toString();
            //sfLinkMode
            var template = "<sf-link-selector sf-link-html='sfLinkHtml' sf-selected-item='selectedItem' sf-editor-content='sfEditorContent'></sf-link-selector>";

            commonMethods.compileDirective(template, scope);

            var directiveScope = scope.$$childHead;

            expect(directiveScope.anchors).toEqualData(ids);
        });

        it('[Manev] / Test sfEditorContent.', function () {

            scope.selectedItem = "";
            scope.sfLinkHtml = $("<a href='CodeBase/widgettests/#f669d9a7-009d-4d83-ddaa-000000000002'>LINK</a>");
            var ids = ['f669d9a7-009d-4d83-ddaa-000000000002', 'f669d9a7-009d-4d83-ddaa-000000000003', 'f669d9a7-009d-4d83-ddaa-000000000004'];

            var titleCount = 0;

            var sfEditorContents = ids.map(function (item) {
                return "<a id='" + item + "'>Link" + (++titleCount) + "</a>";
            });

            scope.sfEditorContent = sfEditorContents.toString();
            
            var template = "<sf-link-selector sf-link-html='sfLinkHtml' sf-selected-item='selectedItem' sf-editor-content='sfEditorContent'></sf-link-selector>";

            commonMethods.compileDirective(template, scope);

            var directiveScope = scope.$$childHead;

            expect(directiveScope.sfSelectedItem.displayText).toBe('LINK');

            var isAnchorRadioChecked = $('#anchorRadio').prop('checked');

            expect(isAnchorRadioChecked).toBe(true);

            var allSelectors = $(document).find('select');

            var selector = allSelectors.filter(function (index) {
                return $(allSelectors[index]).attr('ng-model') === "sfSelectedItem.selectedAnchor";
            })[0];

            expect(selector).toBeDefined();
            expect($(selector).find($('option'))[1].selected).toBe(true);
            expect($(selector).find($('option'))[1].text).toBe('f669d9a7-009d-4d83-ddaa-000000000002');
        });
    });
});