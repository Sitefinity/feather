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

        for (var i = 1; i <= 5; i++)
            expect($('.sfCollectionItem span:contains(' + i + ')').length).toEqual(1);
    });

    it('[Boyko-Karadzhov] / should mark the selected items with sf-selected class.', function () {
        var scope = $rootScope.$new();
        scope.template = '<li ng-repeat="item in items" sf-identifier="id" class="sfCollectionItem"><div><span>Id:</span><span>{{item.id}}</span></div><div>{{item.Title}}</div></li>';
        scope.dataItems = [];
        for (var i = 1; i <= 5; i++) {
            scope.dataItems.push({
                id: i,
                title: 'Item ' + i
            });
        }

        scope.selectedItems = [2, 4];

        var directiveMarkup = '<sf-collection sf-template="template" sf-data="dataItems" ng-model="selectedItems"></sf-collection>';
        commonMethods.compileDirective(directiveMarkup, scope);
        expect($('.sfCollectionItem.sf-selected').length).toEqual(2);
        expect($('.sfCollectionItem.sf-selected span:contains(2)').length).toEqual(1);
        expect($('.sfCollectionItem.sf-selected span:contains(4)').length).toEqual(1);
    });
});