﻿
(function ($) {
    if (typeof ($telerik) != 'undefined') {
        $telerik.$(document).one('dialogRendered', function () {
            angular.bootstrap(document.querySelector('.personalization-designer'), ['personalizationDesigner']);
        });
    }

    var endsWith = function (str, suffix) {
        return str.indexOf(suffix, str.length - suffix.length) !== -1;
    };

    var personalizationDesignerModule = angular.module('personalizationDesigner', ['pageEditorServices', 'modalDialog', 'serverDataModule', 'personalizationServices']);

    personalizationDesignerModule.controller('personalizationDialogCtrl', ['$rootScope', '$scope', '$q', '$filter', '$uibModalInstance', 'serverData', 'personalizationService',
        function ($rootScope, $scope, $q, $filter, $uibModalInstance, serverData, personalizationService) {

            // ------------------------------------------------------------------------
            var onError = function (errorData) {
                $scope.feedback.showError = true;
                if (errorData && errorData.data)
                    $scope.feedback.errorMessage = errorData.data.Detail;
            };

            var setSegmentName = function () {
                var selectedSegments = $telerik.$.grep($scope.segments, function (value, index) {
                    return value.Id === $scope.model.segmentId;
                });

                if (selectedSegments && selectedSegments.length === 1) {
                    $scope.model.segmentName = selectedSegments[0].Name;
                }
            };

            $scope.model = {
                controlId: "",
                segmentId: ""
            };

            $scope.segments = [];
            $scope.filteredSegments = [];
            personalizationService.getSegments($scope.model).then(function (data) {
                $scope.segments = data;
                $scope.filteredSegments = $filter('filter')($scope.segments, { IsAvailable: false });
                if ($scope.filteredSegments && $scope.filteredSegments.length > 0) {
                    $scope.model.segmentId = $scope.filteredSegments[0].Id;
                }

                $scope.feedback.showLoadingIndicator = false;
            });

            $scope.goToCreateSegment = function () {
                var newSegmentUrl = "Sitefinity/Marketing/Personalization?action=newSegment";
                var newSegmentRootedUrl = sitefinity.getRootedUrl(newSegmentUrl);

                window.location.href = newSegmentRootedUrl;
            };

            $scope.addPersonalization = function () {
                $scope.feedback.showLoadingIndicator = true;
                setSegmentName();

                personalizationService.personalize($scope.model)
                    .then(function (args) {
                        $scope.close(true);
                        $telerik.$(document).trigger('personalizationDialogClosed', args);
                    }, onError)
                    .finally(function () {
                        $scope.feedback.showLoadingIndicator = false;
                    });
            };

            $scope.feedback = {
                showLoadingIndicator: true,
                showError: false
            };

            $scope.cancel = function () {
                var canceling = $q.defer();
                canceling.promise
                    .then(function () {
                        $scope.close(true);
                    })
                    .catch(onError)
                    .finally(function () {
                        $scope.feedback.showLoadingIndicator = false;
                    });

                $scope.feedback.showLoadingIndicator = true;
                canceling.resolve();
            };

             $scope.close = function (fireModalClosedEvent) {
                try {
                    $uibModalInstance.close();
                } catch (e) { }

                if (typeof ($telerik) !== 'undefined' && fireModalClosedEvent)
                    $telerik.$(document).trigger('modalDialogClosed');

                if (typeof CustomEvent == "function") {
                    var evt = new CustomEvent('sfModalDialogClosed');
                    document.dispatchEvent(evt);
                }
            };

            $scope.hideError = function () {
                $scope.feedback.showError = false;
                $scope.feedback.errorMessage = null;
            };
        }
    ]);
})(jQuery);