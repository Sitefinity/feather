/* global angular */

(function () {
    var modalDialogModule = angular.module('modalDialog', ['ui.bootstrap']);

    modalDialogModule.directive('modal', ['$modal', function ($modal) {
        var designerDlgClass = 'sf-designer-dlg';

        var resolveControllerName = function (attrs) {
            if (!attrs.dialogController)
                throw 'Please insert an attribute named "dialog-controller" with the name of the controller for the modal dialog next to the "modal" directive.';

            return attrs.dialogController;
        };

        return {
            restrict: 'A',
            link: function (scope, elem, attrs) {
                elem.on('remove', function () {
                    $('.' + designerDlgClass).remove();
                    $('div.modal-backdrop').remove();
                });

                var modalInstance = $modal.open({
                    backdrop: 'static',
                    templateUrl: 'dialog-template',
                    controller: resolveControllerName(attrs),
                    windowClass: designerDlgClass
                });
            }
        };
    }]);
})();