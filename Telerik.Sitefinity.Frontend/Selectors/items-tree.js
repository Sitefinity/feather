(function () {
    angular.module('selectors')
        .directive('sfItemsTree', function () {
            return {
                restrict: 'E',
                scope: {
                    sfItems: '=',
                    sfItemSelected: '&',
                    sfGetChildren: '&'
                },
                link: function (scope, element, attrs) {
                    scope.itemsDataSource = new kendo.data.HierarchicalDataSource({
                        data: scope.sfItems,
                        schema: {
                            model: {
                                hasChildren: "HasChildren",
                            }
                        },
                        select: function (e) {
                            alert(e);
                        }
                    });

                    scope.expand = function (e) {
                        var dataItem = e.sender.dataItem(e.node);
                        if (dataItem && dataItem.Id) {
                            scope.sfGetChildren(dataItem.id)
                                .then(function (data) {
                                    dataItem.items = [];
                                    Array.prototype.push.apply(dataItem.items, data);
                                    //angular.forEach(data, function (item, _) {
                                    //    dataItem.items.push(item);
                                    //});
                                });
                        }
                    };

                    scope.change = function (dataItem) {
                        //TODO: check if this method is actually needed.
                        scope.sfItemSelected(dataItem);
                    };

                    scope.itemTemplate = "<span style='cursor:hand'>{{dataItem.Title}}</span>";
                }
            }
        });
})();