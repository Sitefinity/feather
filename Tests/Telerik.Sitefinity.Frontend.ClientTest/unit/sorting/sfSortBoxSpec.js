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
        scope.sortValue = null;
        var directiveMarkup = '<div sf-sort-box sf-model="sortValue"></div>';
        commonMethods.compileDirective(directiveMarkup, scope);
        var s = scope.$$childHead;

        if (sortOptionIndex) {
            s.sfModel = s.sfSortOptions[sortOptionIndex].value;
        }

        scope.$digest();

        expect(scope.sortValue).toEqual(expectedValue);
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

    it('[dzhenko] / should have proper fourth sort value.', function () {
        generictTestForValue(3, 'Title DESC');
    });

    it('[dzhenko] / should have proper default value if provided custom sort items.', function () {
        var scope = $rootScope.$new();
        scope.sortValue = null;
        scope.sortItems = [
                    {
                        title: 'New Title 1',
                        value: 'New Value 1'
                    }, {
                        title: 'New Title 2',
                        value: 'New Value 2'
                    }];

        var directiveMarkup = '<div sf-sort-box sf-model="sortValue" sf-sort-options="sortItems"></div>';
        commonMethods.compileDirective(directiveMarkup, scope);
        scope.$digest();

        expect(scope.sortValue).toEqual('New Value 1');
    });
});