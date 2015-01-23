/* Tests for news selector */
describe("news selector", function () {
    var scope;
    var serviceResult;
    var $q;
    var provide;

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
            "IsDefault": true,
            "IsDeleteable": false,
            "IsOffline": false,
            "Name": "Site2",
            "SiteMapRootNodeId": "f669d9a7-009d-4d83-ddaa-000000000003",
            "SiteUrl": null,
            "UIStatus": "Online"
        }];

    //Mock link service.
    var linkService = {
        populateAnchorIds: jasmine.createSpy('linkService.populateAnchorIds').andCallFake(function (editorContent) {
            if ($q) {
                serviceResult = $q.defer();
            }

            serviceResult.resolve(dataItems);

            return serviceResult.promise;
        }),
    };

    //Mock site service.
    var siteService = {
        getSites: jasmine.createSpy('siteService.getSites').andCallFake(function (editorContent) {
            return sfSites;
        }),
        getSiteByRootNoteId: jasmine.createSpy('siteService.getSiteByRootNoteId').andCallFake(function (editorContent) {
            return sfSites[1];
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
        $provide.value('linkService', linkService);

        provide = $provide;
    }));

    beforeEach(module(function ($provide) {
        //Force angular to use the mock.
        $provide.value('sfMultiSiteService', siteService);

        provide = $provide;
    }));

    beforeEach(inject(function ($rootScope, $httpBackend, _$q_, $templateCache, _$timeout_) {
        //Build the scope with whom the directive will be compiled.
        scope = $rootScope.$new();
        scope.provider = 'OpenAccessDataProvider';

        $q = _$q_;
        $timeout = _$timeout_;

        serviceResult = _$q_.defer();
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
        iit('[Manev] / Check default property initialization.', function () {

            scope.selectedHtml = "";
            scope.selectedItem = "";
            scope.ngModel = "";

            debugger;

            var template = "<sf-link-selector sf-link-html='selectedHtml' sf-selected-item='selectedItem' sf-editor-content='ngModel'></sf-link-selector>";

            commonMethods.compileDirective(template, scope);

            var directiveScope = scope.$$childHead;

            //expect(childScope.sfCultures).toBeUndefined();

            expect(directiveScope.sfSelectedItem.mode).toBeEqual(1);
            
            expect(directiveScope.sfSite.mode).toBeEqual(sfSites[1]);

        });
    });
});