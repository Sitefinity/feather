/* Tests for page selector */
describe("page selector", function () {
    var ITEMS_COUNT = 3;

    var customDataItems = {
        Items: [],
        TotalCount: ITEMS_COUNT
    };

    for (var i = 0; i < ITEMS_COUNT; i++) {
        customDataItems.Items[i] = {
            Id: '4c003fb0-2a77-61ec-be54-ff00007864f' + i,
            Title: { Value: 'Dummy' + i },
            HasChildren: true,
            Status: "Published"
        };
    }

    var filteredCollection = {
        Items: [],
        TotalCount: ITEMS_COUNT - 1
    };

    for (i = 0; i < ITEMS_COUNT - 1; i++) {
        filteredCollection.Items[i] = customDataItems.Items[i];
    }

    var serviceResult;
    var $q;
    var provide;
    var $rootScope;

    //Mock news item service. It returns promises.
    var pagesService = {
        getItems: jasmine.createSpy('sfPageService.getItems').andCallFake(function (parentId, siteId, provider, search) {
            if ($q) {
                serviceResult = $q.defer();
            }

            if (search) {
                serviceResult.resolve({
                    Items: filteredCollection.Items,
                    TotalCount: 1
                });
            }
            else {
                serviceResult.resolve(customDataItems);
            }

            return serviceResult.promise;
        }),
        getSpecificItems: jasmine.createSpy('sfPageService.getSpecificItems').andCallFake(function (ids, provider) {
            if ($q) {
                serviceResult = $q.defer();
            }

            serviceResult.resolve(filteredCollection);

            return serviceResult.promise;
        }),
        getPredecessors: jasmine.createSpy('sfPageService.getPredecessors')
    };

    //Load the module responsible for the modal dialog
    beforeEach(module('modalDialog'));

    //Load the kendo module directives .
    beforeEach(module('kendo.directives'));

    //Load the module under test.
    beforeEach(module('sfSelectors'));

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));

    beforeEach(module(function ($provide) {
        //Force angular to use the mock.
        $provide.value('sfPageService', pagesService);

        provide = $provide;
    }));

    beforeEach(inject(function (_$rootScope_, $httpBackend, _$q_, $templateCache, _$timeout_) {
        $rootScope = _$rootScope_;

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
        var leftOver = $('.testDiv, .modal, .modal-backdrop');
        leftOver.empty();
        leftOver.remove();
        pagesService.getItems.reset();
        pagesService.getSpecificItems.reset();
        pagesService.getPredecessors.reset();
    });

    describe('check default properties initialization of page selector', function () {
        it('[manev] / should init default page selector values.', function () {
            var scope = $rootScope.$new();
            scope.provider = 'OpenAccessDataProvider';

            var template = "<sf-list-selector sf-page-selector sf-multiselect='true' sf-identifier-field='TitlesPath' />";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            var pageSelecotrScope = scope.$$childHead;
            expect(pageSelecotrScope).toBeDefined();
            expect(pageSelecotrScope.sfIdentifierField).toBe("TitlesPath");
            expect(pageSelecotrScope.searchIdentifierField).toBe("Title");

            expect(pageSelecotrScope.items).toEqualArrayOfObjects(customDataItems.Items, ['Id', 'Title', 'Status']);
        });

        it('[manev] / should filter items when text is typed in the filter box.', function () {
            var scope = $rootScope.$new();
            scope.provider = 'OpenAccessDataProvider';
            var template = "<sf-list-selector sf-page-selector sf-multiselect='true' />";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            //Apply filter
            s.$apply(function () {
                s.filter.searchString = 'filter';
                s.filter.search(s.filter.searchString);
            });

            expect(s.items).toBeDefined();

            expect(s.items).toEqualArrayOfObjects(filteredCollection.Items, ['Id', 'Title']);
        });

        it('[manev] / should mark item as selected when the dialog is opened.', function () {
            var scope = $rootScope.$new();
            scope.provider = 'OpenAccessDataProvider';
            var template = "<sf-list-selector sf-page-selector sf-multiselect='true' sf-selected-ids='selectedIds' />";

            var ids = filteredCollection.Items.map(function (item) {
                return item.Id;
            });

            scope.selectedIds = ids;

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(filteredCollection.Items.length);
            expect(s.selectedItemsInTheDialog[0].Id).toEqual(filteredCollection.Items[0].Id);
        });

        it('[GeorgiMateev] / should reset all items when the site is changed.', function () {
             var template = "<sf-list-selector sf-page-selector='sites.site' sf-selected-item-id='scope.selectedId'/>";
             var scope = $rootScope.$new();
             scope.sites = {
                 site: {}
             };
             scope.selectedId = '12345';
             scope.$digest();

             commonMethods.compileDirective(template, scope);

             scope.sites.site = {
                 SiteMapRootNodeId: '123',
                 Id: '1234'
             };

             scope.$apply();

             expect(pagesService.getItems).toHaveBeenCalledWith(
                 scope.sites.site.SiteMapRootNodeId,
                 scope.sites.site.Id,
                 scope.provider,
                 undefined,
                 undefined);

             expect(pagesService.getSpecificItems).not.toHaveBeenCalled();

             expect(pagesService.getPredecessors).not.toHaveBeenCalled();
        });

        it('[GeorgiMateev] / should not reset items when the culture is changed.', function () {
             var template = "<sf-list-selector sf-page-selector sf-culture='culture' sf-selected-item-id='scope.selectedId'/>";
             var scope = $rootScope.$new();
             scope.selectedId = '12345';
             scope.$digest();

             commonMethods.compileDirective(template, scope);

             scope.culture = {
                 Culture: 'de'
             };

             scope.$apply();

             expect(pagesService.getItems).not.toHaveBeenCalled();

             expect(pagesService.getSpecificItems).not.toHaveBeenCalled();
        });

        it('[GeorgiMateev] / should reset items when the culture is changed and filter is applied.', function () {
             var template = "<sf-list-selector sf-page-selector sf-culture='cultures.culture' sf-selected-item-id='scope.selectedId'></sf-list-selector>";
             var scope = $rootScope.$new();
             scope.selectedId = '12345';
             scope.cultures = {
                 culture: {}
             };
             scope.$digest();

             commonMethods.compileDirective(template, scope);
             scope.$$childHead.filter.isEmpty = false;
             scope.cultures.culture = {
                 Culture: 'de'
             };

             scope.$apply();

             expect(pagesService.getItems).toHaveBeenCalledWith(
                 "850B39AF-4190-412E-9A81-C72B04A34C0F",
                 undefined,
                 undefined,
                 undefined,
                 'de');

             expect(pagesService.getSpecificItems).not.toHaveBeenCalled();

             expect(pagesService.getPredecessors).not.toHaveBeenCalled();
        });
    });
});