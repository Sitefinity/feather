(function () {
    angular.module('sfSelectors')
        .directive('sfPageSelector', ['sfPageService', 'serverContext', function (pageService, serverContext) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        var rootPage = serverContext.getCurrentFrontendRootNodeId();
                        debugger;
                        // <------- Begin: Helper methods ------
                        var getSiteMapRootNodeId = function () {
                            var selectedSite = scope.$eval(attrs.sfPageSelector);

                            if (selectedSite && selectedSite.SiteMapRootNodeId) {
                                return selectedSite.SiteMapRootNodeId;
                            }
                            return rootPage;
                        };

                        var getSiteId = function () {
                            var selectedSite = scope.$eval(attrs.sfPageSelector);

                            if (selectedSite && selectedSite.Id) {
                                return selectedSite.Id;
                            }
                            return ctrl.$scope.siteId;
                        };

                        var getItems = function (parentId, search) {
                            var siteId = getSiteId();
                            var provider = ctrl.$scope.sfProvider;
                            return pageService.getItems(parentId, siteId, provider, search);
                        };

                        var allowLoadingItems = function (newSite, oldSite) {
                            if (newSite && oldSite) {
                                return newSite.Id !== oldSite.Id;
                            }
                            else if (!newSite && !oldSite) {
                                return false;
                            }
                            return true;
                        };

                        var areLanguageEqual = function (newLang, oldLang) {
                            if (newLang && oldLang) {
                                return newLang.Culture === oldLang.Culture;
                            }
                            else if (!newLang && !oldLang) {
                                return true;
                            }
                            return false;
                        };

                        var getCulture = function () {
                            var sfCulture = scope.$eval(attrs.sfCulture);

                            return sfCulture && sfCulture.Culture ? sfCulture.Culture : serverContext.getUICulture();
                        };
                        // ------ End: Helper methods ------->

                        scope.$watch(attrs.sfPageSelector, function (newSite, oldSite) {
                            if (allowLoadingItems(newSite, oldSite)) {
                                ctrl.clearItems();
                                ctrl.beginLoadingItems();
                            }
                        });

                        scope.$watch(attrs.sfCulture, function (newLang, oldLang) {
                            if (!areLanguageEqual(newLang, oldLang)) {
                                ctrl.$scope.selectedItemsInTheDialog.length = 0;
                            }
                        });

                        ctrl.getItems = function (skip, take, search) {
                            var siteMapRootNodeId = getSiteMapRootNodeId();

                            return getItems(siteMapRootNodeId, search);
                        };

                        ctrl.getChildren = function (parentId, search) {
                            return getItems(parentId, search)
                                .then(function (data) {
                                    return data.Items;
                                });
                        };

                        ctrl.getPredecessors = function (itemId) {
                            var provider = ctrl.$scope.sfProvider;
                            return pageService.getPredecessors(itemId, provider);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var provider = ctrl.$scope.sfProvider;
                            return pageService.getSpecificItems(ids, provider);
                        };

                        ctrl.itemDisabled = function (item) {
                            var uiCulture = getCulture();

                            if (uiCulture && item.AvailableLanguages && item.AvailableLanguages.length > 0) {
                                return item.AvailableLanguages.indexOf(uiCulture) < 0;
                            }
                            return false;
                        };

                        ctrl.selectorType = 'PageSelector';

                        ctrl.dialogTemplateUrl = 'client-components/selectors/pages/sf-page-selector.html';
                        ctrl.$scope.dialogTemplateId = 'sf-page-selector-template';

                        var closedDialogTemplate = attrs.sfMultiselect ?
                            'client-components/selectors/common/sf-list-group-selection.html' :
                            'client-components/selectors/common/sf-bubbles-selection.html';

                        ctrl.closedDialogTemplateUrl = closedDialogTemplate;
                        ctrl.$scope.hierarchical = true;
                        ctrl.$scope.expandSelection = true;
                        ctrl.$scope.sfIdentifierField = "TitlesPath";
                        ctrl.$scope.searchIdentifierField = "Title";
                        ctrl.$scope.sfDialogHeader = 'Select a page';

                        ctrl.onPostLinkComleted = function () {
                            var currentSite = scope.$eval(attrs.sfPageSelector);
                            if (currentSite) {
                                ctrl.beginLoadingItems();
                            }
                        };
                    }
                }
            };
        }]);
})();