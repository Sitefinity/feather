describe('Image selector', function () {
    var rootScope;
    var $q;
    var provide;
    var directiveMarkup = '<sf-image-selector/>';

    beforeEach(module('templates'));
    beforeEach(module('sfImageSelector'));

    beforeEach(module(function ($provide) {
        $provide.value('sfMediaService', fakeMediaService);
        $provide.value('sfFlatTaxonService', fakeFlatTaxonService);
        $provide.value('sfHierarchicalTaxonService', fakeHierarchicalTaxonService);

        provide = $provide;
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
                Title: 'Title' + i
            };
        }

        return items;
    };

    var fakeMediaService = {
        images: {
            get: function () { return itemsPromiseTransform(genericGet()) },
            getFolders: function () { return itemsPromiseTransform(genericGet()) },
            getPredecessorsFolders: function () { return itemsPromiseTransform(genericGet()) },
        }
    }
 
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
            for (var i = skip; i < take + skip && i < 10; i++) {
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

    // Basic filter
    it('[dzhenko] / basic filter: should properly set basic id of filter object', function () {
        var scope = rootScope.$new();
        commonMethods.compileDirective(directiveMarkup, scope);
        var s = scope.$$childHead;

        expect(s.filters.library.selected[0]).toBeUndefined();

        $('ul.sf-tree li span:contains("Title1")').first().click();
        scope.$digest();

        expect(s.filters.library.selected[0]).toEqual(1);
    });

    // Library filter
    it('[dzhenko] / library filter: should properly set parent id of filter object', function () {
        var scope = rootScope.$new();
        commonMethods.compileDirective(directiveMarkup, scope);
        var s = scope.$$childHead;

        expect(s.filters.library.selected[0]).toBeUndefined();

        $('ul.sf-tree li span:contains("Title1")').first().click();
        scope.$digest();

        expect(s.filters.library.selected[0]).toEqual(1);
    });

    // Tag filter
    it('[Boyko-Karadzhov] / tag filter: should set taxon id in the filter object when taxon is selected.', function () {
        var scope = rootScope.$new();

        var el = commonMethods.compileDirective(directiveMarkup, scope);
        var s = scope.$$childHead;

        scope.$digest();

        el.find('select[ng-model="selectedFilterOption"]').val(2).change();
        scope.$digest();

        $('span ul li:contains("Taxon 5")').click();
        scope.$digest();

        expect(s.filters.tag.selected).not.toBe(null);
        expect(s.filters.tag.selected[0]).not.toBeUndefined();
        expect(s.filters.tag.selected[0]).toEqual(5);
    });

    // Category filter
    it('[Boyko-Karadzhov] / category filter: should set taxon id in the filter object when taxon is selected.', function () {
        var scope = rootScope.$new();

        var el = commonMethods.compileDirective(directiveMarkup, scope);
        var s = scope.$$childHead;

        scope.$digest();

        el.find('select[ng-model="selectedFilterOption"]').val(3).change();
        scope.$digest();

        $('span ul li span:contains("Taxon 5")').click();
        scope.$digest();
        
        expect(s.filters.category.selected).not.toBe(null);
        expect(s.filters.category.selected[0]).not.toBeUndefined();
        expect(s.filters.category.selected[0]).toEqual(5);
    });

    // Date Filter
    (function () {
        describe('verifies media date filter options', function () {
            var dateScope;
            var dateS;

            beforeEach(function () {
                dateScope = rootScope.$new();
                var el = commonMethods.compileDirective(directiveMarkup, dateScope);
                dateS = dateScope.$$childHead;

                el.find('select[ng-model="selectedFilterOption"]').val(4).change();
                dateScope.$digest();
            });

            var assertFilter = function (expectedDate) {
                var filterDate = dateS.filters.date.selected[0];

                expect(filterDate).not.toBe(null);
                expect(filterDate.getFullYear()).toEqual(expectedDate.getFullYear());
                expect(filterDate.getMonth()).toEqual(expectedDate.getMonth());
                expect(filterDate.getDate()).toEqual(expectedDate.getDate());
            };

            it('[NPetrova] / should set AnyTime for date in the filter object when Any time is selected.', function () {
                $('span ul li:contains("Any time")').click();
                dateScope.$digest();

                expect(dateS.filters.date.selected).not.toBe(null);
                expect(dateS.filters.date.selected[0]).toBe('AnyTime');
            });

            it('[NPetrova] / should set correct date in the filter object when Last 1 day is selected.', function () {
                $('span ul li:contains("Last 1 day")').click();
                dateScope.$digest();

                var expectedDate = new Date();
                expectedDate.setDate(expectedDate.getDate() - 1);

                assertFilter(expectedDate);
            });

            it('[NPetrova] / should set correct date in the filter object when Last 3 days is selected.', function () {
                $('span ul li:contains("Last 3 days")').click();
                dateScope.$digest();

                var expectedDate = new Date();
                expectedDate.setDate(expectedDate.getDate() - 3);

                assertFilter(expectedDate);
            });

            it('[NPetrova] / should set correct date in the filter object when Last 1 week is selected.', function () {
                $('span ul li:contains("Last 1 week")').click();
                dateScope.$digest();

                var expectedDate = new Date();
                expectedDate.setDate(expectedDate.getDate() - 7);

                assertFilter(expectedDate);
            });

            it('[NPetrova] / should set correct date in the filter object when Last 1 month is selected.', function () {
                $('span ul li:contains("Last 1 month")').click();
                dateScope.$digest();

                var expectedDate = new Date();
                expectedDate.setMonth(expectedDate.getMonth() - 1);

                assertFilter(expectedDate);
            });

            it('[NPetrova] / should set correct date in the filter object when Last 6 months is selected.', function () {
                $('span ul li:contains("Last 6 months")').click();
                dateScope.$digest();

                var expectedDate = new Date();
                expectedDate.setMonth(expectedDate.getMonth() - 6);

                assertFilter(expectedDate);
            });

            it('[NPetrova] / should set correct date in the filter object when Last 1 year is selected.', function () {
                $('span ul li:contains("Last 1 year")').click();
                dateScope.$digest();

                var expectedDate = new Date();
                expectedDate.setYear(expectedDate.getFullYear() - 1);

                assertFilter(expectedDate);
            });

            it('[NPetrova] / should set correct date in the filter object when Last 2 years is selected.', function () {
                $('span ul li:contains("Last 2 years")').click();
                dateScope.$digest();

                var expectedDate = new Date();
                expectedDate.setYear(expectedDate.getFullYear() - 2);

                assertFilter(expectedDate);
            });

            it('[NPetrova] / should set correct date in the filter object when Last 5 years is selected.', function () {
                $('span ul li:contains("Last 5 years")').click();
                dateScope.$digest();

                var expectedDate = new Date();
                expectedDate.setYear(expectedDate.getFullYear() - 5);

                assertFilter(expectedDate);
            });
        });
    }());
});