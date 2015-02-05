describe('Image selector', function () {
    var rootScope;
    var $q;
    var directiveMarkup = '<sf-image-selector/>';

    beforeEach(module('templates'));
    beforeEach(module('sfImageSelector'));

    beforeEach(module(function ($provide) {
        $provide.value('sfMediaService', fakeMediaService);
        $provide.value('sfFlatTaxonService', fakeFlatTaxonService);
        $provide.value('sfHierarchicalTaxonService', fakeHierarchicalTaxonService);
    }));

    beforeEach(inject(function (_$rootScope_, _$q_) {
        rootScope = _$rootScope_;
        $q = _$q_;
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

    var fakeMediaService = {
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

    var fakeFlatTaxonService = {
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

    var fakeHierarchicalTaxonService = {
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
        it('[dzhenko] / library filter: should properly set parent id of filter object', function () {
            var scope = rootScope.$new();
            commonMethods.compileDirective(directiveMarkup, scope);
            var s = scope.$$childHead;

            expect(s.filters.library.selected[0]).toBeUndefined();

            $('ul.sf-tree li span:contains("Title1")').first().click();
            scope.$digest();

            expect(s.filters.library.selected[0]).toEqual(1);
        });

        it('[dzhenko] / library filter: should properly set other selected values to null when parent id of filter object is set', function () {
            var scope = rootScope.$new();
            commonMethods.compileDirective(directiveMarkup, scope);
            var s = scope.$$childHead;

            $('ul.sf-tree li span:contains("Title1")').first().click();
            scope.$digest();

            expect(s.filters.tag.selected[0]).toBeUndefined();
            expect(s.filters.category.selected[0]).toBeUndefined();
            expect(s.filters.date.selected[0]).toBeUndefined();

            expect(s.filters.tag.query).toBe(null);
            expect(s.filters.category.query).toBe(null);
        });
    }());
});