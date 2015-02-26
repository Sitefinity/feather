/* Tests for library selector */
describe("library selector", function () {
    var scope,
        ITEMS_COUNT = 10;

    //Will be returned from the service mock.
    var dataItem = {
        Id: '4c003fb0-2a77-61ec-be54-ff00007864f4',
        ParentId: null,
        HasChildren: true,
        Path: null,
        Title: 'Dummy'
    };

    var defaultLibrary = {
        Id: 'DEFAULT-LIBRARY-ID',
        ParentId: null,
        HasChildren: false,
        Path: null,
        Title: 'Default library'
    };

    var childDataItem = {
        Id: '4c003fb0-2a77-61ec-be54-ff00007864f5',
        ParentId: '4c003fb0-2a77-61ec-be54-ff00007864f4',
        HasChildren: false,
        Path: 'Dummy > DummyChild',
        Title: 'DummyChild'
    };

    var dataItem2 = {
        Id: '4c003fb0-2a77-61ec-be54-ff11117864f4',
        ParentId: null,
        HasChildren: false,
        Title: 'Filtered',
        Path: null,
        Filter: true
    };

    var dataItems = {
        Items: [dataItem, dataItem2, childDataItem],
        TotalCount: 3
    };

    var customDataItems = {
        Items: [],
        TotalCount: ITEMS_COUNT
    };

    for (var i = 0; i < ITEMS_COUNT; i++) {
        customDataItems.Items[i] = {
            Id: '4c003fb0-2a77-61ec-be54-ff00007864f' + i,
            Title: 'Dummy' + i
        };
    }

    var serviceResult;
    var $q;
    var provide;

    var imagesObj = {
        getFolders: jasmine.createSpy('sfMediaService.images.getFolders').andCallFake(function (options) {
            if ($q) {
                serviceResult = $q.defer();
            }

            if (options.filter) {
                serviceResult.resolve({
                    Items: [dataItem2],
                    TotalCount: 1
                });
            }
            else if (options.take === 1 && options.sort === 'DateCreated ASC') {
                serviceResult.resolve({
                    Items: [defaultLibrary],
                    TotalCount: 1
                });
            }
            else {
                serviceResult.resolve(dataItems);
            }

            return serviceResult.promise;
        })
    };

    //Mock image item service. It returns promises.
    var mediaService = {
        images: imagesObj
    };

    //Load the module responsible for the modal dialog
    beforeEach(module('modalDialog'));

    //Load the module under test.
    beforeEach(module('sfSelectors'));

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));

    beforeEach(module(function ($provide) {
        //Force angular to use the mock.
        $provide.value('sfMediaService', mediaService);

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

        //The selector mutates the selected item when it is retrieved from the service.
        dataItem.Title = 'Dummy';
        dataItem.Breadcrumb = null;
        dataItem.RootPath = null;
        dataItem.TitlesPath = null;
        dataItem2.Title = 'Filtered';
        dataItem2.Breadcrumb = null;
        dataItem2.RootPath = null;
        dataItem2.TitlesPath = null;
        dataItem2.Path = null;
        childDataItem.Title = 'DummyChild';
        childDataItem.Breadcrumb = null;
        childDataItem.RootPath = null;
        childDataItem.TitlesPath = null;

        mediaService.images.getFolders.calls = [];

        //Sets default mediaService mock.
        provide.value('sfMediaService', mediaService);
    });

    var libraryServiceGetItemsArgs = function () {
        var mostRecent = mediaService.images.getFolders.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        return mostRecent.args;
    };

    describe('in single selection mode', function () {

        it('[EGaneva] / should call image service when no media type is provided.', function () {
            var template = "<sf-list-selector sf-library-selector sf-provider='provider'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            var args = libraryServiceGetItemsArgs();

            expect(args).toBeDefined();
        });

        it('[EGaneva] / should retrieve image libraries from the service when the selector is opened.', function () {
            var template = "<sf-list-selector sf-library-selector sf-media-type='images' sf-provider='provider'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            var args = libraryServiceGetItemsArgs();

            expect(args[0]).toBeDefined();

            //parent
            expect(args[0].parent).toBe(null);

            //sort
            expect(args[0].sort).toBe('Title ASC');

            //Provider
            expect(args[0].provider).toBe('OpenAccessDataProvider');
        });

        it('[EGaneva] / should retrieve child image libraries from the service when the parent is expanded.', function () {
            var template = "<sf-list-selector sf-library-selector sf-media-type='images' sf-provider='provider'/>";

            var element = commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            scope.$$childTail.getChildren("4c003fb0-2a77-61ec-be54-ff00007864f4");

            var args = libraryServiceGetItemsArgs();

            expect(args[0]).toBeDefined();

            //parent
            expect(args[0].parent).toBe("4c003fb0-2a77-61ec-be54-ff00007864f4");

            //provider
            expect(args[0].provider).toBe('OpenAccessDataProvider');

            //sort
            expect(args[0].sort).toBe('Title ASC');

            //filter
            expect(args[0].filter).not.toBe();
        });

        it('[EGaneva] / should assign value to "selected-item" when "selected-item-id" is provided.', function () {
            var template = "<sf-list-selector sf-library-selector sf-media-type='images' sf-selected-item='selectedItem' sf-selected-item-id='selectedId'/>";

            scope.selectedId = dataItem2.Id;

            commonMethods.compileDirective(template, scope);
            scope.$digest();

            expect(scope.selectedId).toBeDefined();
            expect(scope.selectedId).toEqual(dataItem2.Id);

            expect(scope.selectedItem).toBeDefined();
            expect(scope.selectedItem.Id).toEqual(dataItem2.Id);
        });

        it('[EGaneva] / should assign value to "selected-item-id" when "selected-item" is provided.', function () {
            var template = "<sf-list-selector sf-library-selector sf-media-type='images' sf-selected-item='selectedItem' sf-selected-item-id='selectedId'/>";

            scope.selectedItem = dataItem2;

            commonMethods.compileDirective(template, scope);
            scope.$digest();

            expect(scope.selectedId).toBeDefined();
            expect(scope.selectedId).toEqual(dataItem2.Id);

            expect(scope.selectedItem).toBeDefined();
            expect(scope.selectedItem.Id).toEqual(dataItem2.Id);
        });

        it('[EGaneva] / should select folder when Done button is pressed.', function () {
            var template = "<sf-list-selector sf-library-selector sf-media-type='images' sf-selected-item='selectedItem' sf-selected-item-id='selectedId'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            //mock the call to the modal service.
            s.$modalInstance = { close: function () { } };

            expect(s.sfSelectedItem).not.toBeDefined();

            expect(s.items).toBeDefined();
            expect(s.items[0].Id).toEqual(dataItem.Id);
            expect(s.items[1].Id).toEqual(dataItem2.Id);

            //Select item in the selector
            s.itemClicked(0, s.items[0]);

            expect(s.sfSelectedItem).not.toBeDefined();

            //Close the dialog (Done button clicked)
            s.doneSelecting();

            expect(s.sfSelectedItem).toBeDefined();
            expect(s.sfSelectedItem.Id).toEqual(dataItem.Id);

            expect(s.sfSelectedItemId).toBeDefined();
            expect(s.sfSelectedItemId).toEqual(dataItem.Id);
        });

        it('[EGaneva] / should filter items when text is typed in the filter box.', function () {
            var template = "<sf-list-selector sf-library-selector sf-media-type='images' sf-provider='provider'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.items).toBeDefined();
            expect(s.items[0].Id).toEqual(dataItem.Id);
            expect(s.items[0].Filter).toBeFalsy();

            //Apply filter
            s.$apply(function () {
                s.filter.searchString = 'filter';
                s.filter.search(s.filter.searchString);
            });

            var args = libraryServiceGetItemsArgs();

            expect(args[0]).toBeDefined();

            //parent
            expect(args[0].parent).toBe(null);

            //sort
            expect(args[0].sort).toBe('Title ASC');

            //Filter
            expect(args[0].filter).toBe('(Title.ToUpper().Contains("filter".ToUpper()))');

            //Provider
            expect(args[0].provider).toBe('OpenAccessDataProvider');

            expect(s.items).toBeDefined();
            expect(s.items[0].Id).toEqual(dataItem2.Id);
            expect(s.items[0].Filter).toBe(true);
            expect(s.filter.searchString).toBe('filter');
        });

        it('[EGaneva] / should mark item as selected when the dialog is opened.', function () {
            var template = "<sf-list-selector sf-library-selector sf-media-type='images' sf-selected-item-id='selectedId'/>";

            scope.selectedId = dataItem2.Id;

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog[0].Id).toEqual(dataItem2.Id);
        });

        it('[EGaneva] / should deselect the item if it is clicked and it is already selected.', function () {
            var template = "<sf-list-selector sf-library-selector sf-media-type='images' sf-selected-item-id='selectedId'/>";

            scope.selectedId = dataItem2.Id;

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(1);
            expect(s.selectedItemsInTheDialog[0].Id).toEqual(dataItem2.Id);

            //Select item in the selector
            s.itemClicked(0, s.items[1]);

            expect(s.selectedItemsInTheDialog).toBeDefined();
            expect(s.selectedItemsInTheDialog.length).toEqual(0);
        });

        it('[NPetrova] / should add breadcrumb when root level item is selected', function () {
            var template = "<sf-list-selector sf-library-selector sf-media-type='images' sf-selected-item-id='selectedId'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.items[1].Breadcrumb).not.toBe();

            //Select item in the selector
            s.itemClicked(0, s.items[1]);

            expect(s.items[1].Breadcrumb).toBeDefined();
            expect(s.items[1].Breadcrumb).toBe('Filtered');
        });

        it('[NPetrova] / should add breadcrumb when child item is selected', function () {
            var template = "<sf-list-selector sf-library-selector sf-media-type='images' sf-selected-item-id='selectedId'/>";

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.items[2].Breadcrumb).not.toBe();

            //Select item in the selector
            s.itemClicked(0, s.items[2]);

            expect(s.items[2].Breadcrumb).toBeDefined();
            expect(s.items[2].Breadcrumb).toBe('Dummy > DummyChild');
        });

        it('[NPetrova] / should add child item\'s RootPath when items are filtered', function () {
            var template = "<sf-list-selector sf-library-selector sf-media-type='images' sf-provider='provider'/>";

            dataItem2.Path = "Parent > Child";
            dataItem2.Title = "Child";

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
            expect(s.items[0].Id).toEqual(dataItem2.Id);
            expect(s.items[0].RootPath).toBe('Under Parent');
        });

        it('[NPetrova] / should add root level item\'s RootPath when items are filtered', function () {
            var template = "<sf-list-selector sf-library-selector sf-media-type='images' sf-provider='provider'/>";

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
            expect(s.items[0].Id).toEqual(dataItem2.Id);
            expect(s.items[0].RootPath).toBe('On Top Level');
        });

        it('[NPetrova] / should deselect the item if it is clicked and it is already selected.', function () {
            var template = "<sf-list-selector sf-library-selector sf-media-type='images' sf-selected-item-id='selectedId'/>";

            scope.selectedId = childDataItem.Id;

            commonMethods.compileDirective(template, scope);

            $('.openSelectorBtn').click();

            //The scope of the selector is isolated, but it's child of the scope used for compilation.
            var s = scope.$$childHead;

            expect(s.sfSelectedItem.Breadcrumb).toBeDefined();
            expect(s.sfSelectedItem.Breadcrumb).toBe('Filtered');
            expect(s.sfSelectedItem.TitlesPath).toBeDefined();
            expect(s.sfSelectedItem.TitlesPath).toBe('Filtered');
        });
    });
});
