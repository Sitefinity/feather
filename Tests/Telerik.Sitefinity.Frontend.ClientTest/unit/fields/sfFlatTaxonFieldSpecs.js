describe('Flat taxon field', function () {
    var rootScope;
    var provide;
    var $q;
    var mediaService;
    var directiveMarkup = '<sf-flat-taxon-field sf-model="selectedTags" sf-taxonomy-id="CB0F3A19-A211-48a7-88EC-77495C0F5374"></sf-flat-taxon-field>';

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));
    beforeEach(module('sfFlatTaxonField'));

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
        getSpecificItems: function (taxonomyId, ids) {
            var result = [];
            for (var i = 0; i < ids.length; i++) {
                result.push({ Id: ids[i], Title: 'Taxon ' + ids[i] });
            }

            var defer = $q.defer();
            defer.resolve({ Items: result });
            return defer.promise;
        }
    };

    it('[Boyko-Karadzhov] / should render taxon titles.', function () {
        var scope = rootScope.$new();
        scope.selectedTags = ['id-123', 'id-456', 'id-789'];

        var element = commonMethods.compileDirective(directiveMarkup, scope);
        scope.$digest();

        expect($(element).find('span:contains("Taxon id-123")').length).toEqual(1);
        expect($(element).find('span:contains("Taxon id-456")').length).toEqual(1);
        expect($(element).find('span:contains("Taxon id-789")').length).toEqual(1);
    });
});