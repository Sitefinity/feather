describe('Image selector', function () {
    var rootScope;
    var provide;
    var $q;
    var originalMediaService;
    var directiveMarkup = '<sf-image-selector/>';

    beforeEach(module('templates'));
    beforeEach(module('sfImageSelector'));

    beforeEach(module(function ($provide) {
        $provide.value('sfMediaService', sfMediaService);
        $provide.value('sfFlatTaxonService', flatTaxonService);
        $provide.value('sfHierarchicalTaxonService', sfHierarchicalTaxonService);

        provide = $provide;
    }));

    beforeEach(inject(function (_$rootScope_, _$q_, sfMediaService) {
        rootScope = _$rootScope_;
        $q = _$q_;
        originalMediaService = sfMediaService;
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

    var itemsPromiseTransform = function (items) {
        var defer = $q.defer();
        defer.resolve({ Items: items });
        return defer.promise;
    }

    var genericGet = function () {
        var items = [];
        for (var i = 1; i <= 5; i++) {
            items[i] = {
                HasChildren: true,
                Id: i,
                Title: 'Title'+i
            };
        }

        return items;
    }

    var sfMediaService = {
        newFilter: originalMediaService.newFilter,
        images: {
            getFolders: function (options) {
                return itemsPromiseTransform(genericGet());
            },
            getImages: function (options) {
                return itemsPromiseTransform(genericGet());
            },
            getContent: function (options) {
                return itemsPromiseTransform(genericGet());
            }
        }
    };

    var sfFlatTaxonService = {
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

    var sfHierarchicalTaxonService = {
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

    /*
    * Filters
    */

    // Library filter
    (function () {
        it('[dzhenko] / should properly set parent id of filter object', function () {
            var scope = $rootScope.$new();
            commonMethods.compileDirective(directiveMarkup, scope);
            var s = scope.$$childHead;

            expect(s.filters.library.selected).toBe(null);

            $('ul li span:contains("Title1")').click();
            scope.$digest();

            expect(scope.filterObject.parent).toEqual('1');
        });

        it('[dzhenko] / should properly set basic property to null when parent Id is selected', function () {
            var scope = $rootScope.$new();

            scope.filterObject = mediaService.newFilter();

            mediaService.specTest = getFoldersObj;

            var directiveMarkup = '<span sf-library-filter sf-model="filterObject" sf-media-type="specTest"></span>';

            commonMethods.compileDirective(directiveMarkup, scope);

            scope.filterObject.basic = "Initial Value";
            scope.$digest();

            expect(scope.filterObject.parent).toBe(null);
            expect(scope.filterObject.basic).toEqual("Initial Value");

            $('ul li span:contains("Title1")').click();
            scope.$digest();

            expect(scope.filterObject.parent).toEqual('1');
            expect(scope.filterObject.basic).toBe(null);
        });
    }());
});