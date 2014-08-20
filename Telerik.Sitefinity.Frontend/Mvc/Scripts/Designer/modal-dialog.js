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
            //Hide already opened dialogs.
            $(".modal-dialog").hide();

            var modalInstance = $modal.open({
                backdrop: 'static',
                scope: attrs.existingScope && scope,
                templateUrl: attrs.templateUrl,
                controller: resolveControllerName(attrs),
                windowClass: attrs.windowClass
            });

            scope.$modalInstance = modalInstance;

            scope.$modalInstance.result.finally(function () {
                if ($('.' + attrs.windowClass).length <= 1) {                    
                    //Another dialog uses the same class so don't remove it.
                    $('.' + attrs.windowClass).remove();
                }

                if ($(".modal-dialog").length > 0) {
                    //There is another dialog except this one. Show it and keep the backdrop.
                    $(".modal-dialog").show(); 
                }
                else {
                    $('div.modal-backdrop').remove();
                }                
            });
        };

        return {
            restrict: 'A',
            link: function (scope, elem, attrs) {
                if (attrs.autoOpen) {
                    open(scope, attrs);
                }
                else {                    
                    $(document).on("click", attrs.openButton, function () {
                        open(scope, attrs);
                    });                    
                }
            }
        };
    }]);
})();