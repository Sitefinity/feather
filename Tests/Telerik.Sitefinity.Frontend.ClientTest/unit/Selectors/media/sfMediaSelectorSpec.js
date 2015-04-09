describe('media selector', function () {
    var rootScope;
    var $q;
    var provide;
    var directiveMarkup = '<sf-media-selector sf-media-type="{{sfMediaType}}" sf-labels="sfLabels" modal></sf-image-selector>';

    beforeEach(module('templates'));
    beforeEach(module('sfMediaSelector'));

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

    var mediaType = 'images';
    var scope;
    beforeEach(function () {
        scope = rootScope.$new();
        scope.sfMediaType = mediaType;
        scope.sfLabels = {};
    });

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
    };

    var genericGet = function () {
        var items = [];
        for (var i = 0; i <= 5; i++) {
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
            get: function () { return itemsPromiseTransform(genericGet()); },
            getFolders: function () { return itemsPromiseTransform(genericGet()); },
            getPredecessorsFolders: function () { return itemsPromiseTransform(genericGet()); },
            getSettings: function () {
                var defer = $q.defer();
                defer.resolve({ AllowedExensionsRegex: /^image\/(png)$/g });
                return defer.promise;
            }
        },
        documents: this.images,
        videos: this.images
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
    describe('filters', function () {
        // Basic filter
        it('[dzhenko] / basic filter: should properly set basic id of filter object', function () {
            commonMethods.compileDirective(directiveMarkup, scope);
            var s = scope.$$childHead;

            expect(s.filters.library.selected[0]).toBeUndefined();

            $('ul.sf-Tree li span:contains("Title1")').first().click();
            scope.$digest();

            expect(s.filters.library.selected[0]).toEqual(1);
        });

        // Library filter
        it('[dzhenko] / library filter: should properly set parent id of filter object', function () {
            commonMethods.compileDirective(directiveMarkup, scope);
            var s = scope.$$childHead;

            expect(s.filters.library.selected[0]).toBeUndefined();

            $('ul.sf-Tree li span:contains("Title1")').first().click();
            scope.$digest();

            expect(s.filters.library.selected[0]).toEqual(1);
        });

        // Tag filter
        it('[Boyko-Karadzhov] / tag filter: should set taxon id in the filter object when taxon is selected.', function () {
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

        describe('verifies media date filter options', function () {
            var dateScope;
            var dateS;

            beforeEach(function () {
                dateScope = rootScope.$new();
                dateScope.sfMediaType = mediaType;
                dateScope.sfLabels = {};

                var el = commonMethods.compileDirective(directiveMarkup, dateScope);
                dateS = dateScope.$$childHead;

                el.find('select[ng-model="selectedFilterOption"]').val(4).change();
                dateScope.$digest();
            });

            var assertFilter = function (expectedDate) {
                var selectedKey = dateS.filters.date.selectedKey;
                var filterDate = dateS.filters.date.values[selectedKey];

                expect(filterDate).not.toBe(null);
                expect(filterDate.getFullYear()).toEqual(expectedDate.getFullYear());
                expect(filterDate.getMonth()).toEqual(expectedDate.getMonth());
                expect(filterDate.getDate()).toEqual(expectedDate.getDate());
            };

            it('[NPetrova] / should set AnyTime for date in the filter object when Any time is selected.', function () {
                $('span ul li:contains("AnyTime")').click();
                dateScope.$digest();

                expect(dateS.filters.date.selectedKey).not.toBe(null);
                expect(dateS.filters.date.selectedKey).toBe('AnyTime');
            });

            it('[NPetrova] / should set correct date in the filter object when Last 1 day is selected.', function () {
                $('span ul li:contains("Last1Day")').click();
                dateScope.$digest();

                var expectedDate = new Date();
                expectedDate.setDate(expectedDate.getDate() - 1);

                assertFilter(expectedDate);
            });

            it('[NPetrova] / should set correct date in the filter object when Last 3 days is selected.', function () {
                $('span ul li:contains("Last3Days")').click();
                dateScope.$digest();

                var expectedDate = new Date();
                expectedDate.setDate(expectedDate.getDate() - 3);

                assertFilter(expectedDate);
            });

            it('[NPetrova] / should set correct date in the filter object when Last 1 week is selected.', function () {
                $('span ul li:contains("Last1Week")').click();
                dateScope.$digest();

                var expectedDate = new Date();
                expectedDate.setDate(expectedDate.getDate() - 7);

                assertFilter(expectedDate);
            });

            it('[NPetrova] / should set correct date in the filter object when Last 1 month is selected.', function () {
                $('span ul li:contains("Last1Month")').click();
                dateScope.$digest();

                var expectedDate = new Date();
                expectedDate.setMonth(expectedDate.getMonth() - 1);

                assertFilter(expectedDate);
            });

            it('[NPetrova] / should set correct date in the filter object when Last 6 months is selected.', function () {
                $('span ul li:contains("Last6Months")').click();
                dateScope.$digest();

                var expectedDate = new Date();
                expectedDate.setMonth(expectedDate.getMonth() - 6);

                assertFilter(expectedDate);
            });

            it('[NPetrova] / should set correct date in the filter object when Last 1 year is selected.', function () {
                $('span ul li:contains("Last1Year")').click();
                dateScope.$digest();

                var expectedDate = new Date();
                expectedDate.setYear(expectedDate.getFullYear() - 1);

                assertFilter(expectedDate);
            });

            it('[NPetrova] / should set correct date in the filter object when Last 2 years is selected.', function () {
                $('span ul li:contains("Last2Years")').click();
                dateScope.$digest();

                var expectedDate = new Date();
                expectedDate.setYear(expectedDate.getFullYear() - 2);

                assertFilter(expectedDate);
            });

            it('[NPetrova] / should set correct date in the filter object when Last 5 years is selected.', function () {
                $('span ul li:contains("Last5Years")').click();
                dateScope.$digest();

                var expectedDate = new Date();
                expectedDate.setYear(expectedDate.getFullYear() - 5);

                assertFilter(expectedDate);
            });

            it('[Boyko-Karadzhov] / should set is-grid class by default.', function () {
                var element = commonMethods.compileDirective(directiveMarkup, scope);

                expect($(element.find('[sf-collection]')[0]).is('.is-grid')).toBe(true);
                expect($(element.find('[sf-collection]')[0]).is('.is-list')).toBe(false);
            });

            it('[Boyko-Karadzhov] / should set is-list class when switchToList is clicked.', function () {
                var element = commonMethods.compileDirective(directiveMarkup, scope);

                scope.$$childTail.switchToList();
                scope.$digest();

                expect($(element.find('[sf-collection]')[0]).is('.is-grid')).toBe(false);
                expect($(element.find('[sf-collection]')[0]).is('.is-list')).toBe(true);
            });
        });
    });

    /*
    * Upload
    */

    // Drag drop prepopulating
    describe('upload - drag and drop prepopulating', function () {
        var suppressedDataTransferDroppedFunc = function (s) {
            try {
                s.dataTransferDropped({ files: [{ type: 'image/png' }] });
                s.$digest();
            } catch (e) {
                // no actual angular bootstrap window to open - suppres warning
            }
        };

        it('[dzhenko] / should populate library property on uploaded image when dropped in library filter mode.', function () {
            var el = commonMethods.compileDirective(directiveMarkup, scope);
            var s = scope.$$childHead;
            s.breadcrumbs = [{ Id: 1 }, { Id: 2 }, { Id: 3 }, { Id: 4 }, { Id: 5 }];

            suppressedDataTransferDroppedFunc(s);

            expect(s.model.parentId).toEqual(5);
        });

        it('[dzhenko] / should populate tag property on uploaded image when dropped in tag filter mode.', function () {
            var el = commonMethods.compileDirective(directiveMarkup, scope);
            var s = scope.$$childHead;

            s.filters.tag.selected = ['tag1'];
            s.selectedFilterOption = 2;

            suppressedDataTransferDroppedFunc(s);

            expect(s.model.tags[0]).toEqual('tag1');
        });

        it('[dzhenko] / should populate category property on uploaded image when dropped in category filter mode.', function () {
            var el = commonMethods.compileDirective(directiveMarkup, scope);
            var s = scope.$$childHead;

            s.filters.category.selected = ['cat1'];
            s.selectedFilterOption = 3;

            suppressedDataTransferDroppedFunc(s);

            expect(s.model.categories[0]).toEqual('cat1');
        });

        it('[dzhenko] / should populate only library property on uploaded image when dropped in library filter mode and previously dropped on other fields.', function () {
            var el = commonMethods.compileDirective(directiveMarkup, scope);
            var s = scope.$$childHead;

            s.breadcrumbs = [{ Id: 1 }, { Id: 2 }, { Id: 3 }, { Id: 4 }, { Id: 5 }];
            s.filters.tag.selected = ['tag1'];
            s.filters.category.selected = ['cat1'];

            s.selectedFilterOption = 1;
            suppressedDataTransferDroppedFunc(s);
            s.selectedFilterOption = 2;
            suppressedDataTransferDroppedFunc(s);
            s.selectedFilterOption = 3;
            suppressedDataTransferDroppedFunc(s);
            s.selectedFilterOption = 1;
            suppressedDataTransferDroppedFunc(s);

            expect(s.model.parentId).toEqual(5);

            expect(s.model.categories).toEqual([]);
            expect(s.model.tags).toEqual([]);
        });
    });
});
