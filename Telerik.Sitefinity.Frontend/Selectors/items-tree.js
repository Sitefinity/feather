(function () {
    angular.module('selectors')
        .directive('sfItemsTree', ['serverContext', function (serverContext) {
            return {
                restrict: 'E',
                scope: {
                    sfMultiselect: '=',
                    sfItems: '=',
                    sfSelectItem: '&',
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

                    scope.checkboxes = {
                        template: '<input type="checkbox" ng-click="sfSelectItem({ dataItem: dataItem })" ng-checked="sfItemSelected({dataItem: dataItem})">'
                    };

                    scope.itemTemplate = "<a ng-click=\"sfSelectItem({ dataItem: dataItem })\" ng-class=\"{'list-group-item':true, 'active': sfItemSelected({dataItem: dataItem}) }\" style='text-overflow: ellipsis; overflow: hidden;'>{{ sfIdentifierFieldValue({dataItem: dataItem}) }}</a>";
                }
            };
        }]);
})();