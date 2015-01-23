/* link selector modal */
describe("link selector modal", function () {
    var scope;
    var serviceResult;
    var $q;
    var provide;
    
    var appPath = 'http://mysite.com:9999/myapp';
    var sfLinkMode;

    //Load the module responsible for the modal dialog
    beforeEach(module('modalDialog'));

    //Load the module under test.
    beforeEach(module('sfSelectors'));

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));

    beforeEach(module(function ($provide) {
        //Force angular to use the mock.
        //$provide.value('sfLinkService', linkService);
        //$provide.value('sfMultiSiteService', siteService);
        //$provide.value('sfLanguageService', languageService);
        //$provide.value('sfPageService', pagesService);
        provide = $provide;
    }));

    beforeEach(module(function ($provide) {
        var serverContext = {
            getRootedUrl: function (path) {
                return appPath + '/' + path;
            },
            getUICulture: function () {
                return null;
            },
            getCurrentUserId: function () {
                return '36e9e47f-0d78-6425-ae98-ff0000fc9faf';
            },
            getCurrentFrontendRootNodeId: function () {
                return '36e9e47f-0d78-6425-ae98-ff0000fc9faf';
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

    describe('Tests for link selector modal logic', function () {
        it('[?] / Test insertLink method implementation.', function () {

          
        });

        it('[?] / Test isDisabled method implementation (when selectedItem.mode == linkMode.WebAddress).', function () {


        });

        it('[?] / Test isDisabled method implementation (when selectedItem.mode == linkMode.InternalPage).', function () {


        });

        it('[?] / Test isDisabled method implementation (when selectedItem.mode == linkMode.Anchor).', function () {


        });

        it('[?] / Test button UI state when it isDisabled return true.', function () {


        });

        it('[?] / Test that properties (selectedHtml/selectedItem/ngModel) are properly propagated to link selector directive.', function () {


        });
    });
});