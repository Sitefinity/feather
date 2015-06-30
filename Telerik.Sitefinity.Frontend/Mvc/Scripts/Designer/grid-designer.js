
(function ($) {
    if (typeof ($telerik) != 'undefined') {
        $telerik.$(document).one('dialogRendered', function () {
            angular.bootstrap($('.grid-designer'), ['gridDesigner']);
        });
    }

    var endsWith = function (str, suffix) {
        return str.indexOf(suffix, str.length - suffix.length) !== -1;
    };

    var gridDesignerModule = angular.module('gridDesigner', ['pageEditorServices', 'modalDialog', 'serverDataModule']);

    gridDesignerModule.controller('GridDialogCtrl', ['$rootScope', '$scope', '$q', '$modalInstance', 'serverData', 'gridService',
    function ($rootScope, $scope, $q, $modalInstance, serverData, gridService) {

        // ------------------------------------------------------------------------
        // Event handlers
        // ------------------------------------------------------------------------

        var onError = function (data) {
            $scope.feedback.showError = true;
            if (data)
                $scope.feedback.errorMessage = data.Detail ? data.Detail : data;

            $scope.feedback.showLoadingIndicator = false;
        };

        // ------------------------------------------------------------------------
        // helper methods
        // ------------------------------------------------------------------------

        var saveHtml = function () {

            return gridService.save($scope.gridElements, serverData.get("updateServiceUrl"));
        };

        // ------------------------------------------------------------------------
        // Scope variables and setup
        // ------------------------------------------------------------------------

        $scope.feedback = {
            showLoadingIndicator: true,
            showError: false
        };

        //the save action - it will check which properties are changed and send only them to the server 
        $scope.save = function () {

            var saving = $q.defer();
            saving.promise
                .then(saveHtml)
                .then($scope.close)
                .catch(onError)
                .finally(function () {
                    $scope.feedback.showLoadingIndicator = false;
                });

            $scope.feedback.showLoadingIndicator = true;
            saving.resolve();
        };

        $scope.cancel = function () {
            var canceling = $q.defer();
            canceling.promise
                .then(function () {
                    $scope.close();
                })
                .catch(onError)
                .finally(function () {
                    $scope.feedback.showLoadingIndicator = false;
                });

            $scope.feedback.showLoadingIndicator = true;
            canceling.resolve();
        };

        $scope.close = function () {
            try {
                $modalInstance.close();
            } catch (e) { }

            if (typeof ($telerik) != 'undefined')
                $telerik.$(document).trigger('gridModalDialogClosed');
        };

        $scope.hideError = function () {
            $scope.feedback.showError = false;
            $scope.feedback.errorMessage = null;
        };


        gridService.get().then(function (data) {
            if (data) {
                $scope.gridElements = data;
                $scope.feedback.showLoadingIndicator = false;
            }
        },
            onError);
    }
    ]);
})(jQuery);