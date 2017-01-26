var designerModule = angular.module('designer2', ['ngSanitize']);
designerModule.requires.push('sfFields');
designerModule.requires.push('sfSelectors');

angular.module('designer').requires.push('designer2');

designerModule.controller('SimpleCtrl', ['$scope', 'propertyService', function ($scope, propertyService) {
    $scope.selectedMediaId = null;
    $scope.selectedMedia = null;
    $scope.mediaProvider = null;

    $scope.feedback.showLoadingIndicator = true;

    propertyService.get()
        .then(function (data) {
            if (data) {
                $scope.properties = propertyService.toAssociativeArray(data.Items);
            }
        },
        function (data) {
            $scope.feedback.showError = true;
            if (data)
                $scope.feedback.errorMessage = data.Detail;
        })
        .finally(function () {
            $scope.feedback.showLoadingIndicator = false;
        });

    $scope.$watch('properties.SelectedMedia.PropertyValue', function (newValue, oldValue) {
        if (newValue) {
            $scope.selectedMedia = JSON.parse(newValue);
        }
    });

    $scope.$watch('selectedMedia', function (newValue, oldValue) {
        if (newValue) {
            $scope.properties.SelectedMedia.PropertyValue = JSON.stringify(newValue);
        }
    });

    $scope.$watch('properties.SelectedMediaId.PropertyValue', function (newValue, oldValue) {
        if (newValue) {
            $scope.selectedMediaId = newValue;
        }
    });

    $scope.$watch('selectedMediaId', function (newValue, oldValue) {
        if (newValue) {
            $scope.properties.SelectedMediaId.PropertyValue = newValue;
        }
    });

    $scope.$watch('properties.MediaProvider.PropertyValue', function (newValue, oldValue) {
        if (newValue) {
            $scope.mediaProvider = newValue;
        }
    });

    $scope.$watch('mediaProvider', function (newValue, oldValue) {
        if (newValue) {
            $scope.properties.MediaProvider.PropertyValue = newValue;
        }
    });
}])