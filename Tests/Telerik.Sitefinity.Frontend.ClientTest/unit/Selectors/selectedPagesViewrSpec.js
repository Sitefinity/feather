/* Tests for selected pages view */
describe("selected pages view", function () {
    var scope,
        ITEMS_COUNT = 3;

    var customDataItems = {
        Items: [],
        TotalCount: ITEMS_COUNT
    };

    var externalPages = {
        Items: [],
        TotalCount: ITEMS_COUNT
    };

    var filteredCollection = {
        Items: [],
        TotalCount: ITEMS_COUNT - 1
    };

    for (var i = 0; i < ITEMS_COUNT - 1; i++) {
        filteredCollection.Items[i] = customDataItems.Items[i];
    }

    //Load the kendo module directives .
    beforeEach(module('kendo.directives'));

    //Load the module under test.
    beforeEach(module('sfSelectors'));

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));

    beforeEach(module(function ($provide) {
        //Force angular to use the mock.
        provide = $provide;
    }));

    beforeEach(inject(function ($rootScope, _$httpBackend_, _$q_, $templateCache, _$timeout_) {
        //Build the scope with whom the directive will be compiled.
        scope = $rootScope.$new();
    }));

    beforeEach(function () {
        commonMethods.mockServerContextToEnableTemplateCache();
    });

    afterEach(function () {
        tearDownCleanUp();
    });

    beforeEach(function () {
        tearDownCleanUp();

        for (var i = 0; i < ITEMS_COUNT; i++) {
            customDataItems.Items[i] = {
                Id: '4c003fb0-2a77-61ec-be54-ff00007864f' + i,
                Title: { Value: 'Dummy' + i },
                HasChildren: true,
                Status: "Published"
            };
        }

        for (var i = 0; i < ITEMS_COUNT; i++) {
            externalPages.Items[i] = {
                ExternalPageId: '1c003fb0-2a77-61ec-be54-ff00007864f' + i,
                Title: { Value: 'Dummy' + i },
            };
        }
    });

    var tearDownCleanUp = function () {
        var leftOver = $('.testDiv, .modal, .modal-backdrop');
        leftOver.empty();
        leftOver.remove();
    };

    describe('check default properties initialization of selected page view', function () {
        it('[manev] / should init default selected page view with only external pages.', function () {

            scope.selectedItemsViewData = externalPages.Items;
            scope.selectedItemsInTheDialog = externalPages.Items;

            var template = "<sf-selected-items-view" +
                            "    sf-selected-pages-view" +
                            "    sf-template-url='client-components/selectors/pages/sf-selected-pages-view.html'" +
                            "    sf-items='selectedItemsViewData' " +
                            "    sf-selected-items='selectedItemsInTheDialog' " +
                            "    sf-search-identifier-field='searchIdentifierField'" +
                            "    sf-identifier-field='sfIdentifierField'> " +
                            "</sf-selected-items-view>";

            commonMethods.compileDirective(template, scope);

            var directiveScope = scope.$$childHead;

            expect(externalPages.Items).toEqualArrayOfObjects(directiveScope.currentItems, ['ExternalPageId', 'Title']);

            var renderedSpans = $('.list-group-item > div > span');

            expect(renderedSpans.length).toBe(3);

            for (var i = 0; i < renderedSpans.length; i++) {
                var span = renderedSpans[i];
                expect($(span).html()).toBe(externalPages.Items[i].Title.Value);
            }
        });

        it('[manev] / should init default selected page view with only internal pages.', function () {

            scope.selectedItemsViewData = customDataItems.Items;
            scope.selectedItemsInTheDialog = customDataItems.Items;

            var template = "<sf-selected-items-view" +
                            "    sf-selected-pages-view" +
                            "    sf-template-url='client-components/selectors/pages/sf-selected-pages-view.html'" +
                            "    sf-items='selectedItemsViewData' " +
                            "    sf-selected-items='selectedItemsInTheDialog' " +
                            "    sf-search-identifier-field='searchIdentifierField'" +
                            "    sf-identifier-field='sfIdentifierField'> " +
                            "</sf-selected-items-view>";

            commonMethods.compileDirective(template, scope);

            var directiveScope = scope.$$childHead;

            expect(customDataItems.Items).toEqualArrayOfObjects(directiveScope.currentItems, ['Id', 'Title']);

            var renderedSpans = $('.list-group-item > div > span');

            expect(renderedSpans.length).toBe(3);

            for (var i = 0; i < renderedSpans.length; i++) {
                var span = renderedSpans[i];
                expect($(span).html()).toBe(customDataItems.Items[i].Title.Value);
            }
        });

        it('[manev] / should init default selected page view with external and internal pages.', function () {

            var mixPages = customDataItems.Items.concat(externalPages.Items);

            scope.selectedItemsViewData = mixPages;
            scope.selectedItemsInTheDialog = mixPages;

            var template = "<sf-selected-items-view" +
                            "    sf-selected-pages-view" +
                            "    sf-template-url='client-components/selectors/pages/sf-selected-pages-view.html'" +
                            "    sf-items='selectedItemsViewData' " +
                            "    sf-selected-items='selectedItemsInTheDialog' " +
                            "    sf-search-identifier-field='searchIdentifierField'" +
                            "    sf-identifier-field='sfIdentifierField'> " +
                            "</sf-selected-items-view>";

            commonMethods.compileDirective(template, scope);

            var directiveScope = scope.$$childHead;

            expect(mixPages).toEqualArrayOfObjects(directiveScope.currentItems, ['Title']);

            var renderedSpans = $('.list-group-item > div > span');
            
            expect(renderedSpans.length).toBe(6);
            
            for (var i = 0; i < mixPages.length; i++) {
                var span = renderedSpans[i];
                expect($(span).html()).toBe(mixPages[i].Title.Value);
            }
        });
    });

    describe('check removing/adding items in selected page view', function () {
        it('[manev] / should remove first External page item selected in page view.', function () {
            scope.selectedItemsViewData = externalPages.Items;
            scope.selectedItemsInTheDialog = externalPages.Items;

            var template = "<sf-selected-items-view" +
                            "    sf-selected-pages-view" +
                            "    sf-template-url='client-components/selectors/pages/sf-selected-pages-view.html'" +
                            "    sf-items='selectedItemsViewData' " +
                            "    sf-selected-items='selectedItemsInTheDialog' " +
                            "    sf-search-identifier-field='searchIdentifierField'" +
                            "    sf-identifier-field='sfIdentifierField'> " +
                            "</sf-selected-items-view>";

            commonMethods.compileDirective(template, scope);

            var directiveScope = scope.$$childHead;
            
            var checkboxes = $('.list-group-item > input[type=checkbox]');

            $(checkboxes[0]).click();

            expect(directiveScope.currentItems).toEqualArrayOfObjects(externalPages.Items.splice(0, 1), ['ExternalPageId']);
        });

        it('[manev] / should remove first internal page item selected in page view.', function () {
            scope.selectedItemsViewData = customDataItems.Items;
            scope.selectedItemsInTheDialog = customDataItems.Items;

            var template = "<sf-selected-items-view" +
                            "    sf-selected-pages-view" +
                            "    sf-template-url='client-components/selectors/pages/sf-selected-pages-view.html'" +
                            "    sf-items='selectedItemsViewData' " +
                            "    sf-selected-items='selectedItemsInTheDialog' " +
                            "    sf-search-identifier-field='searchIdentifierField'" +
                            "    sf-identifier-field='sfIdentifierField'> " +
                            "</sf-selected-items-view>";

            commonMethods.compileDirective(template, scope);

            var directiveScope = scope.$$childHead;

            var checkboxes = $('.list-group-item > input[type=checkbox]');

            $(checkboxes[0]).click();

            expect(directiveScope.currentItems).toEqualArrayOfObjects(customDataItems.Items.splice(0, 1), ['ExternalPageId']);
        });

        it('[manev] / should remove all with internal and internal pages item selected in page view.', function () {

            var mixPages = customDataItems.Items.concat(externalPages.Items);

            scope.selectedItemsViewData = mixPages;
            scope.selectedItemsInTheDialog = mixPages;

            var template = "<sf-selected-items-view" +
                            "    sf-selected-pages-view" +
                            "    sf-template-url='client-components/selectors/pages/sf-selected-pages-view.html'" +
                            "    sf-items='selectedItemsViewData' " +
                            "    sf-selected-items='selectedItemsInTheDialog' " +
                            "    sf-search-identifier-field='searchIdentifierField'" +
                            "    sf-identifier-field='sfIdentifierField'> " +
                            "</sf-selected-items-view>";

            commonMethods.compileDirective(template, scope);

            var directiveScope = scope.$$childHead;

            var checkboxes = $('.list-group-item > input[type=checkbox]');

            checkboxes.each(function (index) {
                $(this).click();
            });
            
            expect(directiveScope.currentItems.length).toBe(0);
            expect(scope.selectedItemsInTheDialog.length).toBe(0);
        });
    });
});