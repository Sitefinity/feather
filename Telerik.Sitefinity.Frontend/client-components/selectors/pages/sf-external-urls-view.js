(function ($) {
    angular.module('sfSelectors')
        .directive('sfExternalUrlsView', ['serverContext', function (serverContext) {
            return {
                restrict: "E",
                scope: {
                    sfExternalPages: '=?',
                    sfSelectedItems: '=?',
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/pages/sf-external-urls-view.html';
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

                        scope.isItemSelected = function (titlesPath) {
                            if (scope.sfSelectedItems) {
                                for (var i = 0; i < scope.sfSelectedItems.length; i++) {
                                    if (scope.sfSelectedItems[i].TitlesPath === titlesPath) {
                                        return true;
                                    }
                                }
                            }

                            return false;
                        };

                        scope.addItem = function () {
                            scope.sfExternalPages.push({ TitlesPath: 'Enter title', Url: 'Enter URL'});
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
                                return -1
                            }

                            for (var i = 0; i < scope.sfSelectedItems.length; i++) {
                                if (scope.sfSelectedItems[i].TitlesPath === item.TitlesPath) {
                                    return i;
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
                            else {
                                scope.sfSelectedItems.push(item);
                            }
                        };
                    }
                }
            };
        }]);
})(jQuery);
