(function ($) {
    angular.module('sfSelectors')
        .directive('sfSelectedItemsView', ['serverContext', function (serverContext) {
            return {
                restrict: "E",
                scope: {
                    sfItems: '=?',
                    sfSelectedItems: '=?',
                    sfIdentifierField: '=?',
                    sfSearchIdentifierField: '=?',
                    sfSortable: '@'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/common/sf-selected-items-view.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    post: function (scope, element, attrs) {
                        var defaultIdentifierField = 'Title';
                        var identifierField = scope.sfIdentifierField || defaultIdentifierField;

                        // The view is binded to this collection
                        scope.currentItems = [];

                        scope.$watch('sfItems.length', function (newValue, oldValue) {
                            if (newValue && newValue !== 0) {
                                scope.currentItems = [];
                                Array.prototype.push.apply(scope.currentItems, scope.sfItems);

                                if (scope.filter.searchString && scope.filter.searchString !== "") {
                                    scope.filter.search(scope.filter.searchString);
                                }
                            }
                            else {
                                scope.currentItems = [];
                            }
                        });

                        scope.isListEmpty = function () {
                            return scope.sfItems && scope.sfItems.length === 0;
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
                            var element = scope.sfItems[e.oldIndex];
                            scope.sfItems.splice(e.oldIndex, 1);
                            scope.sfItems.splice(e.newIndex, 0, element);
                        };

                        scope.isItemSelected = function (id, externalPageId) {
                            if (scope.sfSelectedItems) {
                                for (var i = 0; i < scope.sfSelectedItems.length; i++) {
                                    if ((id && scope.sfSelectedItems[i].Id === id) ||
                                        (scope.sfSelectedItems[i].ExternalPageId === externalPageId)) {
                                        return true;
                                    }
                                }
                            }

                            return false;
                        };

                        scope.itemClicked = function (item) {
                            if (!scope.sfSelectedItems) {
                                scope.sfSelectedItems = [];
                            }

                            var selectedItemIndex;
                            var alreadySelected = false;
                            for (var i = 0; i < scope.sfSelectedItems.length; i++) {
                                if ((item.Id && scope.sfSelectedItems[i].Id === item.Id)||
                                    (!item.Id && scope.sfSelectedItems[i].ExternalPageId === item.ExternalPageId)) {
                                    selectedItemIndex = i;
                                    alreadySelected = true;
                                    break;
                                }
                            }

                            if (alreadySelected) {
                                scope.sfSelectedItems.splice(selectedItemIndex, 1);
                            }
                            else {
                                scope.sfSelectedItems.push(item);
                            }
                        };

                        scope.filter = {
                            placeholder: 'Narrow by typing',
                            timeoutMs: 500,
                            search: function (keyword) {
                                scope.currentItems.length = 0;

                                if (!keyword) {
                                    Array.prototype.push.apply(scope.currentItems, scope.sfItems);
                                }
                                else {
                                    for (var i = 0; i < scope.sfItems.length; i++) {
                                        if (scope.sfSearchIdentifierField) {
                                            if (bindSearchIdentifierField(scope.sfItems[i], scope.sfSearchIdentifierField).toLowerCase().indexOf(keyword.toLowerCase()) !== -1) {
                                                scope.currentItems.push(scope.sfItems[i]);
                                            }
                                        }
                                        else if (scope.bindIdentifierField(scope.sfItems[i]).toLowerCase().indexOf(keyword.toLowerCase()) !== -1) {
                                            scope.currentItems.push(scope.sfItems[i]);
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
