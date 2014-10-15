/* global angular */
(function () {
    var modalDialogModule = angular.module('modalDialog', ['ui.bootstrap']);

    //Keeps track of the number of opened modal dialogs.
    modalDialogModule.factory('dialogsService', function () {
        var openedDialogsCount = 0;
        return {
            increaseDialogsCount: function () {
                openedDialogsCount++;
            },
            decreaseDialogsCount: function () {
                openedDialogsCount--;
            },
            getOpenedDialogsCount: function () {
                return openedDialogsCount;
            }
        };
    });

    modalDialogModule.directive('modal', ['$modal', 'dialogsService', function ($modal, dialogsService) {
        var resolveControllerName = function (attrs) {
            if (!attrs.dialogController && !attrs.existingScope) {
                throw 'Please either insert an attribute named "dialog-controller" with the name of the controller for the modal dialog next to the "modal" directive ' +
                'or insert attribute named "existing-scope" to reuse the current scope in the dialog.';
            }

            return attrs.dialogController;
        };

        var open = function (scope, attrs) {
            //It will be used for identifying the opened window in order to be removed from the DOM when the dialog is closed.
            //It is set as a class because of a limitation in the angular-bootstrap directive.
            var modalWindowId = 'id' + Math.floor((Math.random() * 1000) + 1);

            //Hide already opened dialogs.
            $(".modal-dialog").hide();

            var modalInstance = $modal.open({
                backdrop: 'static',
                scope: attrs.existingScope && scope,
                templateUrl: attrs.templateUrl,
                controller: resolveControllerName(attrs),
                windowClass: attrs.windowClass + ' ' + modalWindowId
            });

            scope.$modalInstance = modalInstance;

            dialogsService.increaseDialogsCount();

            scope.$modalInstance.result.finally(function () {
                dialogsService.decreaseDialogsCount();

                $('.' + modalWindowId).remove();

                if (dialogsService.getOpenedDialogsCount() > 0) {
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
                    // The open button selector attribute can be binded to a property of the scope.
                    attrs.$observe("openButton", function (value) {
                        $(document).off("click", value);
                        $(document).on("click", value, function () {
                            open(scope, attrs);
                        });
                    });
                }
            }
        };
    }]);
})();
