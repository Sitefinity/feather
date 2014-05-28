/*global angular */

(function () {
    var modalDialogModule = angular.module('modalDialog', ['ui.bootstrap']);

    modalDialogModule.directive('modal', ['$modal', function ($modal) {
        var resolveControllerName = function (attrs) {
            if (!attrs.dialogController)
                throw 'Please insert an attribute named "dialog-controller" with the name of the controller for the modal dialog next to the "modal" directive.';

            return attrs.dialogController;
        };

        return {
            restrict: 'A',
            link: function (scope, elem, attrs) {
                var modalInstance = $modal.open({
                    backdrop: 'static',
                    templateUrl: 'dialog-template',
                    controller: resolveControllerName(attrs),
                    windowClass: 'sf-designer-dlg'
                });
            }
        };
    }]);
})();