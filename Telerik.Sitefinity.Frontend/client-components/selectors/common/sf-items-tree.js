(function () {
    var module = angular.module('sfSelectors');

    module.factory('sfTreeHelper', function () {
        var constructPredecessorsTree = function (predecessors, selectedItemId) {
            // The predecessors collection contains all children of if the items that are in the path to the selected one.            

            var parentsIds = [],
                lastIndex = predecessors.length - 1,
                currentParentId = predecessors[lastIndex].ParentId,
                previousParentId,
                currentLevel = [],
                previousLevel,
                previousLevelParent,
                isFirstLevel = true,
                selectedItem;

            // We construct the tree from the bottom to the top.
            for (var i = lastIndex; i >= 0 ; i--) {
                var currentItem = predecessors[i];

                if (currentItem.ParentId !== currentParentId) {
                    // We are in a upper level in the hierarchy.

                    if (previousLevelParent) {
                        // We push the parent of the previous level here so it can appear first in the list of items.
                        currentLevel.push(previousLevelParent);
                    }

                    if (selectedItem && isFirstLevel) {
                        // The selected item should be first in the list of items.
                        currentLevel.push(selectedItem);
                    }

                    // Continue with the next level.
                    previousParentId = currentParentId;
                    currentParentId = currentItem.ParentId;

                    previousLevel = currentLevel.reverse();
                    currentLevel = [];

                    isFirstLevel = false;
                }

                // The selected item can only be in the bottom level.
                if (currentItem.Id === previousParentId) {
                    // This item is parent of the previous level of items.
                    // Append the previous level as its children.
                    parentsIds.push(currentItem.Id);
                    currentItem.items = previousLevel;
                    previousLevelParent = currentItem;
                }
                else if (currentItem.Id === selectedItemId) {
                    selectedItem = currentItem;
                }
                else {
                    currentLevel.push(currentItem);
                }
            }

            if (selectedItem && isFirstLevel) {
                // The selected item is on the root level.
                currentLevel.push(selectedItem);
            }

            if (previousLevelParent) {
                // If we have parent of a previous level this means that we are on root level and we have to add the item to it.
                currentLevel.push(previousLevelParent);
            }

            // The last level is the root level.
            return {
                items: currentLevel.reverse(),
                parentsIds: parentsIds.reverse()
            };
        };

        return {
            constructPredecessorsTree: constructPredecessorsTree
        };
    });

    /**
     * Wrapper that extends the kendo's HierarchicalDataSource
     * with ability to fetch tree levels from both already constructed tree and a web service.
     */
    module.factory('sfHybridHierarchicalDataSource', function () {
        function getFromAvailableItems(data, id) {
            if (!id) {
                return data;
            }
            else if (data) {
                for (var i = 0; i < data.length; i++) {
                    if (data[i].Id === id) {
                        return data[i].items;
                    }
                    else if (data[i].items) {
                        var result = getFromAvailableItems(data[i].items, id);
                        if (result) return result;
                    }
                }
            }
        }

        function createDataSource(model, tree, getChildren) {
            return new kendo.data.HierarchicalDataSource({
                schema: {
                    model: model
                },
                transport: {
                    read: function (options) {
                        var id = options.data.Id;
                        var availableItems = getFromAvailableItems(tree, id);
                        if (availableItems) {
                            options.success(availableItems);
                        }
                        else if (id) {
                            getChildren(id).then(function (children) {
                                options.success(children);
                            });
                        }
                    }
                }
            });
        }

        return {
            getDataSource: function (model, tree, getChildren) {
                var dataSource = createDataSource(model, tree, getChildren);
                dataSource.data(tree);

                return dataSource;
            }
        };
    });

    module.directive('sfItemsTree', ['serverContext', 'sfTreeHelper', 'sfHybridHierarchicalDataSource', '$q',
        function (serverContext, sfTreeHelper, sfHybridHierarchicalDataSource, $q) {
            return {
                restrict: 'E',
                scope: {
                    // The class indicating the element with max-height and overflow-y:auto.
                    sfScrollContainerClass: '@',
                    sfMultiselect: '=',
                    sfExpandSelection: '=',
                    sfItemsPromise: '=',
                    sfSelectedIds: '=',
                    sfSelectItem: '&',
                    sfItemSelected: '&',
                    sfItemDisabled: '&?',
                    sfGetChildren: '&',
                    sfGetPredecessors: '&',
                    sfIdentifierFieldValue: '&'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/common/sf-items-tree.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs) {
                    /* Helper methods */

                    var kendoTreeCreatedPromise = $q.defer();

                    var model = {
                        id: 'Id',
                        hasChildren: 'HasChildren'
                    };

                    /**
                     * Retrieves collection of predecessors and constructs a tree.
                     * @param  {String} selectedIds the Id of the selected item
                     * @return {Promise}            a promise for the tree
                     */
                    var constructPredecessorsTree = function (selectedIds) {
                        var selectedId = selectedIds[0];
                        return scope.sfGetPredecessors({ itemId: selectedId })
                            .then(function (predecessors) {
                                return sfTreeHelper.constructPredecessorsTree(predecessors.Items, selectedId);
                            });
                    };

                    /**
                     * Determines whether the tree should be expanded to the selected item.
                     */
                    var shouldExpandTree = function (scope) {
                        return scope.sfSelectedIds &&
                            scope.sfSelectedIds.length > 0 &&
                            !scope.sfMultiselect &&
                            scope.sfExpandSelection;
                    };

                    var getChildrenCallback = function (id) {
                        return scope.sfGetChildren({ parentId: id });
                    };

                    /**
                     * Creates a hybrid data source and sets it to the tree.
                     */
                    var bindDataSource = function (items) {
                        var itemsDataSource = sfHybridHierarchicalDataSource
                            .getDataSource(model, items, getChildrenCallback);

                        scope.treeView.setDataSource(itemsDataSource);
                    };

                    /**
                     * Expand the tree after the items are loaded and the kendo tree widget is created.
                     */
                    var expandTreeToSelectedItem = function () {
                        $q.all([
                            constructPredecessorsTree(scope.sfSelectedIds),
                            kendoTreeCreatedPromise.promise])
                        .then(function (promiseResults) {
                            var predecessorsTree = promiseResults[0];
                            bindDataSource(predecessorsTree.items);

                            return predecessorsTree;
                        })
                        .then(function (predecessorsTree) {
                            scope.treeView.expandPath(predecessorsTree.parentsIds);
                        })
                        .then(scrollToSelectedItem);
                    };

                    /**
                     * Bind the provided from the user items into the tree.
                     */
                    var setItemsIntoTree = function () {
                        $q.all([
                           scope.sfItemsPromise,
                           kendoTreeCreatedPromise.promise])
                       .then(function (promiseResults) {
                           var items = promiseResults[0];
                           bindDataSource(items);
                       });
                    };

                    /**
                     * Scrolls the items list in order to show the selected item.
                     */
                    var scrollToSelectedItem = function () {
                        if (!scope.sfScrollContainerClass) return;

                        //scroll to the selected element
                        var selectedId = scope.sfSelectedIds[0],
                            selectedDataNode = scope.treeView.dataSource.get(selectedId),
                            selectedTreeNode = scope.treeView.findByUid(selectedDataNode.uid),
                            container = $('.' + scope.sfScrollContainerClass),
                            scrollTop = container.scrollTop() - container.offset().top + selectedTreeNode.offset().top,
                            middleOffset = container.height() / 2 + selectedTreeNode.height() / 2;

                        container.animate({
                            scrollTop: scrollTop - middleOffset
                        }, 600);
                    };

                    scope.$on("kendoWidgetCreated", function (event, widget) {
                        // check if the event is emmited from our widget
                        if (widget === scope.treeView) {
                            kendoTreeCreatedPromise.resolve();
                        }
                    });

                    scope.$watch('sfItemsPromise', function () {
                        if (shouldExpandTree(scope)) {
                            expandTreeToSelectedItem();
                        }
                        else {
                            setItemsIntoTree();
                        }
                        if (scope.treeView) {
                            kendoTreeCreatedPromise.resolve();
                        }
                    });

                    scope.checkboxes = {
                        template: '<input type="checkbox" ng-click="sfSelectItem({ dataItem: dataItem })" ng-checked="sfItemSelected({dataItem: dataItem})" ng-hide="sfItemDisabled({dataItem: dataItem})">'
                    };

                    scope.itemTemplate = attrs.sfSingleItemTemplateHtml ||
                        "<a ng-click=\"sfSelectItem({ dataItem: dataItem })\" ng-class=\"{'disabled': sfItemDisabled({dataItem: dataItem}),'active': sfItemSelected({dataItem: dataItem})}\" >" +
                            "<span ng-class=\"{'text-muted': sfItemDisabled({dataItem: dataItem})}\">{{ sfIdentifierFieldValue({dataItem: dataItem}) }}</span> <em ng-show='sfItemDisabled({dataItem: dataItem})' class=\" m-left-md \">(not translated)</em>" +
                        "</a>";
                }
            };
        }]);
})();