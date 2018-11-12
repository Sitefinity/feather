(function ($) {
    angular.module('sfSelectors').directive('sfEventSelector', ['sfEventService', function (eventService) {
        return {
            require: '^sfListSelector',
            restrict: 'A',
            link: {
                pre: function (scope, element, attrs, ctrl) {
                    var status = attrs.sfMaster === 'true' || attrs.sfMaster === 'True' ? 'master' : 'live';

                    ctrl.getItems = function (skip, take, search, frontendLanguages) {
                        return eventService.getItems(ctrl.$scope.sfProvider, skip, take, search, frontendLanguages, status);
                    };

                    ctrl.getSpecificItems = function (ids) {
                        return eventService.getSpecificItems(ids, ctrl.$scope.sfProvider, status);
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