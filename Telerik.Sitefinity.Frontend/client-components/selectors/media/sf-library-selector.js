(function ($) {
    angular.module('sfSelectors')
        .directive('sfLibrarySelector', ['serviceHelper', 'sfMediaService', '$q', function (serviceHelper, mediaService, $q) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        var mediaType = attrs.sfMediaType ? attrs.sfMediaType : 'images';

                        ctrl.getItems = function (skip, take, search) {
                            var filter = serviceHelper.filterBuilder()
                                                      .searchFilter(search)
                                                      .getFilter();

                            var options = {
                                parent: null,
                                filter: filter,
                                provider: ctrl.$scope.sfProvider,
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
                                provider: ctrl.$scope.sfProvider,
                                filter: filter,
                                sort: "Title ASC"
                            };

                            return mediaService[mediaType].getFolders(options).then(function (data) { return data.Items; });
                        };

                        var shouldFetch = function (ids) {
                            if (!ctrl.$scope.sfSelectedItems)
                                return true;

                            if (ids.length !== ctrl.$scope.sfSelectedItems.length)
                                return true;

                            for (var i = 0; i < ctrl.$scope.sfSelectedItems.length; i++) {
                                if (ids[i] !== ctrl.$scope.sfSelectedItems[i].Id)
                                    return true;
                            }

                            return false;
                        };

                        ctrl.getSpecificItems = function (ids) {
                            if (shouldFetch(ids)) {
                                var filter = serviceHelper.filterBuilder()
                                                          .specificItemsFilter(ids)
                                                          .getFilter();

                                var options = {
                                    parent: null,
                                    skip: 0,
                                    take: 100,
                                    provider: ctrl.$scope.sfProvider,
                                    recursive: true,
                                    filter: filter
                                };

                                return mediaService[mediaType].getFolders(options);
                            }
                            else {
                                var defer = $q.defer();
                                defer.resolve({ Items: ctrl.$scope.sfSelectedItems });
                                return defer.promise;
                            }
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
                        ctrl.dialogTemplateUrl = 'client-components/selectors/media/sf-library-selector.sf-cshtml';
                        ctrl.$scope.dialogTemplateId = 'sf-library-selector';
                        ctrl.closedDialogTemplateUrl = 'client-components/selectors/common/sf-bubbles-selection.sf-cshtml';

                        ctrl.$scope.hierarchical = true;
                        ctrl.$scope.sfIdentifierField = "Breadcrumb";
                        ctrl.$scope.searchIdentifierField = "Title";
                    }
                }
            };
        }]);
})(jQuery);