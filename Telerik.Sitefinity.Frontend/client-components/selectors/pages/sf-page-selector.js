(function () {
    angular.module('sfSelectors')
        .directive('sfPageSelector', ['sfPageService', "serverContext", function (pageService, serverContext) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        var rootPage = serverContext.getCurrentFrontendRootNodeId();

                        var getItems = function (parentId, search) {
                            var siteId = ctrl.$scope.siteId;
                            var provider = ctrl.$scope.provider;
                            return pageService.getItems(parentId, siteId, provider, search);
                        };

                        ctrl.getItems = function (skip, take, search) {
                            return getItems(rootPage, search);
                        };

                        ctrl.getChildren = function (parentId, search) {                            
                            return getItems(parentId, search)
                                .then(function (data) {
                                    return data.Items;
                                });
                        };

                        ctrl.getPredecessors = function (itemId) {
                            var provider = ctrl.$scope.provider;
                            return pageService.getPredecessors(itemId, provider);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var provider = ctrl.$scope.provider;
                            return pageService.getSpecificItems(ids, provider);
                        };

                        ctrl.itemDisabled = function (item) {
                            var uiCulture = serverContext.getUICulture();

                            if (uiCulture && item.AvailableLanguages && item.AvailableLanguages.length > 0) {
                                return item.AvailableLanguages.indexOf(uiCulture) < 0;
                            }

                            return false;
                        };

                        ctrl.selectorType = 'PageSelector';

                        ctrl.dialogTemplateUrl = 'client-components/selectors/pages/sf-page-selector.html';
                        ctrl.$scope.dialogTemplateId = 'sf-page-selector-template';

                        var closedDialogTemplate = attrs.multiselect ?
                            'client-components/selectors/common/sf-list-group-selection.html' :
                            'client-components/selectors/common/sf-bubbles-selection.html';

                        ctrl.closedDialogTemplateUrl = closedDialogTemplate;                       

                        ctrl.$scope.hierarchical = true;

                        ctrl.$scope.expandSelection = true;

                        ctrl.$scope.identifierField = "TitlesPath";

                        ctrl.$scope.searchIdentifierField = "Title";
                    }
                }
            };
        }]);
})();