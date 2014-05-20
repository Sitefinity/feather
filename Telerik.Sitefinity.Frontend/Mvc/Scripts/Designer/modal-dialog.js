/*global angular */

(function () {
    var modalDialogModule = angular.module('modalDialog', ['ui.bootstrap']);

    modalDialogModule.directive('modal', function ($modal) {
        return {
            restrict: 'A',
            link: function (scope, elem, attrs) {
                var modalInstance = $modal.open({
                    backdrop: 'static',
                    templateUrl: 'dialog-template',
                    controller: attrs.dialogController,
                    windowClass: 'sf-designer-dlg'
                });
            }
        };
    });
})();