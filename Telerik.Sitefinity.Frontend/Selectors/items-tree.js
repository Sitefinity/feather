(function () {
    angular.module('selectors')
        .directive('sfItemsTree', ['serverContext', function (serverContext) {
            return {
                restrict: 'E',
                scope: {
                    sfItems: '=',
                    sfItemSelected: '&',
                    sfGetChildren: '&',
                    sfIdentifierFieldValue: '&'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'Selectors/items-tree.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs) {
                    scope.itemsDataSource = new kendo.data.HierarchicalDataSource({
                        //data: scope.sfItems,
                        // data: [{ text: 'asd', HasChildren: false }, { text: 'asd', HasChildren: true }],
                        schema: {
                            model: {
                                //text: 'text',                                
                                hasChildren: "HasChildren",
                            }
                        },
                        select: function (e) {
                            alert(e);
                        }
                    });

                    scope.$watchCollection('sfItems', function (newValue) {
                        scope.itemsDataSource.data(newValue);
                    });

                    scope.expand = function (e) {
                        var dataItem = e.sender.dataItem(e.node);
                        if (dataItem && dataItem.Id) {
                            scope.sfGetChildren({ parentId: dataItem.Id })
                                .then(function (data) {
                                    //dataItem.items = [];
                                    //Array.prototype.push.apply(dataItem.items, data);
                                    angular.forEach(data, function (item, _) {
                                        dataItem.items.push(item);
                                    });
                                });
                        }
                    };

                    scope.select = function (e) {
                        var dataItem = e.sender.dataItem(e.node);
                        scope.sfItemSelected({ dataItem: dataItem });
                    };

                    scope.itemTemplate = "<a ng-class=\"{'list-group-item':true }\" style='text-overflow: ellipsis; overflow: hidden;'>{{ sfIdentifierFieldValue({dataItem: dataItem}) }}</a>";
                    //scope.itemTemplate = "<span style='cursor:hand'>{{ dataItem.Title.Value }}</span>";
                    //scope.itemTemplate = "<span style='cursor:hand'>{{dataItem.title}}</span>";
                }
            }
        }]);
})();