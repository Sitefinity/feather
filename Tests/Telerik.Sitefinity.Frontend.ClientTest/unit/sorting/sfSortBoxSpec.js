describe('sort box directive', function () {
    beforeEach(module('sfSortBox'));
    beforeEach(module('templates'));

    var $rootScope;
    var $compile = null;
    var el = null;
    var changeSelectedOption;

    beforeEach(inject(function (_$rootScope_) {
        $rootScope = _$rootScope_;
    }));

    beforeEach(function () {
        commonMethods.mockServerContextToEnableTemplateCache();
    });
    
    beforeEach(inject(function ($injector) {
        changeSelectedOption = function () {

        }
    }));

    it('[dzhenko] / should have first sort value on default and not call callback function.', function () {
        var scope = $rootScope.$new();
        var setSortExpression = null;
        var called = false;

        scope.callback = function (sortExpression) {
            setSortExpression = sortExpression;
            called = true;
        };

        var directiveMarkup = '<div class="sort-box" sf-sort-box sf-action="callback"></div>';
        var el = commonMethods.compileDirective(directiveMarkup, scope);
        var s = scope.$$childHead;

        //var ctrl = el.find('select').controller('ngModel');
        //ctrl.$setViewValue('New-uploaded first');
        //ctrl.$render();

        //el.find('select').trigger('select');

        $('.sort-box').change();

        expect(s.sfModel).toEqual('DateCreated DESC');
        expect(setSortExpression).toEqual(null);
        expect(called).toEqual(false);
    });

    it('[dzhenko] / should have second sort value when chosen and trigger callback.', function () {
        var scope = $rootScope.$new();
        var setSortExpression = null;
        var called = false;

        scope.callback = function (sortExpression) {
            setSortExpression = sortExpression;
            called = true;
        };

        var directiveMarkup = '<div class="sort-box" sf-sort-box sf-action="callback"></div>';
        var el = commonMethods.compileDirective(directiveMarkup, scope);
        var s = scope.$$childHead;

        $('.sort-box select').val(s.sfSortOptions[1].value);

        //var ctrl = el.find('select').controller('ngModel');
        //ctrl.$setViewValue(s.sfSortOptions[1].value);
        //ctrl.$render();
        //s.$digest();

        expect(s.sfModel).toEqual('LastModified DESC');
        expect(setSortExpression).not.toEqual(null);
        expect(called).toEqual(true);
    });

    //it('[dzhenko] / should throw error if no callback is passed to sf-action.', function () {
    //    var scope = $rootScope.$new();

    //    var directiveMarkup = '<div sf-sort-box></div>';

    //    expect(function () {
    //        commonMethods.compileDirective(directiveMarkup, scope);
    //    }).toThrow();
    //});
});