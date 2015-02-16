describe('collection directive', function () {
    var rootScope;
    var templateCache;

    beforeEach(module('sfCollection'));

    beforeEach(inject(function (_$rootScope_, $templateCache) {
        rootScope = _$rootScope_;
        templateCache = $templateCache;
    }));

    afterEach(function () {
        //Tear down.
        var leftOver = $('.testDiv');
        leftOver.empty();
        leftOver.remove();
    });

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

        for (i = 1; i <= 5; i++)
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

        var directiveMarkup = '<ul sf-collection sf-template-url="sf-collection/marks-selected-items.html" sf-data="dataItems" sf-model="selectedItems" sf-identifier="id"></ul>';
        commonMethods.compileDirective(directiveMarkup, scope);
        expect($('.sfCollectionItem.sf-selected').length).toEqual(2);
        expect($('.sfCollectionItem.sf-selected span:contains(2)').length).toEqual(1);
        expect($('.sfCollectionItem.sf-selected span:contains(4)').length).toEqual(1);
    });

    it('[dzhenko] / should recognize selected items using the isSelected method if no identifier is provided and the item has Id prop.', function () {
        var scope = rootScope.$new();
        templateCache.put('/Frontend-Assembly/Telerik.Sitefinity.Frontend/sf-collection/marks-selected-items.html', '<li ng-repeat="item in items" class="sfCollectionItem" ng-class="{\'sf-selected\': isSelected(item)}"><div><span>Id:</span><span>{{item.Id}}</span></div><div>{{item.Title}}</div></li>');

        scope.dataItems = [];
        for (var i = 1; i <= 5; i++) {
            scope.dataItems.push({
                Id: i,
                title: 'Item ' + i
            });
        }

        scope.selectedItems = [scope.dataItems[1], scope.dataItems[3]];//1 and 3 index are id 2 and 4

        var directiveMarkup = '<ul sf-collection sf-template-url="sf-collection/marks-selected-items.html" sf-data="dataItems" sf-model="selectedItems"></ul>';
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
        var directiveMarkup = '<ul sf-collection sf-template-url="sf-collection/marks-single-select.html" sf-data="dataItems" sf-model="selectedItems" sf-identifier="id"></ul>';
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
        var directiveMarkup = '<ul sf-collection sf-template-url="sf-collection/marks-multi-select.html" sf-data="dataItems" sf-multiselect sf-model="selectedItems" sf-identifier="id"></ul>';
        var element = commonMethods.compileDirective(directiveMarkup, scope);
        $('.sfCollectionItem:contains("4")').click();
        scope.$digest();

        expect(scope.selectedItems.length).toEqual(2);
        expect(scope.selectedItems[0]).toEqual(2);
        expect(scope.selectedItems[1]).toEqual(4);
    });

    it('[dzhenko] / should have the only selected item if clicked twice and sf-deselectable and sf-multiselect are NOT present (by default deselecting is not allowed).', function () {
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
        var directiveMarkup = '<ul sf-collection sf-template-url="sf-collection/marks-multi-select.html" sf-data="dataItems" sf-model="selectedItems" sf-identifier="id"></ul>';
        var element = commonMethods.compileDirective(directiveMarkup, scope);
        $('.sfCollectionItem:contains("4")').click();
        scope.$digest();

        expect(scope.selectedItems[0]).toEqual(4);

        $('.sfCollectionItem:contains("4")').click();
        scope.$digest();

        expect(scope.selectedItems[0]).toEqual(4);
    });

    it('[dzhenko] / should have no selected item if clicked twice and sf-deselectable is present and sf-multiselect is NOT present.', function () {
        var scope = rootScope.$new();
        templateCache.put('/Frontend-Assembly/Telerik.Sitefinity.Frontend/sf-collection/marks-multi-select.html', '<li ng-repeat="item in items" class="sfCollectionItem" ng-click="select(item)"><div><span>Id:</span><span>{{item.id}}</span></div><div>{{item.Title}}</div></li>');

        scope.dataItems = [];
        for (var i = 1; i <= 5; i++) {
            scope.dataItems.push({
                id: i,
                title: 'Item ' + i
            });
        }

        scope.selectedItems = [];
        var directiveMarkup = '<ul sf-collection sf-template-url="sf-collection/marks-multi-select.html" sf-deselectable sf-data="dataItems" sf-model="selectedItems" sf-identifier="id"></ul>';
        var element = commonMethods.compileDirective(directiveMarkup, scope);

        $('.sfCollectionItem:contains("4")').click();
        scope.$digest();

        expect(scope.selectedItems[0]).toEqual(4);

        $('.sfCollectionItem:contains("4")').click();
        scope.$digest();

        expect(scope.selectedItems[0]).toEqual(undefined);
    });

    it('[dzhenko] / should have two selected items if you select two items and click second time on the first and have sf-multiselect (by default deselecting is not allowed).', function () {
        var scope = rootScope.$new();
        templateCache.put('/Frontend-Assembly/Telerik.Sitefinity.Frontend/sf-collection/marks-multi-select.html', '<li ng-repeat="item in items" class="sfCollectionItem" ng-click="select(item)"><div><span>Id:</span><span>{{item.id}}</span></div><div>{{item.Title}}</div></li>');

        scope.dataItems = [];
        for (var i = 1; i <= 5; i++) {
            scope.dataItems.push({
                id: i,
                title: 'Item ' + i
            });
        }

        scope.selectedItems = [];
        var directiveMarkup = '<ul sf-collection sf-template-url="sf-collection/marks-multi-select.html" sf-multiselect sf-data="dataItems" sf-model="selectedItems" sf-identifier="id"></ul>';
        var element = commonMethods.compileDirective(directiveMarkup, scope);

        $('.sfCollectionItem:contains("4")').click();
        scope.$digest();
        $('.sfCollectionItem:contains("3")').click();
        scope.$digest();

        expect(scope.selectedItems[0]).toEqual(4);
        expect(scope.selectedItems[1]).toEqual(3);

        $('.sfCollectionItem:contains("4")').click();
        scope.$digest();
        
        expect(scope.selectedItems[0]).toEqual(4);
        expect(scope.selectedItems[1]).toEqual(3);
    });

    it('[dzhenko] / should have one selected item if you select two items and click second time on the first and have sf-multiselect and sf-deselectable.', function () {
        var scope = rootScope.$new();
        templateCache.put('/Frontend-Assembly/Telerik.Sitefinity.Frontend/sf-collection/marks-multi-select.html', '<li ng-repeat="item in items" class="sfCollectionItem" ng-click="select(item)"><div><span>Id:</span><span>{{item.id}}</span></div><div>{{item.Title}}</div></li>');

        scope.dataItems = [];
        for (var i = 1; i <= 5; i++) {
            scope.dataItems.push({
                id: i,
                title: 'Item ' + i
            });
        }

        scope.selectedItems = [];
        var directiveMarkup = '<ul sf-collection sf-template-url="sf-collection/marks-multi-select.html" sf-multiselect sf-deselectable sf-data="dataItems" sf-model="selectedItems" sf-identifier="id"></ul>';
        var element = commonMethods.compileDirective(directiveMarkup, scope);

        $('.sfCollectionItem:contains("4")').click();
        scope.$digest();
        $('.sfCollectionItem:contains("3")').click();
        scope.$digest();

        expect(scope.selectedItems[0]).toEqual(4);
        expect(scope.selectedItems[1]).toEqual(3);

        $('.sfCollectionItem:contains("4")').click();
        scope.$digest();

        expect(scope.selectedItems[0]).toEqual(3);
        expect(scope.selectedItems[1]).toEqual(undefined);
    });
});