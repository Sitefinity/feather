(function ($) {
    angular.module('sfSelectors')
        .directive('sfLibrarySelector', ['serviceHelper', 'sfMediaService', function (serviceHelper, mediaService) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        var mediaType = attrs.sfMediaType;

                        ctrl.getItems = function (skip, take, search) {
                            var filter = serviceHelper.filterBuilder()
                                                      .searchFilter(search)
                                                      .getFilter();

                            var options = {
                                parent: null,
                                skip: skip,
                                take: take,
                                filter: filter,
                                recursive: search ? true : null,
                                sort: "Title ASC"
                            };

                            return mediaService[mediaType].getFolders(options);
                        };

                        ctrl.getChildren = function (parentId, search) {
                            var filter = serviceHelper.filterBuilder()
                                                      .searchFilter(search)
                                                      .getFilter();

                            var options = {
                                parent: parentId,
                                filter: filter,
                                sort: "Title ASC"
                            };

                            return mediaService[mediaType].getFolders(options).then(function (data) { return data.Items; });
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var filter = serviceHelper.filterBuilder()
                                                      .specificItemsFilter(ids)
                                                      .getFilter();

                            var options = {
                                skip: 0,
                                take: 100,
                                recursive: true,
                                filter: filter
                            };

                            return mediaService[mediaType].getFolders(options);
                        };

                        ctrl.onSelectedItemsLoadedSuccess = function (data) {
                            angular.forEach(data.Items, function (result) {
                                var breadcrumb = result.Path ? result.Path : result.Title;
                                result.Breadcrumb = breadcrumb;
                                result.TitlesPath = breadcrumb;
                            });

                            ctrl.updateSelection(data.Items);
                        };

                        ctrl.onItemSelected = function (item) {
                            item.Breadcrumb = item.Path ? item.Path : item.Title;
                        };

                        ctrl.onFilterItemSucceeded = function (items) {
                            angular.forEach(items, function (item) {
                                item.RootPath = item.Path ? "Under " + item.Path.replace(' > ' + item.Title, '') : 'On Top Level';
                            });
                        };

                        ctrl.selectorType = 'LibrarySelector';
                        ctrl.dialogTemplateUrl = 'client-components/selectors/libraries/sf-library-selector.html';
                        ctrl.$scope.dialogTemplateId = 'sf-hierarchical-taxon-selector';
                        ctrl.closedDialogTemplateUrl = attrs.sfMultiselect ? 'client-components/selectors/common/sf-list-group-selection.html' :
                            'client-components/selectors/common/sf-bubbles-selection.html';

                        ctrl.$scope.hierarchical = true;
                        ctrl.$scope.sfIdentifierField = "Breadcrumb";
                        ctrl.$scope.searchIdentifierField = "Title";
                    }
                }
            };
        }]);
})(jQuery);