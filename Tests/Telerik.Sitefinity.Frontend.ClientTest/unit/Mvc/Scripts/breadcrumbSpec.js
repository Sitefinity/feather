/* tests for the BreadCrumb */
describe('BreadCrumb directive test', function() {
    beforeEach(module('breadcrumb'));

    describe('Directives tests', function () {
        var element;
        var scope;

        beforeEach(inject(function ($rootScope, $compile) {
            element = angular.element('<ul ng-model="propertyPath" breadcrumb></ul>');
            var compile = $compile(element);
            compile($rootScope);
            $rootScope.RefreshHierarchy("Settings/Level0/Level1/Level2", "Level2");
            $rootScope.$digest();
            scope = $rootScope;
        }));

        it('[EGaneva] / Can refreshes the whole breadcrumb content.', inject(function () {
            expect(element.text()).not.toContain('Settings');
            expect(element.text()).toContain('Level0');
            expect(element.text()).toContain('Level1');
            expect(element.text()).toContain('Level2');
        }));

        it('[EGaneva] / Adds single element to the breadcrumb when proeprtyPath has been changed', inject(function () {
            scope.propertyPath = "Level0/Level1/Level2/Level3";
            scope.$digest();
            expect(element.text()).not.toContain('Settings');
            expect(element.text()).toContain('Level0');
            expect(element.text()).toContain('Level1');
            expect(element.text()).toContain('Level2');
            expect(element.text()).toContain('Level3');
        }));
    });
});