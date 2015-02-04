describe('sort box directive', function () {
    beforeEach(module('sfSortBox'));
    beforeEach(module('templates'));

    var $rootScope;
    var $compile = null;
    var el = null;

    var changeSelectedOption = function (elm, value) {
        elm.val(value);
        browserTrigger(elm, 'change');
    };

    beforeEach(inject(function (_$rootScope_) {
        $rootScope = _$rootScope_;
    }));

    beforeEach(function () {
        commonMethods.mockServerContextToEnableTemplateCache();
    });

    var generictTestForValue = function (sortOptionIndex, expectedValue) {
        var scope = $rootScope.$new();
        var directiveMarkup = '<div class="sort-box" sf-sort-box sf-action="callback"></div>';
        commonMethods.compileDirective(directiveMarkup, scope);
        var s = scope.$$childHead;

        if (sortOptionIndex) {
            s.sfModel = s.sfSortOptions[sortOptionIndex].value;
        }

        expect(s.sfModel).toEqual(expectedValue);
    };

    it('[dzhenko] / should have first sort value on default.', function () {
        generictTestForValue(null, 'DateCreated DESC');
    });

    it('[dzhenko] / should have proper second sort value.', function () {
        generictTestForValue(1, 'LastModified DESC');
    });

    it('[dzhenko] / should have proper third sort value.', function () {
        generictTestForValue(2, 'Title ASC');
    });

    it('[dzhenko] / should have proper third sort value.', function () {
        generictTestForValue(3, 'Title DESC');
    });
});