; (function () {
    var sfSelectors = angular.module('sfSelectors');
    sfSelectors.requires.push('sfInfiniteScroll');

    sfSelectors
        .directive('sfImageSelector', ['serverContext', 'sfImageService', 'serviceHelper', function (serverContext, sfImageService, serviceHelper) {
            var constants = {
                initialLoadedItemsCount: 50,
                infiniteScrollLoadedItemsCount: 20,
            };

            var TaxonFilterObject = function () {
                this.id = null;
                this.field = null;

                this.expression = function () {
                    return this.field + '.Contains(' + this.id + ')';
                };
            };

            var FilterObject = function () {
                // Query that is typed by a user in a text box.
                this.query = null;

                // Recent, My or AllLibraries
                this.basic = null;

                // Parent id
                this.parent = null;

                // Last 1 day, Last 3 days, etc...
                this.date = null;

                // Filter by any taxon
                this.taxon = null;

                this.expression = function () {
                    return serviceHelper.filterBuilder()
                                .lifecycleFilter();
                };
            };

            scope.items = [];
            scope.filterObject = {};
            scope.sortExpression = null;

            var filterExpression = null;

            return {
                restrict: 'E',
                scope: {
                    selectedItem: '=ngModel'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/images/sf-image-selector.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    scope.filterObject = new FilterObject();
                    scope.sortExpression = null;
                    scope.items = [];

                    var filterExpression = null;

                    // initial open populates dialog with all root libraries
                    sfImageService.getFolders({ take: constants.initialLoadedItemsCount }).then(function (rootFolders) {
                        if (rootFolders && rootFolders.Items) {
                            scope.items = rootFolders.Items;
                        }
                    });

                    scope.loadMore = function () {

                    };

                    scope.$watch('filterObject', function (newVal, oldVal) {
                        filterExpression = composeFilter(newVal);
                        refresh();
                    }, true);

                    scope.$watch('sortExpression', function (newVal, oldVal) {
                        refresh();
                    }, true);

                    var refresh = function () {

                    };

                    var composeFilter = function (filterObject) {

                    };
                }
            };
        }]);
})();