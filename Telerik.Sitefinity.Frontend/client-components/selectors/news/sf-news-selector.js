﻿(function ($) {
    angular.module('sfSelectors')
        .directive('sfNewsSelector', ['sfNewsItemService', function (newsItemService) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        var status = attrs.sfMaster === 'true' || attrs.sfMaster === 'True' ? 'master' : 'live';
                        ctrl.getItems = function (skip, take, search, frontendLanguages) {
                            var provider = ctrl.$scope.sfProvider;
                            return newsItemService.getItems(provider, skip, take, search, frontendLanguages, status);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var provider = ctrl.$scope.sfProvider;
                            return newsItemService.getSpecificItems(ids, provider, status);
                        };

                        ctrl.selectorType = 'NewsSelector';

                        ctrl.dialogTemplateUrl = 'client-components/selectors/news/sf-news-selector.sf-cshtml';
                        ctrl.$scope.dialogTemplateId = 'sf-news-selector-template';

                        var closedDialogTemplate = attrs.sfMultiselect ?
                            'client-components/selectors/common/sf-list-group-selection.sf-cshtml' :
                            'client-components/selectors/common/sf-bubbles-selection.sf-cshtml';

                        ctrl.closedDialogTemplateUrl = closedDialogTemplate;
                    }
                }
            };
        }]);
})(jQuery);