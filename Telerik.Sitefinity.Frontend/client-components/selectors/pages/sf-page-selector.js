(function () {
    angular.module('sfSelectors')
        .directive('sfPageSelector', ['sfPageService', 'serverContext', '$q', function (pageService, serverContext, $q) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        var rootPage = serverContext.getCurrentFrontendRootNodeId();

                        var itemAssembly = ctrl.$scope.sfItemTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                        var itemUrl = ctrl.$scope.sfItemTemplateUrl || 'client-components/selectors/pages/sf-page-selector-view.sf-cshtml';
                        ctrl.$scope.itemTemplateUrl = serverContext.getEmbeddedResourceUrl(itemAssembly, itemUrl);

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

                        var clearSelectedItems = function () {
                            if (ctrl.$scope.sfSelectedIds) {
                                ctrl.$scope.sfSelectedIds.length = 0;
                            }

                            if (ctrl.$scope.sfSelectedItems) {
                                ctrl.$scope.sfSelectedItems.length = 0;
                            }
                        };

                        var invalidateCurrentItems = function () {
                            ctrl.resetItems();
                            clearSelectedItems();
                            ctrl.beginLoadingItems();
                        };
                        // ------ End: Helper methods ------->

                        scope.$watch(attrs.sfPageSelector, function (newSite, oldSite) {
                            if (allowLoadingItems(newSite, oldSite)) {
                                invalidateCurrentItems();
                            }
                        });

                        scope.$watch(attrs.sfCulture, function (newLang, oldLang) {
                            if (!areLanguageEqual(newLang, oldLang)) {
                                // We strictly compare to false, because the possible values of the property are true/false.
                                // If the value is undefined this means that the property is not yet initialized.
                                if (ctrl.$scope.filter.isEmpty === false) {
                                    invalidateCurrentItems();
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
                            var siteId = getSiteId();
                            return pageService.getPredecessors(itemId, provider, siteId);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var provider = ctrl.$scope.sfProvider;

                            var rootId = getSiteMapRootNodeId();

                            var specificItemsPromise = pageService.getSpecificItems(ids, provider, rootId);

                            ctrl.$scope.selectedIdsPromise = specificItemsPromise.then(function (data) {
                                return data.Items.map(function (item) {
                                    return item.Id;
                                });
                            });

                            return specificItemsPromise;
                        };

                        var resetItemsBase = ctrl.resetItems;
                        ctrl.resetItems = function () {
                            ctrl.$scope.selectedIdsPromise = null;
                            ctrl.$scope.externalPagesInTheDialog = jQuery.extend(true, [], ctrl.$scope.sfExternalPages);
                            resetItemsBase.apply(ctrl, arguments);
                        };

                        ctrl.itemDisabled = function (item) {
                            var uiCulture = getCulture();

                            if (uiCulture && item.AvailableLanguages && item.AvailableLanguages.length > 0) {
                                return item.AvailableLanguages.indexOf(uiCulture) < 0;
                            }
                            return false;
                        };

                        ctrl.removeUnselectedItems = function () {
                            removeEmptyExternalPages();
                            if (ctrl.$scope.multiselect) {
                                var reoderedItems = [];
                                if (ctrl.$scope.selectedItemsViewData && ctrl.$scope.selectedItemsViewData.length > 0) {
                                    for (var i = 0; i < ctrl.$scope.selectedItemsViewData.length; i++) {
                                        for (var j = 0; j < ctrl.$scope.selectedItemsInTheDialog.length; j++) {
                                            if (ctrl.$scope.selectedItemsInTheDialog[j].Id && ctrl.$scope.selectedItemsInTheDialog[j].Id === ctrl.$scope.selectedItemsViewData[i].Id) {
                                                reoderedItems.push(ctrl.$scope.selectedItemsInTheDialog[j]);
                                                break;
                                            }
                                        }
                                    }
                                    ctrl.$scope.selectedItemsInTheDialog = [];
                                    Array.prototype.push.apply(ctrl.$scope.selectedItemsInTheDialog, reoderedItems);
                                }

                                ctrl.$scope.selectedItemsViewData = [];
                            }
                        };

                        ctrl.fetchSelectedItems = function () {
                            var externalPages = [];
                            if (ctrl.$scope.multiselect && ctrl.$scope.sfSelectedItems) {
                                Array.prototype.push.apply(externalPages, ctrl.$scope.sfSelectedItems.filter(function (page) {
                                    if (page.IsExternal && !page.NodeType) {
                                        return page;
                                    }
                                }));
                            }

                            var ids = ctrl.$scope.getSelectedIds();
                            currentSelectedIds = ids;

                            if (ids.length === 0) {
                                return;
                            }

                            return ctrl.getSpecificItems(ids)
                                .then(function (data) {
                                    Array.prototype.push.apply(data.Items, externalPages);

                                    ctrl.$scope.sfMissingSelectedItems = data.Items.length < ids.length;
                                    ctrl.onSelectedItemsLoadedSuccess(data);
                                }, ctrl.onError)
                                .finally(function () {
                                    ctrl.$scope.showLoadingIndicator = false;
                                });
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

                        var removeEmptyExternalPages = function () {
                            var externalPages = [];

                            if (ctrl.$scope.externalPagesInTheDialog && ctrl.$scope.externalPagesInTheDialog.length > 0) {
                                externalPages = jQuery.map(ctrl.$scope.externalPagesInTheDialog, function (item) {
                                    if (item.Status != 'new')
                                        return item;
                                });
                            }

                            ctrl.$scope.sfExternalPages = jQuery.extend(true, {}, externalPages);
                        };

                        ctrl.selectorType = 'PageSelector';

                        ctrl.dialogTemplateUrl = 'client-components/selectors/pages/sf-page-selector.sf-cshtml';
                        ctrl.$scope.dialogTemplateId = 'sf-page-selector-template';

                        var closedDialogTemplate = attrs.sfMultiselect ?
                            'client-components/selectors/common/sf-list-group-selection.sf-cshtml' :
                            'client-components/selectors/common/sf-bubbles-selection.sf-cshtml';

                        ctrl.closedDialogTemplateUrl = closedDialogTemplate;
                        ctrl.$scope.hierarchical = true;
                        ctrl.$scope.expandSelection = true;
                        ctrl.$scope.sfIdentifierField = "TitlesPath";
                        ctrl.$scope.searchIdentifierField = "Title";
                        ctrl.$scope.externalPagesInTheDialog = jQuery.extend(true, [], ctrl.$scope.sfExternalPages);

                        ctrl.onPostLinkComleted = function () {
                            var bindOnLoad = scope.$eval(attrs.sfBindOnLoad);
                            if (!bindOnLoad) return;

                            if (serverContext.isMultisiteEnabled()) {
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
