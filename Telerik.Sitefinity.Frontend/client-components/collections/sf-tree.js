; (function ($) {
    angular.module('sfTree', ['sfServices'])
        .directive('sfTree', ['serverContext', '$q', function (serverContext, $q) {
            var TreeNode = function (item) {
                this.item = item;
                this.children = null;
                this.collapsed = true;
            };

            return {
                restrict: 'AE',
                scope: {
                    selectedId: '=?ngModel',
                    sfIdentifier: '@',
                    sfHasChildrenField: '@',
                    sfExpandOnSelect: '@',
                    sfItemTemplateUrl: '@',
                    sfItemTemplateAssembly: '@',
                    sfRequestChildren: '&'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/collections/sf-tree.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    scope.sfItemTemplateAssembly = scope.sfItemTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    scope.itemTemplateUrl = scope.sfItemTemplateUrl || 'client-components/collections/sf-tree-item.html';
                    scope.itemTemplateUrl = serverContext.getEmbeddedResourceUrl(scope.sfItemTemplateAssembly, scope.sfItemTemplateUrl);
                    scope.sfIdentifier = scope.sfIdentifier || 'Id';
                    scope.hierarchy = {};

                    // In case no function for getting children is provided, a default one returning empty array is provided.
                    scope.sfRequestChildren = scope.sfRequestChildren || function () { return []; };

                    // TODO: Remove
                    //scope.sfRequestChildren = function (parentItem) {
                    //    parentItem = parentItem || { Id: 'Root' };
                    //    var items = [];
                    //    for (var i = 0; i < 5; i++) {
                    //        items.push({
                    //            Id: i + '#' + parentItem.Id,
                    //            Title: i + '#' + 'Title ' + parentItem.Id
                    //        });
                    //    };

                    //    items[0].HasChildren = true;
                    //    items[1].HasChildren = true;

                    //    console.log('Requested!');

                    //    var result = $q.defer();
                    //    result.resolve(items);
                    //    return result.promise;
                    //}

                    //TODO : THIS
                    //var hItem = {};
                    //scope.hasChildren = function (item) {
                    //    return (hItem.children !== null && hItem.children.length > 0) ||
                    //            (!scope.sfHasChildrenField && hItem.children === null) ||
                    //            (item[scope.sfHasChildrenField] === true);
                    //};

                    scope.hasChildren = function (node) {
                        return node.item.HasChildren === true;
                    };

                    scope.isSelected = function (node) {
                        return node.item[scope.sfIdentifier] === scope.selectedId;
                    };

                    scope.select = function (node) {
                        scope.selectedId = node.item[scope.sfIdentifier];

                        if (scope.sfExpandOnSelect !== undefined) {
                            scope.toggle(node);
                        }
                    };

                    scope.toggle = function (parentNode) {
                        if (!parentNode || parentNode.children === {}) {
                            return;
                        }

                        // no requests yet
                        if (parentNode.children === null) {
                            scope.sfRequestChildren({ parent: parentNode.item }).then(function (items) {
                                if (items && items instanceof Array) {
                                    parentNode.children = parentNode.children || {};

                                    // Item must remain collapsed if it has no children
                                    if (items.length === 0) {
                                        parentNode.collapsed = false;
                                    }
                                    else {
                                        items.forEach(function (item) {
                                            parentNode.children[item[scope.sfIdentifier]] = new TreeNode(item);
                                        });
                                    }
                                }
                            });
                        }

                        parentNode.collapsed = !parentNode.collapsed;
                    };

                    // Initial load of root elements
                    scope.sfRequestChildren({ parent: null }).then(function (items) {
                        if (items && items instanceof Array) {
                            items.forEach(function (item) {
                                scope.hierarchy[item[scope.sfIdentifier]] = new TreeNode(item);
                            });
                        }
                    });
                }
            };
        }]);
})(jQuery);