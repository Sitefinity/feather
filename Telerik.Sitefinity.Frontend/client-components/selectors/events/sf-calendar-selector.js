(function ($) {
    angular.module('sfSelectors')
        .directive('sfCalendarSelector', ['sfCalendarService', 'serverContext', function (calendarService, serverContext) {

            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {

                        var fromCurrentLanguageOnly = scope.$eval(attrs.sfFromCurrentLanguageOnly);

                        function isItemTranslated(item) {
                            var uiCulture = serverContext.getUICulture();

                            if (uiCulture && item.AvailableLanguages && item.AvailableLanguages.length > 0) {
                                return item.AvailableLanguages.indexOf(uiCulture) >= 0;
                            }
                            return true;
                        }

                        ctrl.getItems = function (skip, take, search, frontendLanguages) {
                            var provider = ctrl.$scope.sfProvider;
                            var itemsPromise = calendarService.getItems(provider, skip, take, search, frontendLanguages);
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
                            var itemsPromise = calendarService.getSpecificItems(ids);
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

                        ctrl.selectorType = 'CalendarSelector';
                        ctrl.dialogTemplateUrl = 'client-components/selectors/events/sf-calendar-selector.sf-cshtml';
                        ctrl.closedDialogTemplateUrl = 'client-components/selectors/common/sf-bubbles-selection.sf-cshtml';
                        ctrl.$scope.dialogTemplateId = 'sf-calendar-selector-template';
                    }
                }
            };
        }]);
})(jQuery);