(function ($) {
    var designerModule = angular.module('designer', designerExtensions.dependencies);

    designerModule.config(designerExtensions.config);

    designerModule.directive('modal', function ($modal) {
        return {
            restrict: 'A',
            link: function (scope, elem, attrs) {
                var modalInstance = $modal.open({
                    backdrop: 'static',
                    templateUrl: 'dialog-template',
                    controller: 'DialogCtrl',
                    windowClass: "sf-designer-dlg"
                });
            }
        };
    });

    designerModule.controller('DialogCtrl', ['$scope', '$modalInstance', 'propertyService', 'widgetContext',
        function ($scope, $modalInstance, propertyService, widgetContext) {
            var ifSaveToAllTranslations = true;
            // ------------------------------------------------------------------------
            // Event handlers
            // ------------------------------------------------------------------------

            var onGetPropertiesSuccess = function (data) {
                if (typeof ($telerik) != "undefined")
                    $telerik.$(document).trigger("controlPropertiesLoad", [{ "Items": data.Items }]);
                $scope.ShowLoadingIndicator = false;
            };

            var onError = function (data, status, headers, config) {
                showError(data.Detail);
            }

            var dialogClose = function () {
                try {
                    $modalInstance.close()
                } catch (e) { }
                $scope.ShowLoadingIndicator = false;

                if (typeof ($telerik) != "undefined") {
                    $telerik.$(document).trigger("modalDialogClosed");
                }
            };

            // ------------------------------------------------------------------------
            // helper methods
            // ------------------------------------------------------------------------

            var saveProperties = function (modifiedProperties) {
                var saveMode = {
                    Default: 0,
                    AllTranslations: 1,
                    CurrentTranslationOnly: 2
                };

                var currentSaveMode = saveMode.Default;
                if (widgetContext.culture) {
                    currentSaveMode = ifSaveToAllTranslations ? saveMode.AllTranslations : saveMode.CurrentTranslationOnly;
                }

                propertyService.save(currentSaveMode, modifiedProperties).then(dialogClose, onError);
            };

            var showError = function(message){
                $scope.ShowError = true;
                if (message)
                    $scope.ErrorMessage = message;

                $scope.ShowLoadingIndicator = false;
            };

            // ------------------------------------------------------------------------
            // Scope variables and setup
            // ------------------------------------------------------------------------

            $scope.ShowLoadingIndicator = true;
            $scope.ShowError = false;

            //the save action - it will check which properties are changed and send only them to the server 
            $scope.Save = function (saveToAllTranslations) {
                ifSaveToAllTranslations = saveToAllTranslations;

                $scope.$broadcast('saveButtonPressed', null);

                $scope.ShowLoadingIndicator = true;

                var args = { Cancel: false };

                if (typeof ($telerik) != "undefined") {
                    propertyService.get().then(function (data) {
                        $telerik.$(document).trigger("controlPropertiesUpdating", [{ "Items": data.Items, "args": args }]);
                    }, onError);
                }

                if (!args.Cancel)
                    saveProperties();
            };

            $scope.Cancel = function () {
                if (typeof ($telerik) != "undefined") {
                    propertyService.get().then(function (data) {
                        $telerik.$(document).trigger("controlPropertiesUpdateCanceling", [{ "Items": data.Items }]);
                    }, onError);
                }
                propertyService.reset();
                dialogClose();
            };

            $scope.ShowSimpleButton = function (event) {
                $scope.IsSimpleVisible = true;
            };

            $scope.ChangeView = function (event) {
                $scope.IsSimpleVisible = !$scope.IsSimpleVisible;
            };

            $scope.HideSaveAllTranslations = widgetContext.hideSaveAllTranslations;

            propertyService.get().then(onGetPropertiesSuccess, onError);

            if (typeof ($telerik) != "undefined") {
                $telerik.$(document).one("controlPropertiesUpdate", function (e, params) {
                    if(params.Items)
                        saveProperties(params.Items);
                    if (params.error)
                        showError(params.error);
                    else
                        dialogClose();
                });
                $telerik.$(document).one("controlPropertiesLoaded", function (e, params) {
                    propertyService.set(params.Items);
                });
            }
        }
    ]);
})(jQuery);