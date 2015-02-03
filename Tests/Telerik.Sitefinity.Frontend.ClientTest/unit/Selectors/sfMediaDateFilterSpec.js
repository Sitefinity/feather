describe('Media date filter', function () {
    var rootScope;
    var provide;
    var $q;
    var mediaService;
    var directiveMarkup = '<span sf-media-date-filter sf-model="filterObject"></span>';

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

    it('[NPetrova] / should set date in the filter object when Last 3 days is selected.', function () {
        var scope = rootScope.$new();
        scope.filterObject = mediaService.newFilter();
        scope.filterObject.basic = 'someValue';

        commonMethods.compileDirective(directiveMarkup, scope);
        scope.$digest();

        $('span ul li:contains("Last 3 days")').click();
        scope.$digest();

        var expectedDate = new Date();
        expectedDate.setDate(expectedDate.getDate() - 3);

        expect(scope.filterObject.date).not.toBe(null);
        expect(scope.filterObject.date.getYear()).toEqual(expectedDate.getYear());
        expect(scope.filterObject.date.getMonth()).toEqual(expectedDate.getMonth());
        expect(scope.filterObject.date.getDate()).toEqual(expectedDate.getDate());
        expect(scope.filterObject.basic).toBe(null);
    });

    it('[NPetrova] / should set date in the filter object when Last 6 months is selected.', function () {
        var scope = rootScope.$new();
        scope.filterObject = mediaService.newFilter();
        scope.filterObject.basic = 'someValue';

        commonMethods.compileDirective(directiveMarkup, scope);
        scope.$digest();

        $('span ul li:contains("Last 6 months")').click();
        scope.$digest();

        var expectedDate = new Date();
        expectedDate.setMonth(expectedDate.getMonth() - 6);

        expect(scope.filterObject.date).not.toBe(null);
        expect(scope.filterObject.date.getYear()).toEqual(expectedDate.getYear());
        expect(scope.filterObject.date.getMonth()).toEqual(expectedDate.getMonth());
        expect(scope.filterObject.date.getDate()).toEqual(expectedDate.getDate());
        expect(scope.filterObject.basic).toBe(null);
    });

    it('[NPetrova] / should set date in the filter object when Last 2 years is selected.', function () {
        var scope = rootScope.$new();
        scope.filterObject = mediaService.newFilter();
        scope.filterObject.basic = 'someValue';

        commonMethods.compileDirective(directiveMarkup, scope);
        scope.$digest();

        $('span ul li:contains("Last 2 years")').click();
        scope.$digest();

        var expectedDate = new Date();
        expectedDate.setYear(expectedDate.getYear() - 2);

        expect(scope.filterObject.date).not.toBe(null);
        expect(scope.filterObject.date.getYear()).toEqual(expectedDate.getYear());
        expect(scope.filterObject.date.getMonth()).toEqual(expectedDate.getMonth());
        expect(scope.filterObject.date.getDate()).toEqual(expectedDate.getDate());
        expect(scope.filterObject.basic).toBe(null);
    });

    it('[NPetrova] / should set AnyTime for date in the filter object when Any time is selected.', function () {
        var scope = rootScope.$new();
        scope.filterObject = mediaService.newFilter();
        scope.filterObject.basic = 'someValue';

        commonMethods.compileDirective(directiveMarkup, scope);
        scope.$digest();

        $('span ul li:contains("Any time")').click();
        scope.$digest();

        expect(scope.filterObject.date).not.toBe(null);
        expect(scope.filterObject.date).toBe("AnyTime");
        expect(scope.filterObject.basic).toBe(null);
    });

    it('[NPetrova] / should throw error if no ng-model is passed.', function () {
        var scope = rootScope.$new();
        scope.filterObject = { };

        expect(function () {
            commonMethods.compileDirective(directiveMarkup, scope);
        }).toThrow();
    });
});