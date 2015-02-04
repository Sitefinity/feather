describe('Media flat taxon filter', function () {
    var rootScope;
    var provide;
    var $q;
    var mediaService;
    var directiveMarkup = '<span sf-media-flat-taxon-filter ng-model="filterObject" sf-taxonomy-id="CB0F3A19-A211-48a7-88EC-77495C0F5374" sf-title="Tags" sf-field="Tags"></span>';

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));
    beforeEach(module('sfMediaFlatTaxonFilter'));

    beforeEach(module(function ($provide) {
        $provide.value('sfFlatTaxonService', flatTaxonService);

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

    var flatTaxonService = {
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

        $('span ul li:contains("Taxon 5")').click();
        scope.$digest();

        expect(scope.filterObject.taxon).not.toBe(null);
        expect(scope.filterObject.taxon.id).toEqual(5);
        expect(scope.filterObject.basic).toBe(null);
    });
});