/* Tests for timeSpan selector */
describe("timeSpan selector", function () {
    var scope;

    var serviceResult;
    var $q;
    var $timeout;

    var timeSpanItem = function () {
        this.periodType = 'anyTime';
        this.fromDate = null;
        this.toDate = null;
        this.timeSpanValue = null;
        this.timeSpanInterval = 'days';
        this.displayText = '';
    };

    //This is the id of the cached templates in $templateCache. The external templates are cached by a karma/grunt preprocessor.
    var timespanSelectorTemplatePath = 'client-components/selectors/date-time/sf-timespan-selector.sf-cshtml';

    //Load the module responsible for the modal dialog
    beforeEach(module('modalDialog'));

    //Load themodule under test.
    beforeEach(module('sfSelectors'));

    //Load the module that contains the cached tempaltes.
    beforeEach(module('templates'));

    beforeEach(inject(function ($rootScope, $httpBackend, _$q_, $templateCache, _$timeout_) {
        //Build the scope with whom the directive will be compiled.
        scope = $rootScope.$new();
        scope.selectedItem = new timeSpanItem();
        $q = _$q_;
        $timeout = _$timeout_;

        serviceResult = _$q_.defer();
    }));

    beforeEach(inject(function ($templateCache) {
        //This method is called by the templateUrl property of the directive's definition object.
        spyOn(sitefinity, 'getEmbeddedResourceUrl').andCallFake(function (assembly, url) {
            return timespanSelectorTemplatePath;
        });
    }));

    afterEach(function () {
        //Tear down.
        var leftOver = $('.testDiv, .modal, .modal-backdrop');
        leftOver.empty();
        leftOver.remove();
    });
    
    it('[EGaneva] / should format the timespan item for a period.', function () {
        scope.selectedItem.periodType = 'periodToNow';
        scope.selectedItem.timeSpanValue = 2;
        scope.selectedItem.timeSpanInterval = 'months';
        var template = "<sf-timespan-selector sf-selected-item='selectedItem'></sf-timespan-selector>";

        commonMethods.compileDirective(template, scope);

        expect(scope.selectedItem.displayText.indexOf(" 2 months")).toBeGreaterThan(-1);
    });

    it('[EGaneva] / should format the timespan item with custom range.', function () {
        scope.selectedItem.periodType = 'customRange';
        scope.selectedItem.fromDate = new Date("12/12/2012");
        scope.selectedItem.toDate = new Date("12/14/2012");
        var template = "<sf-timespan-selector sf-selected-item='selectedItem'></sf-timespan-selector>";

        commonMethods.compileDirective(template, scope);
    });

    it('[EGaneva] / custom range is validated correctly.', function () {
        scope.change = jasmine.createSpy('change');

        var template = "<sf-timespan-selector sf-change='change' sf-selected-item='selectedItem'></sf-timespan-selector>";
        commonMethods.compileDirective(template, scope);

        $('.openSelectorBtn').click();

        //The scope of the selector is isolated, but it's child of the scope used for compilation.
        var s = scope.$$childHead;

        //mock the call to the modal service.
        s.$modalInstance = { close: function () { } };

        s.selectedItemInTheDialog.periodType = 'customRange';
        s.selectedItemInTheDialog.fromDate = new Date("12/17/2012");
        s.selectedItemInTheDialog.toDate = new Date("12/14/2012");

        //Close the dialog (Done button clicked)
        s.selectItem();

        expect(scope.change).not.toHaveBeenCalled();
        expect(s.showError).toBeTruthy();
    });

    it('[EGaneva] / should fire "change" event with correct arguments.', function () {
        scope.change = function (args) {
            expect(args.newSelectedItem).toBeDefined();
            expect(args.newSelectedItem.displayText.indexOf(' 3 weeks')).toBeGreaterThan(-1);
            expect(args.newSelectedItem.periodType).toBe('periodToNow');
            expect(args.newSelectedItem.timeSpanValue).toBe(3);
            expect(args.newSelectedItem.timeSpanInterval).toBe("weeks");

            expect(args.oldSelectedItem).toBeDefined();
            expect(args.oldSelectedItem.displayText).toBe('');
            expect(args.oldSelectedItem.periodType).toBe('anyTime');
            expect(args.oldSelectedItem.timeSpanValue).toBeFalsy();
            expect(args.oldSelectedItem.timeSpanInterval).toBe("days");
        };

        var template = "<sf-timespan-selector sf-change='change' sf-selected-item='selectedItem'></sf-timespan-selector>";

        commonMethods.compileDirective(template, scope);
        $('.openSelectorBtn').click();

        //The scope of the selector is isolated, but it's child of the scope used for compilation.
        var s = scope.$$childHead;

        //mock the call to the modal service.
        s.$modalInstance = { close: function () { } };   

        s.selectedItemInTheDialog.periodType = 'periodToNow';
        s.selectedItemInTheDialog.timeSpanValue = 3;
        s.selectedItemInTheDialog.timeSpanInterval = 'weeks';

        //Close the dialog (Done button clicked)
        s.selectItem();

        expect(s.sfSelectedItem.displayText.indexOf(' 3 weeks')).toBeGreaterThan(-1);
    });   
});