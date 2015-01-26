describe('collection directive', function () {
    beforeEach(module('sfSelectors'));

    beforeEach(inject(function (_$rootScope_) {
        $rootScope = _$rootScope_;
    }));

    it('[Boyko-Karadzhov] / should render all items.', function () {
        var scope = $rootScope.$new();
        scope.template = '<li ng-repeat="item in items" class="sfCollectionItem"><div><span>Id:</span><span>{{item.id}}</span></div><div>{{item.Title}}</div></li>';
        scope.dataItems = [];
        for (var i = 1; i <= 5; i++) {
            scope.dataItems.push({
                id:  i,
                title: 'Item ' + i
            });
        }

        var directiveMarkup = '<sf-collection sf-template="template" sf-data="dataItems"></sf-collection>';
        commonMethods.compileDirective(directiveMarkup, scope);

        expect($('.sfCollectionItem').length).toEqual(5);
    });
});