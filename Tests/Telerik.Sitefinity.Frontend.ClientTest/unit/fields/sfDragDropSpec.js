describe('Drag Drop', function () {
    var rootScope;

    var fakeDataTransferObject = { files: ['fakeFile'] };
    var fakeEvent = $.Event('drop');
    fakeEvent.originalEvent = { dataTransfer: fakeDataTransferObject };

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));
    beforeEach(module('sfSelectors'));
    beforeEach(module('sfDragDrop'));

    beforeEach(inject(function (_$rootScope_) {
        rootScope = _$rootScope_;
    }));

    beforeEach(function () {
        commonMethods.mockServerContextToEnableTemplateCache();
    });

    afterEach(function () {
        //Tear down.
        var leftOver = $('.testDiv');
        leftOver.empty();
        leftOver.remove();
    });

    it('[dzhenko] / should trigger callback on item dropped.', function () {
        var directiveMarkup = '<div sf-drag-drop sf-data-transfer-callback="dataTransferDropped(dataTransferObject)"></div>';
        var scope = rootScope.$new();

        var called = false;
        var passedObj = null;

        scope.dataTransferDropped = function (dataTransferObject) {
            called = true;
            passedObj = dataTransferObject;
        };

        var element = commonMethods.compileDirective(directiveMarkup, scope);
        var s = scope.$$childHead;

        expect($('.sf-Drag').length).toBe(1);

        $('.sf-Drag').trigger(fakeEvent, fakeDataTransferObject);

        s.$digest();
        scope.$digest();

        expect(called).toBe(true);
        expect(passedObj).toEqual(fakeDataTransferObject);
    });
});