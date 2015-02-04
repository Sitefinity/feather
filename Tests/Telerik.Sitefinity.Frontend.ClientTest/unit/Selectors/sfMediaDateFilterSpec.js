describe('Media date filter', function () {
    var rootScope;
    var provide;
    var $q;
    var mediaService;
    var directiveMarkup = '<span sf-media-date-filter sf-show-any-time sf-model="filterObject"></span>';
    var scope;

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));
    beforeEach(module('sfMediaDateFilter'));

    beforeEach(inject(function (_$rootScope_, _$q_, sfMediaService) {
        rootScope = _$rootScope_;
        $q = _$q_;
        mediaService = sfMediaService;
    }));

    beforeEach(function () {
        commonMethods.mockServerContextToEnableTemplateCache();
    });

    describe('verifies media date filter options', function () {
        beforeEach(function () {
            scope = rootScope.$new();
            scope.filterObject = mediaService.newFilter();
            scope.filterObject.basic = 'someValue';
        });

        var assertFilter = function (filter, expectedDate) {
            expect(filter.date).not.toBe(null);
            expect(filter.date.getYear()).toEqual(expectedDate.getYear());
            expect(filter.date.getMonth()).toEqual(expectedDate.getMonth());
            expect(filter.date.getDate()).toEqual(expectedDate.getDate());
            expect(filter.basic).toBe(null);
        };

        it('[NPetrova] / should set AnyTime for date in the filter object when Any time is selected.', function () {
            commonMethods.compileDirective(directiveMarkup, scope);
            scope.$digest();

            $('span ul li:contains("Any time")').click();
            scope.$digest();

            expect(scope.filterObject.date).not.toBe(null);
            expect(scope.filterObject.date).toBe("AnyTime");
            expect(scope.filterObject.basic).toBe(null);
        });

        it('[NPetrova] / should set correct date in the filter object when Last 1 day is selected.', function () {
            commonMethods.compileDirective(directiveMarkup, scope);
            scope.$digest();

            $('span ul li:contains("Last 1 day")').click();
            scope.$digest();

            var expectedDate = new Date();
            expectedDate.setDate(expectedDate.getDate() - 1);

            assertFilter(scope.filterObject, expectedDate);
        });

        it('[NPetrova] / should set correct date in the filter object when Last 3 days is selected.', function () {
            commonMethods.compileDirective(directiveMarkup, scope);
            scope.$digest();

            $('span ul li:contains("Last 3 days")').click();
            scope.$digest();

            var expectedDate = new Date();
            expectedDate.setDate(expectedDate.getDate() - 3);

            assertFilter(scope.filterObject, expectedDate);
        });

        it('[NPetrova] / should set correct date in the filter object when Last 1 week is selected.', function () {
            commonMethods.compileDirective(directiveMarkup, scope);
            scope.$digest();

            $('span ul li:contains("Last 1 week")').click();
            scope.$digest();

            var expectedDate = new Date();
            expectedDate.setDate(expectedDate.getDate() - 7);

            assertFilter(scope.filterObject, expectedDate);
        });

        it('[NPetrova] / should set correct date in the filter object when Last 1 month is selected.', function () {
            commonMethods.compileDirective(directiveMarkup, scope);
            scope.$digest();

            $('span ul li:contains("Last 1 month")').click();
            scope.$digest();

            var expectedDate = new Date();
            expectedDate.setMonth(expectedDate.getMonth() - 1);

            assertFilter(scope.filterObject, expectedDate);
        });

        it('[NPetrova] / should set correct date in the filter object when Last 6 months is selected.', function () {
            commonMethods.compileDirective(directiveMarkup, scope);
            scope.$digest();

            $('span ul li:contains("Last 6 months")').click();
            scope.$digest();

            var expectedDate = new Date();
            expectedDate.setMonth(expectedDate.getMonth() - 6);

            assertFilter(scope.filterObject, expectedDate);
        });

        it('[NPetrova] / should set correct date in the filter object when Last 1 year is selected.', function () {
            commonMethods.compileDirective(directiveMarkup, scope);
            scope.$digest();

            $('span ul li:contains("Last 1 year")').click();
            scope.$digest();

            var expectedDate = new Date();
            expectedDate.setYear(expectedDate.getYear() - 1);

            assertFilter(scope.filterObject, expectedDate);
        });

        it('[NPetrova] / should set correct date in the filter object when Last 2 years is selected.', function () {
            commonMethods.compileDirective(directiveMarkup, scope);
            scope.$digest();

            $('span ul li:contains("Last 2 years")').click();
            scope.$digest();

            var expectedDate = new Date();
            expectedDate.setYear(expectedDate.getYear() - 2);

            assertFilter(scope.filterObject, expectedDate);
        });

        it('[NPetrova] / should set correct date in the filter object when Last 5 years is selected.', function () {
            commonMethods.compileDirective(directiveMarkup, scope);
            scope.$digest();

            $('span ul li:contains("Last 5 years")').click();
            scope.$digest();

            var expectedDate = new Date();
            expectedDate.setYear(expectedDate.getYear() - 5);

            assertFilter(scope.filterObject, expectedDate);
        });

        it('[dzhenko] / should not have any time option if such is not provided in the directive markup.', function () {
            var dirMarkup = '<span sf-media-date-filter sf-model="filterObject"></span>';
            commonMethods.compileDirective(dirMarkup, scope);
            scope.$digest();

            expect($('span ul li:contains("Any Time")').length).toEqual(0);
        });

        it('[dzhenko] / should use custom dates if such are provided.', function () {
            var date1 = new Date(1998, 11);
            var date2 = new Date(1998, 9);
            scope.datesToUse = [{ text: 'Custom Date 1', dateValue: date1 },
                                { text: 'Custom Date 2', dateValue: date2 }, ];

            var dirMarkup = '<span sf-dates="datesToUse" sf-media-date-filter sf-model="filterObject"></span>';
            commonMethods.compileDirective(dirMarkup, scope);
            scope.$digest();

            expect($('span ul li:contains("Custom Date 1")').length).toEqual(1);
            expect($('span ul li:contains("Custom Date 2")').length).toEqual(1);
        });
    });

    describe('verifies error message', function () {
        it('[NPetrova] / should throw error if no ng-model is passed.', function () {
            var scope = rootScope.$new();
            scope.filterObject = { };

            expect(function () {
                commonMethods.compileDirective(directiveMarkup, scope);
            }).toThrow();
        });
    });
});