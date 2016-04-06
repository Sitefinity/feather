(function ($) {
    angular.module('sfSelectors').directive('sfEventSelector', ['sfEventService', function (eventService) {
        return {
            require: '^sfListSelector',
            restrict: 'A',
            link: {
                pre: function (scope, element, attrs, ctrl) {
                    ctrl.getItems = function (skip, take, search, frontendLanguages) {
                        return eventService.getItems(ctrl.$scope.sfProvider, skip, take, search, frontendLanguages);
                    };

                    ctrl.getSpecificItems = function (ids) {
                        return eventService.getSpecificItems(ids, ctrl.$scope.sfProvider);
                    };

                    ctrl.selectorType = 'EventSelector';
                    ctrl.dialogTemplateUrl = 'client-components/selectors/events/sf-event-selector.sf-cshtml';
                    ctrl.$scope.dialogTemplateId = 'sf-event-selector-template';
                    
                    var closedDialogTemplate = attrs.sfMultiselect ?
                        'client-components/selectors/common/sf-list-group-selection.sf-cshtml' :
                        'client-components/selectors/common/sf-bubbles-selection.sf-cshtml';

                    ctrl.closedDialogTemplateUrl = closedDialogTemplate;
                }
            }
        };
    }]);
}(jQuery));