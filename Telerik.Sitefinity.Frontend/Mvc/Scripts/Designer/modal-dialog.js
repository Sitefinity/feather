/* global angular */

(function () {
    var modalDialogModule = angular.module('modalDialog', ['ui.bootstrap']);

    modalDialogModule.directive('modal', ['$modal', function ($modal) {
        var resolveControllerName = function (attrs) {
            if (!attrs.dialogController && !attrs.existingScope) {
                throw 'Please either insert an attribute named "dialog-controller" with the name of the controller for the modal dialog next to the "modal" directive ' +
                'or insert attribute named "existing-scope" to reuse the current scope in the dialog.';
            }

            return attrs.dialogController;
        };

        var open = function (scope, attrs) {
            var modalInstance = $modal.open({
                backdrop: 'static',
                scope: attrs.existingScope && scope,
                templateUrl: attrs.templateUrl,
                controller: resolveControllerName(attrs),
                windowClass: attrs.windowClass
            });
            scope.$modalInstance = modalInstance;
        };

        return {
            restrict: 'A',
            link: function (scope, elem, attrs) {
                elem.on('remove', function () {
                    $('.' + attrs.windowClass).remove();
                    $('div.modal-backdrop').remove();
                });

                if (attrs.autoOpen) {
                    open(scope, attrs);
                }
                else {                    
                    $(attrs.openButton).click(function () {
                        open(scope, attrs);
                    });                    
                }
            }
        };
    }]);
})();