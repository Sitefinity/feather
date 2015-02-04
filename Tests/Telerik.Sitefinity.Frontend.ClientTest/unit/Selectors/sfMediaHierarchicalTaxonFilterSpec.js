describe('Media hierarchical taxon filter', function () {
    var rootScope;
    var provide;
    var $q;
    var mediaService;
    var directiveMarkup = '<span sf-media-hierarchical-taxon-filter sf-model="filterObject" sf-taxonomy-id="CATEGORIES-ID" sf-title="Categories" sf-field="Category"></span>';

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));
    beforeEach(module('sfMediaHierarchicalTaxonFilter'));

    beforeEach(module(function ($provide) {
        $provide.value('sfHierarchicalTaxonService', hierarchicalTaxonService);

        provide = $provide;
    }));

    beforeEach(inject(function (_$rootScope_, _$q_, sfMediaService) {
        rootScope = _$rootScope_;
        $q = _$q_;
        mediaService = sfMediaService;
    }));

    beforeEach(function () {
        commonMethods.mockServerContextToEnableTemplateCache();
    });

    afterEach(function () {
        //Tear down.
        var leftOver = $('.testDiv');
        leftOver.empty();
        leftOver.remove();
    });

    var hierarchicalTaxonService = {
        getTaxons: function (taxonomyId, skip, take, search, frontendLanguages) {
            var result = [];
            for (var i = skip; i < take + skip && i < 100; i++) {
                if (search) {
                    result.push({ Id: i, Title: search + " " + i });
                }
                else {
                    result.push({ Id: i, Title: "Taxon " + i });
                }
            }
            var defer = $q.defer();
            defer.resolve({ Items: result });
            return defer.promise;
        }
    };

    it('[Boyko-Karadzhov] / should set taxon id in the filter object when taxon is selected.', function () {
        var scope = rootScope.$new();
        scope.filterObject = mediaService.newFilter();
        scope.filterObject.basic = 'someValue';

        commonMethods.compileDirective(directiveMarkup, scope);
        scope.$digest();

        $('span ul li:visible:contains("Taxon 50") span').click();
        scope.$digest();

        expect(scope.filterObject.taxon).not.toBe(null);
        expect(scope.filterObject.taxon.id).toEqual(50);
        expect(scope.filterObject.basic).toBe(null);
    });

    it('[Boyko-Karadzhov] / should set taxon id in the filter object when taxon is selected from search results.', function () {
        var scope = rootScope.$new();
        scope.filterObject = mediaService.newFilter();
        scope.filterObject.basic = 'someValue';

        commonMethods.compileDirective(directiveMarkup, scope);
        scope.$digest();

        $('span input').val('Searched');
        angular.element('span input').triggerHandler('input');
        scope.$apply();

        $('span ul li:visible:contains("Searched 50") span').click();
        scope.$digest();

        expect(scope.filterObject.taxon).not.toBe(null);
        expect(scope.filterObject.taxon.id).toEqual(50);
        expect(scope.filterObject.basic).toBe(null);
    });
});