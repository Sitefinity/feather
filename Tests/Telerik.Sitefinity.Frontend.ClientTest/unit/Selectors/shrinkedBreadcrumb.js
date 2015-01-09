describe('shrinked breadcrumb directive', function  () {
    /* Variables */
    var $rootScope;

    //Load the module under test.
    beforeEach(module('sfSelectors'));

    beforeEach(inject(function (_$rootScope_){
    	$rootScope = _$rootScope_;
    }));

    afterEach(function () {
        //Tear down.
        var leftOver = $('.testDiv');
        leftOver.empty();
        leftOver.remove();
    });

    it('[GeorgiMateev] / should drop parts of the breadcrumb if the max length is exceeded.', function  () {
    	var scope = $rootScope.$new();    	
    	scope.text = 'home > page1 > page2 > page3 > page4 > page5 > page6';

    	var template = '<span id="testSpan" sf-shrinked-breadcrumb="{{text}}" sf-max-length="45"></span>';

    	commonMethods.compileDirective(template, scope);

    	var expectedResult = 'home > ... > page3 > page4 > page5 > page6';

    	var actualResult = $('#testSpan').text();

    	expect(actualResult).toBe(expectedResult);
    });

    it('[GeorgiMateev] / should leave only the last very long part and set skip symbol in the beginning and in the end.', function  () {
    	var scope = $rootScope.$new();    	
    	scope.text = 'home > page1 > page2 > page3 > page4 > page5 > this is sooo loooooooooooooooooooooooooooooooooooooooong';

    	var template = '<span id="testSpan" sf-shrinked-breadcrumb="{{text}}" sf-max-length="45"></span>';

    	commonMethods.compileDirective(template, scope);

    	var expectedResult = '... > this is sooo looooooooooooooooooooooooo...';

    	var actualResult = $('#testSpan').text();

    	expect(actualResult).toBe(expectedResult);
    });

    it('[GeorgiMateev] / should not shrink if the max length is not exceeded.', function  () {
    	var scope = $rootScope.$new();    	
    	scope.text = 'home > page1 > page2 > page3 > page4 > page5';

    	var template = '<span id="testSpan" sf-shrinked-breadcrumb="{{text}}" sf-max-length="45"></span>';

    	commonMethods.compileDirective(template, scope);

    	var expectedResult = 'home > page1 > page2 > page3 > page4 > page5';

    	var actualResult = $('#testSpan').text();

    	expect(actualResult).toBe(expectedResult);
    });

	it('[GeorgiMateev] / should trim the end if there is only root part and the max leng is exceeded.', function  () {
    	var scope = $rootScope.$new();    	
    	scope.text = 'home so loooooooooooooooooooooooooooooooooooooooong';

    	var template = '<span id="testSpan" sf-shrinked-breadcrumb="{{text}}" sf-max-length="45"></span>';

    	commonMethods.compileDirective(template, scope);

    	var expectedResult = 'home so loooooooooooooooooooooooooooooooooooo...';

    	var actualResult = $('#testSpan').text();

    	expect(actualResult).toBe(expectedResult);
    });

    it('[GeorgiMateev] / should not shrink if there is only root part and the max leng is not exceeded.', function  () {
    	var scope = $rootScope.$new();    	
    	scope.text = 'home';

    	var template = '<span id="testSpan" sf-shrinked-breadcrumb="{{text}}" sf-max-length="45"></span>';

    	commonMethods.compileDirective(template, scope);

    	var expectedResult = 'home';

    	var actualResult = $('#testSpan').text();

    	expect(actualResult).toBe(expectedResult);
    });

    it('[GeorgiMateev] / should shrink the root part if it is too long.', function  () {
    	var scope = $rootScope.$new();    	
    	scope.text = 'home so loooooooooooooooooooooooooooooooooooooooong > page';

    	var template = '<span id="testSpan" sf-shrinked-breadcrumb="{{text}}" sf-max-length="45"></span>';

    	commonMethods.compileDirective(template, scope);

    	var expectedResult = '... > page';

    	var actualResult = $('#testSpan').text();

    	expect(actualResult).toBe(expectedResult);
    });

    it('[GeorgiMateev] / should shrink the middle part if it is too long.', function  () {
    	var scope = $rootScope.$new();    	
    	scope.text = 'home > home so loooooooooooooooooooooooooooooooooooooooong > page';

    	var template = '<span id="testSpan" sf-shrinked-breadcrumb="{{text}}" sf-max-length="45"></span>';

    	commonMethods.compileDirective(template, scope);

    	var expectedResult = 'home > ... > page';

    	var actualResult = $('#testSpan').text();

    	expect(actualResult).toBe(expectedResult);
    });

    it('[GeorgiMateev] / should set empty string.', function  () {
    	var scope = $rootScope.$new();    	
    	scope.text = '';

    	var template = '<span id="testSpan" sf-shrinked-breadcrumb="{{text}}" sf-max-length="45"></span>';

    	commonMethods.compileDirective(template, scope);

    	var expectedResult = '';

    	var actualResult = $('#testSpan').text();

    	expect(actualResult).toBe(expectedResult);
    });
});