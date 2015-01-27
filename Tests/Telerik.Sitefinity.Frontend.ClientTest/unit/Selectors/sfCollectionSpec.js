describe('collection directive', function () {
    var rootScope;
    var templateCache;

    beforeEach(module('sfSelectors'));

    beforeEach(inject(function (_$rootScope_, $templateCache) {
        rootScope = _$rootScope_;
        templateCache = $templateCache;
    }));

    it('[Boyko-Karadzhov] / should render all items.', function () {
        var scope = rootScope.$new();
        templateCache.put('/Frontend-Assembly/Telerik.Sitefinity.Frontend/sf-collection/reder-all-items.html', '<li ng-repeat="item in items" class="sfCollectionItem"><div><span>Id:</span><span ng-bind="item.id"></span></div><div ng-bind="item.title"></div></li>');

        scope.dataItems = [];
        for (var i = 1; i <= 5; i++) {
            scope.dataItems.push({
                id:  i,
                title: 'Item ' + i
            });
        }

        var directiveMarkup = '<ul sf-collection sf-template-url="sf-collection/reder-all-items.html" sf-data="dataItems"></ul>';
        commonMethods.compileDirective(directiveMarkup, scope);

        expect($('.sfCollectionItem').length).toEqual(5);

        for (var i = 1; i <= 5; i++)
            expect($('.sfCollectionItem span:contains(' + i + ')').length).toEqual(1);
    });

    it('[Boyko-Karadzhov] / should recognize selected items using the isSelected method.', function () {
        var scope = rootScope.$new();
        templateCache.put('/Frontend-Assembly/Telerik.Sitefinity.Frontend/sf-collection/marks-selected-items.html', '<li ng-repeat="item in items" class="sfCollectionItem" ng-class="{\'sf-selected\': isSelected(item)}"><div><span>Id:</span><span>{{item.id}}</span></div><div>{{item.Title}}</div></li>');

        scope.dataItems = [];
        for (var i = 1; i <= 5; i++) {
            scope.dataItems.push({
                id: i,
                title: 'Item ' + i
            });
        }

        scope.selectedItems = [2, 4];

        var directiveMarkup = '<ul sf-collection sf-template-url="sf-collection/marks-selected-items.html" sf-data="dataItems" ng-model="selectedItems" sf-identifier="id"></ul>';
        commonMethods.compileDirective(directiveMarkup, scope);
        expect($('.sfCollectionItem.sf-selected').length).toEqual(2);
        expect($('.sfCollectionItem.sf-selected span:contains(2)').length).toEqual(1);
        expect($('.sfCollectionItem.sf-selected span:contains(4)').length).toEqual(1);
    });

    it('[Boyko-Karadzhov] / should have only one item selected when sf-multiselect is not present and a second item is clicked.', function () {
        var scope = rootScope.$new();
        templateCache.put('/Frontend-Assembly/Telerik.Sitefinity.Frontend/sf-collection/marks-single-select.html', '<li ng-repeat="item in items" class="sfCollectionItem" ng-click="select(item)"><div><span>Id:</span><span>{{item.id}}</span></div><div>{{item.Title}}</div></li>');

        scope.dataItems = [];
        for (var i = 1; i <= 5; i++) {
            scope.dataItems.push({
                id: i,
                title: 'Item ' + i
            });
        }
        
        scope.selectedItems = [2];
        var directiveMarkup = '<ul sf-collection sf-template-url="sf-collection/marks-single-select.html" sf-data="dataItems" ng-model="selectedItems" sf-identifier="id"></ul>';
        var element = commonMethods.compileDirective(directiveMarkup, scope);
        $(element).find('.sfCollectionItem').has('span:contains("4")').click();
        scope.$digest();

        expect(scope.selectedItems.length).toEqual(1);
        expect(scope.selectedItems[0]).toEqual(4);
    });

    it('[Boyko-Karadzhov] / should have both items selected when sf-multiselect is present and a second item is clicked.', function () {
        var scope = rootScope.$new();
        templateCache.put('/Frontend-Assembly/Telerik.Sitefinity.Frontend/sf-collection/marks-multi-select.html', '<li ng-repeat="item in items" class="sfCollectionItem" ng-click="select(item)"><div><span>Id:</span><span>{{item.id}}</span></div><div>{{item.Title}}</div></li>');

        scope.dataItems = [];
        for (var i = 1; i <= 5; i++) {
            scope.dataItems.push({
                id: i,
                title: 'Item ' + i
            });
        }

        scope.selectedItems = [2];
        var directiveMarkup = '<ul sf-collection sf-template-url="sf-collection/marks-multi-select.html" sf-data="dataItems" sf-multiselect ng-model="selectedItems" sf-identifier="id"></ul>';
        var element = commonMethods.compileDirective(directiveMarkup, scope);
        $('.sfCollectionItem:contains("4")').click();
        scope.$digest();

        expect(scope.selectedItems.length).toEqual(2);
        expect(scope.selectedItems[0]).toEqual(2);
        expect(scope.selectedItems[1]).toEqual(4);
    });

    it('[Boyko-Karadzhov] / should set sf-collection-grid class by default.', function () {
        var scope = rootScope.$new();
        templateCache.put('/Frontend-Assembly/Telerik.Sitefinity.Frontend/sf-collection/views.html', '<span>The collection.</span><div><a class="grid" ng-click="switchToGrid()">Grid</a><a class="list" ng-click="switchToList()">List</a></div>');

        var directiveMarkup = '<div sf-collection sf-template-url="sf-collection/views.html"></div>';
        var element = commonMethods.compileDirective(directiveMarkup, scope);

        expect($(element).is('.sf-collection-grid')).toBe(true);
        expect($(element).is('.sf-collection-list')).toBe(false);
    });

    it('[Boyko-Karadzhov] / should set sf-collection-list class when switchToList is clicked.', function () {
        var scope = rootScope.$new();
        templateCache.put('/Frontend-Assembly/Telerik.Sitefinity.Frontend/sf-collection/views.html', '<span>The collection.</span><div><a class="grid" ng-click="switchToGrid()">Grid</a><a class="list" ng-click="switchToList()">List</a></div>');

        var directiveMarkup = '<div sf-collection sf-template-url="sf-collection/views.html"></div>';
        var element = commonMethods.compileDirective(directiveMarkup, scope);
        $(element).find('.list').click();
        scope.$digest();

        expect($(element).is('.sf-collection-grid')).toBe(false);
        expect($(element).is('.sf-collection-list')).toBe(true);
    });
});