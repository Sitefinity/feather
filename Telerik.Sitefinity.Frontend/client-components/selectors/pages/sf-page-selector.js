(function () {
    angular.module('sfSelectors')
        .directive('sfPageSelector', ['sfPageService', 'serverContext', '$q', function (pageService, serverContext, $q) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        var rootPage = serverContext.getCurrentFrontendRootNodeId();

                        // <------- Begin: Helper methods ------
                        var getSiteMapRootNodeId = function () {
                            var selectedSite = scope.$eval(attrs.sfPageSelector);

                            if (selectedSite && selectedSite.SiteMapRootNodeId) {
                                return selectedSite.SiteMapRootNodeId;
                            }
                            return rootPage;
                        };

                        var getItems = function (parentId, search) {
                            var provider = ctrl.$scope.sfProvider;
                            var siteId = getSiteId();
                            var culture = getCulture();
                            return pageService.getItems(parentId, siteId, provider, search, culture);
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

                        var getSiteId = function () {
                            var selectedSite = scope.$eval(attrs.sfPageSelector);

                            if (selectedSite && selectedSite.Id) {
                                return selectedSite.Id;
                            }
                            return ctrl.$scope.siteId;
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

                        var invalidateCurrentSelection = function () {
                            ctrl.resetItems();

                            // We should clear the selection, because it is not relevant anymore.
                            if (ctrl.$scope.sfSelectedIds) {
                                ctrl.$scope.sfSelectedIds.length = 0;
                            }

                            if (ctrl.$scope.sfSelectedItems) {
                                ctrl.$scope.sfSelectedItems.length = 0;
                            }

                            ctrl.beginLoadingItems();
                        };
                        // ------ End: Helper methods ------->

                        scope.$watch(attrs.sfPageSelector, function (newSite, oldSite) {
                            if (allowLoadingItems(newSite, oldSite)) {
                                invalidateCurrentSelection();
                            }
                        });

                        scope.$watch(attrs.sfCulture, function (newLang, oldLang) {
                            if (!areLanguageEqual(newLang, oldLang)) {
                                if (!ctrl.$scope.filter.isEmpty) {
                                    invalidateCurrentSelection();
                                }
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
                            var rootId = getSiteMapRootNodeId();
                            return pageService.getPredecessors(itemId, provider, rootId);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var provider = ctrl.$scope.sfProvider;

                            var rootId = getSiteMapRootNodeId();

                            var specificItemsPromise = pageService.getSpecificItems(ids, provider, rootId);

                            ctrl.$scope.selectedIdsPromise  = specificItemsPromise.then(function (data) {
                                return data.Items.map(function (item) {
                                    return item.Id;
                                });
                            });

                            return specificItemsPromise;
                        };

                        var resetItemsBase = ctrl.resetItems;
                        ctrl.resetItems = function () {
                            ctrl.$scope.selectedIdsPromise = null;
                            resetItemsBase.apply(ctrl, arguments);
                        };

                        ctrl.itemDisabled = function (item) {
                            var uiCulture = getCulture();

                            if (uiCulture && item.AvailableLanguages && item.AvailableLanguages.length > 0) {
                                return item.AvailableLanguages.indexOf(uiCulture) < 0;
                            }
                            return false;
                        };

                        // Adds multilingual support.
                        ctrl.$scope.bindPageIdentifierField = function (dataItem) {
                            return pageService.getPageTitleByCulture(dataItem, getCulture());
                        };

                        ctrl.OnItemsFiltering = function (items) {
                            var culture = getCulture();
                            if (items && culture) {
                                return items.filter(function (element) {
                                    // Check only in multilingual.
                                    if (element.AvailableLanguages.length > 0) {
                                        return element.AvailableLanguages.indexOf(culture) > -1;
                                    }
                                    return true;
                                });
                            }
                            return items;
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

                        var templateHtml = "<a ng-click=\"sfSelectItem({ dataItem: dataItem })\" ng-class=\"{'disabled': sfItemDisabled({dataItem: dataItem}),'active': sfItemSelected({dataItem: dataItem})}\" >" +
                                                  "<i class='pull-left icon-item-{{dataItem.Status.toLowerCase()}}'></i>" +
                                                  "<span class='pull-left'>" +
                                                      "<span ng-class=\"{'text-muted': sfItemDisabled({dataItem: dataItem})}\" ng-bind=\"sfIdentifierFieldValue({dataItem: dataItem})\"></span> <em ng-show='sfItemDisabled({dataItem: dataItem})' class='m-left-md'>(not translated)</em>" +
                                                      "<span class='small text-muted' ng-bind='dataItem.Status'></span>" +
                                                  "</span>" +
                                            "</a>";
                        ctrl.$scope.singleItemTemplateHtml = templateHtml;

                        ctrl.onPostLinkComleted = function () {
                            if (sitefinity.isMultisiteEnabled()) {
                                var currentSite = scope.$eval(attrs.sfPageSelector);
                                if (currentSite) {
                                    ctrl.beginLoadingItems();
                                }
                            }
                            else {
                                ctrl.beginLoadingItems();
                            }
                        };
                    }
                }
            };
        }]);
})();
