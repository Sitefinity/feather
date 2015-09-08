; (function ($) {
    angular.module('sfSelectors').requires.push('sfFormSelector');
    angular.module('sfFormSelector', ['sfServices', 'sfInfiniteScroll', 'sfCollection', 'sfSearchBox'])
        .directive('sfFormSelector', ['sfFormService', 'serverContext', 'serviceHelper', function (sfFormService, serverContext, serviceHelper) {
            return {
                restrict: 'AE',
                scope: {
                    selectedItems: '=?sfModel',
                    filterObject: '=?sfFilter',
                    sfMultiselect: '@',
                    sfDeselectable: '@'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/forms/sf-form-selector.sf-cshtml';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    var defaultItemsTake = 20;

                    scope.items = [];
                    scope.filterObject = scope.filterObject || {};

                    if (!scope.selectedItems || !angular.isArray(scope.selectedItems)) {
                        scope.selectedItems = [];
                    }

                    scope.isMultiselect = scope.sfMultiselect !== undefined && scope.sfMultiselect.toLowerCase() !== 'false';
                    scope.isDeselectable = scope.sfDeselectable !== undefined && scope.sfDeselectable.toLowerCase() !== 'false';

                    var loadItems = function () {
                        scope.isLoading = true;
                        sfFormService.getItems(scope.filterObject.provider, scope.filterObject.skip, scope.filterObject.take || defaultItemsTake, scope.filterObject.search)
                            .then(function (items) {
                                items = items || {};
                                scope.items = items.Items || [];
                                scope.isLoading = false;
                            });
                    };

                    scope.loadMoreItems = function () {
                        sfFormService.getItems(scope.filterObject.provider, scope.items.length, scope.filterObject.take || defaultItemsTake, scope.filterObject.search)
                            .then(function (items) {
                                items = items || {};
                                Array.prototype.push.apply(scope.items, items.Items || []);
                            });
                    };

                    loadItems();

                    scope.$watch('filterObject.search', function (newVal, oldVal) {
                        if (newVal != oldVal) {
                            loadItems();
                        }
                    });
                }
            };
        }])

        .directive('sfScrollIfSelected', [function () {
            return {
                link: function (scope, element) {
                    if (scope.isSelected(scope.item)) {
                        setTimeout(function () {
                            element[0].scrollIntoView();
                        }, 0);
                    }
                }
            };
        }]);
})(jQuery);