(function ($) {
    angular.module('sfSelectors')
        .directive('sfTaxonSelector', ['sfFlatTaxonService', 'serverContext', function (flatTaxonService, serverContext) {
            // Tags Id
            var defaultTaxonomyId = "cb0f3a19-a211-48a7-88ec-77495c0f5374";
            var emptyGuid = "00000000-0000-0000-0000-000000000000";

            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        var taxonomyId = attrs.sfTaxonomyId;

                        var fromCurrentLanguageOnly = scope.$eval(attrs.sfFromCurrentLanguageOnly);

                        if (!taxonomyId || taxonomyId === emptyGuid) {
                            taxonomyId = defaultTaxonomyId;
                        }

                        function isItemTranslated(item) {
                            var uiCulture = serverContext.getUICulture();

                            if (uiCulture && item.AvailableLanguages && item.AvailableLanguages.length > 0) {
                                return item.AvailableLanguages.indexOf(uiCulture) >= 0;
                            }
                            return true;
                        }

                        ctrl.getItems = function (skip, take, search, frontendLanguages) {
                            var itemsPromise = flatTaxonService.getTaxons(taxonomyId, skip, take, search, frontendLanguages);
                            if (fromCurrentLanguageOnly) {
                                return itemsPromise.then(function (data) {
                                    data.Items = data.Items.filter(function (item) {
                                        return isItemTranslated(item);
                                    });
                                    return data;
                                });
                            }

                            return itemsPromise;
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var itemsPromise = flatTaxonService.getSpecificItems(taxonomyId, ids);
                            if (fromCurrentLanguageOnly) {
                                return itemsPromise.then(function (data) {
                                    data.Items = data.Items.filter(function (item) {
                                        return isItemTranslated(item);
                                    });
                                    return data;
                                });                                
                            }

                            return itemsPromise;
                        };

                        ctrl.selectorType = 'TaxonSelector';
                        ctrl.dialogTemplateUrl = 'client-components/selectors/taxonomies/sf-taxon-selector.sf-cshtml';
                        ctrl.closedDialogTemplateUrl = 'client-components/selectors/common/sf-bubbles-selection.sf-cshtml';
                        ctrl.$scope.dialogTemplateId = 'sf-taxon-selector-template';
                    }
                }
            };
        }]);
})(jQuery);