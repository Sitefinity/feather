; (function ($) {
    angular.module('sfTree', ['sfServices'])
        .directive('sfTree', ['serverContext', function (serverContext) {
            return {
                restrict: 'AE',
                scope: {
                    selectedItem: '=?ngModel',
                    sfIdentifier: '@',
                    sfHasChildrenField: '@',
                    sfExpandOnSelect: '@',
                    sfItemTemplateUrl: '@',
                    sfItemTemplateAssembly: '@',
                    sfRequestChildren: '^&'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/collections/sf-tree.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    scope.sfItemTemplateAssembly = scope.sfItemTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    scope.sfItemTemplateUrl = scope.sfItemTemplateUrl || 'client-components/collections/sf-tree-item.html';
                    scope.sfItemTemplateUrl = serverContext.getEmbeddedResourceUrl(scope.sfItemTemplateAssembly, scope.sfItemTemplateUrl);
                    scope.sfIdentifier = scope.sfIdentifier || 'Id';

                    scope.hierarchy = {};

                    // TODO: Remove
                    //scope.sfRequestChildren = function (parentId) {
                    //    if (parentId === undefined) {
                    //        parentId = 'root';
                    //    }

                    //    var items = [];
                    //    for (var i = 0; i < 5; i++) {
                    //        items.push({
                    //            Id: i + '#' + parentId,
                    //            Title: 'Title ' + parentId + '#' + i
                    //        });
                    //    };

                    //    items[0].HasChildren = true;
                    //    items[1].HasChildren = true;

                    //    console.log('Requested!');

                    //    return { Items: items };
                    //}


                    //var hItem = {};
                    //scope.hasChildren = function (item) {
                    //    return (hItem.children !== null && hItem.children.length > 0) ||
                    //            (!scope.sfHasChildrenField && hItem.children === null) ||
                    //            (item[scope.sfHasChildrenField] === true);
                    //};

                    scope.hasChildren = function (item) {
                        return item.HasChildren === true;
                    };

                    scope.isSelected = function (item) {
                        return item[scope.sfIdentifier] === scope.selectedItem;
                    };

                    scope.select = function (item) {
                        scope.selectedItem = item[scope.sfIdentifier];

                        if (sfExpandOnSelect === true) {
                            scope.expandTree(item);
                        }
                    };

                    scope.toggle = function (parent) {
                        if (!parent) {
                            return;
                        }

                        if (parent.collapsed === true) {
                            parent.collapsed = false;
                        }
                        else if (parent.collapsed === false) {
                            parent.collapsed = true;
                        }
                            // If we reach here we have no property, so the children were never populated
                        else {
                            // TODO: Add!
                            scope.sfRequestChildren(parent[scope.sfIdentifier]).then(function (data) {
                                if (data && data.Items) {
                                    parent.children = parent.children || {};

                                    data.Items.forEach(function (item) {
                                        parent.children[item[scope.sfIdentifier]] = item;
                                    });
                                }
                            });

                            parent.collapsed = false;

                            // TODO: Remove!
                            //var expandData = scope.sfRequestChildren(parent[scope.sfIdentifier]);
                            //if (expandData && expandData.Items) {
                            //    parent.children = parent.children || {};

                            //    expandData.Items.forEach(function (item) {
                            //        parent.children[item[scope.sfIdentifier]] = item;
                            //    });
                            //}
                        }
                    };

                    // Initial load of root elements
                    // TODO: Add!
                    scope.sfRequestChildren().then(function (data) {
                        if (data && data.Items) {
                            data.Items.forEach(function (item) {
                                scope.hierarchy[item[scope.sfIdentifier]] = item;
                            });
                        }
                    });

                    // TODO: Remove!
                    //var initialData = scope.sfRequestChildren();
                    //if (initialData && initialData.Items) {
                    //    initialData.Items.forEach(function (item) {
                    //        scope.hierarchy[item[scope.sfIdentifier]] = item;
                    //    });
                    //};
                }
            };
        }]);
})(jQuery);