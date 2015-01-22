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

                        scope.isItemSelected = function (id) {
                            if (scope.sfSelectedItems) {
                                for (var i = 0; i < scope.sfSelectedItems.length; i++) {
                                    if (scope.sfSelectedItems[i].Id === id) {
                                        return true;
                                    }
                                }
                            }

                            return false;
                        };

                        scope.addItem = function () {
                            scope.sfExternalPages.push({ Title: 'Enter title', Url: 'Enter URL' });
                        };

                        scope.removeItem = function (index) {
                            scope.sfExternalPages.splice(index, 1);
                        };

                        scope.itemClicked = function (item) {
                            if (!scope.sfSelectedItems) {
                                scope.sfSelectedItems = [];
                            }

                            var selectedItemIndex;
                            var alreadySelected = false;
                            for (var i = 0; i < scope.sfSelectedItems.length; i++) {
                                if (scope.sfSelectedItems[i].Title === item.Title) {
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
                    }
                }
            };
        }]);
})(jQuery);
