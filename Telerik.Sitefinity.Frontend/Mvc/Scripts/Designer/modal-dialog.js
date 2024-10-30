﻿/* global angular */
(function ($) {
    var modalDialogModule = angular.module('modalDialog', ['ui.bootstrap']);

    //Keeps track of the number of opened modal dialogs.
    modalDialogModule.factory('dialogsService', function () {
        var dialogSelectors = [];
        return {
            pushSelector: function (selector) {
                dialogSelectors.push(selector);
            },
            pop: function () {
                if (dialogSelectors.length)
                    return $(dialogSelectors.pop());
                else
                    return $();
            },
            peek: function () {
                if (dialogSelectors.length)
                    return $(dialogSelectors[dialogSelectors.length - 1]);
                else
                    return $();
            },
            count: function () {
                return dialogSelectors.length;
            }
        };
    });

    modalDialogModule.directive('modal', ['$uibModal', 'dialogsService', function ($uibModal, dialogsService) {
        var resolveControllerName = function (attrs) {
            if (!attrs.dialogController && !attrs.existingScope) {
                throw 'Please either insert an attribute named "dialog-controller" with the name of the controller for the modal dialog next to the "modal" directive ' +
                'or insert attribute named "existing-scope" to reuse the current scope in the dialog.';
            }

            return attrs.dialogController;
        };

        var modalDialogClass = 'modal-dialog-';
        var backdropClass = 'div.modal-backdrop';

        var open = function (scope, attrs, resolve, onRenderedCb) {
            dialogsService.peek().hide();

            var uniqueClass = modalDialogClass + dialogsService.count();
            var windowClass = attrs.windowClass ? attrs.windowClass + ' ' + uniqueClass : uniqueClass;
            var modalInstance = $uibModal.open({
                backdrop: 'static',
                scope: attrs.existingScope && scope,
                templateUrl: attrs.templateUrl,
                controller: resolveControllerName(attrs),
                windowClass: windowClass,
                resolve: resolve
            });

            scope.$uibModalInstance = modalInstance;

            dialogsService.pushSelector('.' + uniqueClass);

            if (typeof onRenderedCb === 'function') {
                scope.$uibModalInstance.rendered.then(function (res) {
                    onRenderedCb(res);
                    return res;
                });
            }

            return scope.$uibModalInstance.result.finally(function () {
                dialogsService.pop().remove();

                if (dialogsService.count() > 0) {
                    dialogsService.peek().show();
                }
                else {
                    $(backdropClass).remove();
                }
            })
            .catch(function () {
            });
        };

        return {
            restrict: 'A',
            link: function (scope, elem, attrs) {
                if (attrs.autoOpen) {
                    open(scope, attrs);
                }
                else {
                    scope.$openModalDialog = function (resolve, onRenderedCb) {
                        return open(scope, attrs, resolve, onRenderedCb);
                    };
                }
            }
        };
    }]);
})(jQuery);
