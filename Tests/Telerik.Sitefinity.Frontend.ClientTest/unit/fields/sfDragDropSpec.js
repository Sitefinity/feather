describe('Drag Drop', function () {
    var rootScope,
        directiveMarkup = '<div sf-drag-drop sf-data-transfer-callback="dataTransferDropped(dataTransferObject)"></div>';

    var fakeDataTransferObject = { files: ['fakeFile'] };
    var fakeEvent = $.Event("drop");
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
        var scope = rootScope.$new();
        var called = false;
        var passedObj = null;

        scope.dataTransferDropped = function (dataTransferObject) {
            called = true;
            passedObj = dataTransferObject;
        };

        var element = commonMethods.compileDirective(directiveMarkup, scope);
        scope.$digest();

        element.trigger(fakeEvent, fakeDataTransferObject);
        scope.$digest();

        expect(called).toBe(true);
        expect(passedObj).toEqual(fakeDataTransferObject);
    });
});