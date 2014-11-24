/* Tests for page selector */
describe("page selector", function () {
    var scope,
        ITEMS_COUNT = 3;

    var customDataItems = {
        Items: [],
        TotalCount: ITEMS_COUNT
    };

    for (var i = 0; i < ITEMS_COUNT; i++) {
        customDataItems.Items[i] = {
            Id: '4c003fb0-2a77-61ec-be54-ff00007864f' + i,
            Title: { Value: 'Dummy' + i },
            HasChildren: true
        };
    }

    var filteredCollection = {
        Items: [],
        TotalCount: ITEMS_COUNT - 1
    };

    for (var i = 0; i < ITEMS_COUNT - 1; i++) {
        filteredCollection.Items[i] = customDataItems.Items[i];
    }

    var serviceResult;
    var $q;
    var provide;

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
        })
    };

    //This is the id of the cached templates in $templateCache. The external templates are cached by a karma/grunt preprocessor.
    var pageSelectorTemplatePath = 'client-components/selectors/pages/sf-page-selector.html';
    var listSelectorTemplatePath = 'client-components/selectors/common/sf-list-selector.html';
    var treeSelectorTemplatePath = 'client-components/selectors/common/sf-items-tree.html';

    //Load the module responsible for the modal dialog
    beforeEach(module('modalDialog'));

    //Load the kendo module directives .
    beforeEach(module('kendo.directives'));

    //Load the module under test.
    beforeEach(module('sfSelectors'));

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));

    beforeEach(module(function ($provide) {
        var serverContext = {
            getRootedUrl: function (path) {
                return appPath + '/' + path;
            },
            getUICulture: function () {
                return null;
            },
            getEmbeddedResourceUrl: function (assembly, url) {
                return url;
            },
            getFrontendLanguages: function () {
                return ['en', 'de'];
            },
            getCurrentFrontendRootNodeId: function () {
                return "850B39AF-4190-412E-9A81-C72B04A34C0F";
            }
        };
        $provide.value('serverContext', serverContext);
    }));

    beforeEach(module(function ($provide) {
        //Force angular to use the mock.
        $provide.value('sfPageService', pagesService);

        provide = $provide;
    }));

    beforeEach(inject(function ($rootScope, $httpBackend, _$q_, $templateCache, _$timeout_) {
        //Build the scope with whom the directive will be compiled.
        scope = $rootScope.$new();
        scope.provider = 'OpenAccessDataProvider';

        $q = _$q_;
        $timeout = _$timeout_;

        serviceResult = _$q_.defer();

        //Prevent failing of the template request.
        $httpBackend.whenGET(listSelectorTemplatePath);
        $httpBackend.whenGET(pageSelectorTemplatePath).respond({});
        $httpBackend.whenGET(treeSelectorTemplatePath).respond({});
    }));

    beforeEach(function () {
        this.addMatchers({
            // Used to compare arrays of primitive values
            toEqualArrayOfValues: function (expected) {
                var valid = true;
                for (var i = 0; i < expected.length; i++) {
                    if (expected[i] !== this.actual[i]) {
                        valid = false;
                        break;
                    }
                }
                return valid;
            },

            // Used to compare arrays of data items with Id and Title
            toEqualArrayOfDataItems: function (expected) {
                var valid = true;
                for (var i = 0; i < expected.length; i++) {
                    var id = this.actual[i].item ? this.actual[i].item.Id : this.actual[i].Id;
                    var title = this.actual[i].item ? this.actual[i].item.Title : this.actual[i].Title;
                    if (expected[i].Id !== id || expected[i].Title !== title) {
                        valid = false;
                        break;
                    }
                }
                return valid;
            },
        });
    });

    afterEach(function () {
        //Tear down.
        var leftOver = $('.testDiv, .modal, .modal-backdrop');
        leftOver.empty();
        leftOver.remove();
    });

    /* Helper methods */
    var compileDirective = function (template, container) {
        var cntr = container || 'body';

        inject(function ($compile) {
            var directiveElement = $compile(template)(scope);
            $(cntr).append($('<div/>').addClass('testDiv')
                .append(directiveElement));
        });

        // $digest is necessary to finalize the directive generation
        scope.$digest();
    };

    describe('check default properties initialization of page selector', function () {
        it('[manev] / should init default page selector values.', function () {

            var template = "<sf-list-selector sf-page-selector sf-multiselect='true' />";

            compileDirective(template);

            $('.openSelectorBtn').click();

            var pageSelecotrScope = scope.$$childHead;
            expect(pageSelecotrScope).toBeDefined();
            expect(pageSelecotrScope.sfIdentifierField).toBe("TitlesPath");
            expect(pageSelecotrScope.searchIdentifierField).toBe("Title");

            expect(pageSelecotrScope.items).toEqualArrayOfDataItems(customDataItems.Items);
        });

        it('[manev] / should filter items when text is typed in the filter box.', function () {

            var template = "<sf-list-selector sf-page-selector sf-multiselect='true' />";

            compileDirective(template);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            //Apply filter
            s.$apply(function () {
                s.filter.searchString = 'filter';
                s.filter.search(s.filter.searchString);
            });

            expect(s.items).toBeDefined();

            expect(s.items).toEqualArrayOfDataItems(filteredCollection.Items);
        });

        it('[manev] / should mark item as selected when the dialog is opened.', function () {
            var template = "<sf-list-selector sf-page-selector sf-multiselect='true' sf-selected-ids='selectedIds' />";

            var ids = filteredCollection.Items.map(function (item) {
                return item.Id;
            });

            scope.selectedIds = ids;

            compileDirective(template);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(filteredCollection.Items.length);
            expect(s.selectedItemsInTheDialog[0].item.Id).toEqual(filteredCollection.Items[0].Id);
        });
    });
});