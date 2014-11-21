(function ($) {
    angular.module('sfSelectors')
        .directive('sfSelectedItemsView', ['serverContext', function (serverContext) {
            return {
                restrict: "E",
                scope: {
                    items: '=?',
                    selectedItems: '=?',
                    identifierField: '=?',
                    searchIdentifierField: '=?',
                    sortable: '@'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'client-components/selectors/common/sf-selected-items-view.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    post: function (scope, element, attrs) {
                        var defaultIdentifierField = 'Title';
                        var identifierField = scope.identifierField || defaultIdentifierField;

                        // The view is binded to this collection
                        scope.currentItems = [];

                        scope.$watch('items.length', function (newValue, oldValue) {
                            if (newValue !== 0) {
                                scope.currentItems = [];
                                Array.prototype.push.apply(scope.currentItems, scope.items);

                                if (scope.filter.searchString && scope.filter.searchString !== "") {
                                    scope.filter.search(scope.filter.searchString);
                                }
                            }
                        });

                        scope.isListEmpty = function () {
                            return scope.items.length === 0;
                        };

                        scope.bindIdentifierField = function (item) {
                            return bindSearchIdentifierField(item, identifierField);
                        };

                        var bindSearchIdentifierField = function (item, filterIdentifierField) {
                            if (item) {
                                var mainField = item[filterIdentifierField];

                                var valueProp = 'Value';

                                if (!mainField) {
                                    return item.Id;
                                }

                                if (typeof mainField === 'string') {
                                    return mainField;
                                }
                                else if (valueProp in mainField) {
                                    return mainField.Value;
                                }
                            }
                        };

                        scope.sortItems = function (e) {
                            var element = scope.selectedItems[e.oldIndex];
                            scope.selectedItems.splice(e.oldIndex, 1);
                            scope.items.splice(e.oldIndex, 1);
                            scope.selectedItems.splice(e.newIndex, 0, element);
                            scope.items.splice(e.newIndex, 0, element);
                        };

                        scope.itemClicked = function (item) {
                            var selectedItemIndex;
                            for (var i = 0; i < scope.selectedItems.length; i++) {
                                if (scope.selectedItems[i].item.Id === item.Id) {
                                    selectedItemIndex = i;
                                    break;
                                }
                            }

                            scope.selectedItems[selectedItemIndex].isChecked = !scope.selectedItems[selectedItemIndex].isChecked;
                        };

                        scope.filter = {
                            placeholder: 'Narrow by typing',
                            timeoutMs: 500,
                            search: function (keyword) {
                                scope.currentItems.length = 0;

                                if (!keyword) {
                                    Array.prototype.push.apply(scope.currentItems, scope.items);
                                }
                                else {
                                    for (var i = 0; i < scope.items.length; i++) {
                                        if (scope.searchIdentifierField) {
                                            if (bindSearchIdentifierField(scope.items[i].item, scope.searchIdentifierField).toLowerCase().indexOf(keyword.toLowerCase()) !== -1) {
                                                scope.currentItems.push(scope.items[i]);
                                            }
                                        }
                                        else if (scope.bindIdentifierField(scope.items[i].item).toLowerCase().indexOf(keyword.toLowerCase()) !== -1) {
                                            scope.currentItems.push(scope.items[i]);
                                        }
                                    }
                                }
                            }
                        };

                        scope.sortableOptions = {
                            hint: function (element) {
                                return $('<div class="list-group-item list-group-item-multiselect list-group-item-draggable list-group-item-hint">' +
                                            element.html() +
                                        '</div>');
                            },
                            placeholder: function (element) {
                                return $('<div class="list-group-item list-group-item-placeholder"></div>');
                            },
                            handler: ".handler",
                            axis: "y"
                        };
                    }
                }
            };
        }]);
})(jQuery);
