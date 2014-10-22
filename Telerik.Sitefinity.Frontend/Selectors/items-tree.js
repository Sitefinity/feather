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
                                id: 'Id',                                
                                hasChildren: 'HasChildren'
                            }
                        },
                        transport: {
                            read: function (options) {
                                var id = options.data.Id;

                                if (id) {
                                    scope.sfGetChildren({ parentId: id })
                                        .then(function (data) {
                                            options.success(data);
                                        });
                                }
                            }
                        }
                    });

                    scope.$watchCollection('sfItems', function (newValue) {
                        scope.itemsDataSource.data(newValue);
                    });

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