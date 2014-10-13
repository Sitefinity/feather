(function ($) {
    angular.module('selectors')
        .directive('selectedItemsView', ['serverContext', function (serverContext) {
            return {
                restrict: "E",
                scope: {
                    items: '=?',
                    selectedItems: '=?',
                    identifierField: '=?'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'Selectors/selected-items-view.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    post: function (scope, element, attrs) {
                        var defaultIdentifierField = 'Title';
                        var identifierField = scope.identifierField || defaultIdentifierField;
                        var originalItems = [];

                        scope.$watch('items.length', function () {
                            // the collection is not changed by the search
                            if (!scope.filter.searchString || scope.filter.searchString === "") {
                                originalItems = [];
                                Array.prototype.push.apply(originalItems, scope.items);
                            }
                        });

                        scope.isListEmpty = function () {
                            return originalItems.length === 0;
                        };

                        scope.bindIdentifierField = function (item) {
                            if (item) {
                                var mainField = item[identifierField];
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

                        scope.isItemSelectedInDialog = function (item) {
                            for (var i = 0; i < scope.selectedItems.length; i++) {
                                if (scope.selectedItems[i].Id === item.Id) {
                                    return true;
                                }
                            }
                        };

                        scope.itemClicked = function (item) {
                            var alreadySelected;
                            var selectedItemindex;
                            for (var i = 0; i < scope.selectedItems.length; i++) {
                                if (scope.selectedItems[i].Id === item.Id) {
                                    alreadySelected = true;
                                    selectedItemindex = i;
                                    break;
                                }
                            }

                            if (alreadySelected) {
                                scope.selectedItems.splice(selectedItemindex, 1);
                            }
                            else {
                                scope.selectedItems.push(item);
                            }
                            
                        };

                        scope.filter = {
                            placeholder: 'Narrow by typing',
                            timeoutMs: 500,
                            search: function (keyword) {
                                scope.items.length = 0;

                                if (!keyword) {
                                    Array.prototype.push.apply(scope.items, originalItems);
                                }
                                else {
                                    for (var i = 0; i < originalItems.length; i++) {
                                        if (scope.bindIdentifierField(originalItems[i]).toLowerCase().indexOf(keyword.toLowerCase()) !== -1) {
                                            scope.items.push(originalItems[i]);
                                        }
                                    }
                                }
                            }
                        };
                    }
                }
            };
        }]);
})(jQuery);