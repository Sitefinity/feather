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

                        //creates new guid
                        var guid = (function () {
                            function s4() {
                                return Math.floor((1 + Math.random()) * 0x10000)
                                           .toString(16)
                                           .substring(1);
                            }
                            return function () {
                                return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
                                       s4() + '-' + s4() + s4() + s4();
                            };
                        })();

                        // The view is binded to this collection
                        if (!scope.sfExternalPages)
                            scope.sfExternalPages = [];

                        scope.isListEmpty = function () {
                            return scope.sfExternalPages && scope.sfExternalPages.length === 0;
                        };

                        scope.isItemSelected = function (externalPageId) {
                            if (scope.sfSelectedItems) {
                                for (var i = 0; i < scope.sfSelectedItems.length; i++) {
                                    if (scope.sfSelectedItems[i].ExternalPageId === externalPageId) {
                                        return true;
                                    }
                                }
                            }

                            return false;
                        };

                        scope.addItem = function () {
                            scope.sfExternalPages.push({ ExternalPageId: guid(), TitlesPath: 'Enter title', Url: 'Enter URL'});
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
                                if (scope.sfSelectedItems[i].ExternalPageId === item.ExternalPageId) {
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
