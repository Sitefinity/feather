describe('tree directive', function () {
    beforeEach('sfTree');

    var rootScope;
    var templateCache;

    beforeEach(inject(function (_$rootScope_, $templateCache) {
        rootScope = _$rootScope_;
        templateCache = $templateCache;
    }));

    it('[Boyko-Karadzhov] / should request items initially.', function () {
        var scope = rootScope.$new();
        var childrenRequested = false;

        templateCache.put('/Frontend-Assembly/Telerik.Sitefinity.Frontend/sf-tree/dummy.html', '');

        scope.requestChildren = function () {
            childrenRequested = true;
        };
        
        var directiveMarkup = '<div sf-tree sf-template-url="sf-tree/dummy.html" sf-request-children="requestChildren(parent)"></div>';
        commonMethods.compileDirective(directiveMarkup, scope);

        expect(childrenRequested).toBe(true);
    });
});