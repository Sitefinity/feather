(function ($) {
    var selectorsModule = angular.module('sfSelectors');
    selectorsModule.requires.push('sfFields');

    selectorsModule
        .directive('sfExternalUrlsView', ['serverContext', function (serverContext) {
            return {
                restrict: "E",
                scope: {
                    sfExternalPages: '=?',
                    sfSelectedItems: '=?',
                    sfOpenExternalsInNewTab: '='
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/pages/sf-external-urls-view.sf-cshtml';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    post: function (scope, element, attrs) {

                        // The view is binded to this collection
                        if (!scope.sfExternalPages)
                            scope.sfExternalPages = [];

                        scope.isListEmpty = function () {
                            return scope.sfExternalPages && scope.sfExternalPages.length === 0;
                        };

                        scope.isItemSelected = function (externalPageId, status) {
                            if (status === 'new')
                                return true;

                            if (scope.sfSelectedItems) {
                                for (var i = 0; i < scope.sfSelectedItems.length; i++) {
                                    if (scope.sfSelectedItems[i].Id === externalPageId) {
                                        return true;
                                    }
                                }
                            }

                            return false;
                        };

                        scope.addItem = function () {
                            scope.sfExternalPages.push({ Id: kendo.guid(), IsExternal: true, TitlesPath: '', Url: '', Status: 'new' });
                        };

                        scope.removeItem = function (index, item) {
                            scope.sfExternalPages.splice(index, 1);

                            var selectedItemIndex = findSelectedItemIndex(item);

                            if (selectedItemIndex > -1) {
                                scope.sfSelectedItems.splice(selectedItemIndex, 1);
                            }
                        };

                        var findSelectedItemIndex = function (item) {
                            if (!scope.sfSelectedItems) {
                                return -1;
                            }

                            for (var i = 0; i < scope.sfSelectedItems.length; i++) {
                                if (scope.sfSelectedItems[i].Id === item.Id) {
                                    return i;
                                }
                            }

                            return -1;
                        };

                        scope.itemChanged = function (item) {
                            var selectedItemIndex = findSelectedItemIndex(item);
                            if (item.TitlesPath) {
                                item.Status = 'valid';

                                if (selectedItemIndex === -1) {
                                    scope.sfSelectedItems.push(item);
                                }
                                else {
                                    scope.sfSelectedItems[selectedItemIndex] = item;
                                }
                            }
                            else {
                                if (item.Url)
                                    item.Status = 'invalid';
                                else
                                    item.Status = 'new';

                                if (selectedItemIndex > -1) {
                                    scope.sfSelectedItems.splice(selectedItemIndex, 1);
                                }
                            }
                        };

                        scope.itemClicked = function (item) {
                            if (!scope.sfSelectedItems) {
                                scope.sfSelectedItems = [];
                            }

                            var selectedItemIndex = findSelectedItemIndex(item);

                            if (selectedItemIndex > -1) {
                                scope.sfSelectedItems.splice(selectedItemIndex, 1);
                            }
                            else if (item.Status !== 'new') {
                                scope.sfSelectedItems.push(item);
                            }
                        };
                    }
                }
            };
        }]);
})(jQuery);
