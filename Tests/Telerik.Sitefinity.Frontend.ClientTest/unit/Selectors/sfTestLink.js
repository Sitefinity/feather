describe('sfLinkTest directive', function () {
    /* Variables */
    var $rootScope;

    //Load the module under test.
    beforeEach(module('sfSelectors'));

    var windowMock = {
        open: jasmine.createSpy('$window.open')
    };

    beforeEach(module(function ($provide) {
        //Force angular to use the mock.
        $provide.value('$window', windowMock);
    }));

    beforeEach(inject(function (_$rootScope_){
        $rootScope = _$rootScope_;
    }));

    afterEach(function () {
        //Tear down.
        var leftOver = $('.testDiv');
        leftOver.empty();
        leftOver.remove();
    });

    it('[GeorgiMateev] / should open new window with web address.', function () {
        var scope = $rootScope.$new(); 
        scope.webAddress = 'http://asd.com';

        var template = '<input type="button" id="btn" sf-test-link sf-test-value="webAddress"/>';

        commonMethods.compileDirective(template, scope);

        $('#btn').click();

        var mostRecent = windowMock.open.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();
        expect(mostRecent.args[0]).toBe('http://asd.com');;
    });

    it('[GeorgiMateev] / should open new window with mail address.', function () {
        var scope = $rootScope.$new(); 
        scope.mailAddress = 'asd@asd.com';

        var template = '<input type="button" id="btn" sf-test-link sf-test-value="mailAddress" sf-test-type="mail"/>';

        commonMethods.compileDirective(template, scope);

        $('#btn').click();

        var mostRecent = windowMock.open.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();
        expect(mostRecent.args[0]).toBe('mailto:asd@asd.com');;
    });
});