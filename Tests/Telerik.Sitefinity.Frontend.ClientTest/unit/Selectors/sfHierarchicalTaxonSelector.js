describe('Hierarchical taxon selector', function () {
    var rootScope;
    var $q;
    var scope;
    var serverContext;
    var directiveMarkup = '<sf-list-selector ' +
                          '   sf-hierarchical-taxon-selector ' +
                          '   sf-selected-item-id="selectedId" ' +
                          '   sf-dialog-header="Header"> ' +
                          '</sf-list-selector>';

    var taxons = [];
    for (i = 0; i < 10; i++) {
        taxons.push({ Id: i, Title: "Taxon " + i });
    }

    var hierarhicalTaxons = [];
    for (i = 0; i < 10; i++) {
        hierarhicalTaxons.push(
            [{
                Id: i,
                Title: "Taxon " + i,
                Items: [{ Title: 'Child Taxon ' + i }]
            }]
        );
    }

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));
    beforeEach(module('sfSelectors'));

    beforeEach(module(function ($provide) {
        $provide.value('sfHierarchicalTaxonService', hierarchicalTaxonService);
    }));

    beforeEach(inject(function (_$rootScope_, _$q_, _serverContext_) {
        rootScope = _$rootScope_;
        $q = _$q_;
        scope = rootScope.$new();
        serverContext = _serverContext_;
    }));

    beforeEach(function () {
        sitefinity.getCategoriesTaxonomyId = function () {
            return 'e5cd6d69-1543-427b-ad62-688a99f5e7d4';
        };
        commonMethods.mockServerContextToEnableTemplateCache();
    });

    beforeEach(function () {
        cleanMarkup();
    });

    afterEach(function () {
        //Tear down.
        cleanMarkup();
    });

    var cleanMarkup = function () {
        var leftOver = $('.testDiv, .modal, .modal-backdrop');
        leftOver.empty();
        leftOver.remove();
    };

    var basePromiseResolver = function (items) {
        var defer = $q.defer();
        defer.resolve({ Items: items });
        return defer.promise;
    };

    var hierarchicalTaxonService = {
        getTaxons: function (taxonomyId, skip, take, search, frontendLanguages) {
            return basePromiseResolver(taxons);
        },
        getChildTaxons: function (parentId, search) {
            return basePromiseResolver(taxons);
        },
        getSpecificItems: function (ids) {
            return basePromiseResolver(hierarhicalTaxons);
        }
    };

    it('[Manev] / Should initialize default properties.', function () {
        commonMethods.compileDirective(directiveMarkup, scope);

        var directiveScope = scope.$$childHead;

        expect(directiveScope.changeButtonText).not.toBeDefined();
        expect(directiveScope.searchIdentifierField).toBe('Title');
        expect(directiveScope.sfIdentifierField).toBe('Breadcrumb');
        expect(directiveScope.hierarchical).toBe(true);
        expect(directiveScope.multiselect).toBe(false);
    });

    it('[Manev] / getItems method test.', function () {
        commonMethods.compileDirective(directiveMarkup, scope);

        var directiveScope = scope.$$childHead;

        directiveScope.open();

        directiveScope.$digest();

        expect(directiveScope.items).toEqualArrayOfObjects(taxons, ['Id', 'Title']);
    });

    it('[Manev] / getChildren method test.', function () {
        commonMethods.compileDirective(directiveMarkup, scope);

        var items;
        var directiveScope = scope.$$childHead;
        directiveScope.getChildren()
                      .then(function (result) {
                          items = result;
                      });
        directiveScope.$digest();
        expect(items).toEqualArrayOfObjects(taxons, ['Id', 'Title']);
    });

    it('[Manev] / getSpecificItems/onSelectedItemsLoadedSuccess methods test.', function () {
        scope.selectedId = "GUID";

        commonMethods.compileDirective(directiveMarkup, scope);

        var directiveScope = scope.$$childHead;

        var items = hierarhicalTaxons.map(function (item) {
            return item[0];
        });

        expect(directiveScope.sfSelectedItems).toEqualArrayOfObjects(items, ['Id', 'Title']);

        for (var i = 0; i < directiveScope.sfSelectedItems.length; i++) {
            var taxon = directiveScope.sfSelectedItems[i];

            expect(taxon.TitlesPath).toBe('Taxon ' + i);
            expect(taxon.Breadcrumb).toBe('Taxon ' + i);
        }
    });

    it('[Manev] / Should set breadcrumb when taxon is selected.', function () {
        scope.selectedId = "GUID";

        commonMethods.compileDirective(directiveMarkup, scope);

        var directiveScope = scope.$$childHead;

        var taxon = { Id: '0', Title: 'Taxon', TitlesPath: 'TitlesPath' };

        directiveScope.itemClicked(taxon);

        expect(taxon.Breadcrumb).toBe('TitlesPath > Taxon');
    });

    it('[Manev] / Should set breadcrumb after filtering.', function () {
        scope.selectedId = "GUID";

        serverContext.getFrontendLanguages = function () { };

        commonMethods.compileDirective(directiveMarkup, scope);

        var directiveScope = scope.$$childHead;

        directiveScope.filter.search();

        for (var i = 0; i < directiveScope.sfSelectedItems.length; i++) {
            var taxon = directiveScope.sfSelectedItems[i];

            expect(taxon.TitlesPath).toBe('Taxon ' + i);
            expect(taxon.Breadcrumb).toBe('Taxon ' + i);
        }
    });
});