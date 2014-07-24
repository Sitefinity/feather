(function () {
    var designerModule = angular.module('designer');

    //basic controller for the advanced designer view
    designerModule.controller('PropertyGridCtrl', ['$scope', 'propertyService', 'dialogFeedbackService',
        function ($scope, propertyService, dialogFeedbackService) {

            var onGetPropertiesSuccess = function (data) {
                if (data.Items)
                    $scope.Items = data.Items;
            };

            var onGetError = function (data) {
                $scope.Feedback.ShowError = true;
                if (data)
                    $scope.Feedback.ErrorMessage = data.Detail;
            };

            propertyService.get().then(onGetPropertiesSuccess, onGetError)
                .finally(function () {
                    $scope.Feedback.ShowLoadingIndicator = false;
                });

            $scope.Feedback = dialogFeedbackService;
            $scope.Feedback.ShowLoadingIndicator = true;

            $scope.DrillDownPropertyHierarchy = function (propertyPath, propertyName) {
                $scope.propertyPath = propertyPath;
                $scope.propertyName = propertyName;
            };
        }]);

    //filters property hierarchy
    designerModule.filter('propertyHierarchy', function () {

        return function (inputArray, propertyName, propertyPath) {
            var currentLevel = 0;
            if (propertyPath)
                currentLevel = propertyPath.split('/').length - 1;

            var levelFilter = function (property) {
                var level = property.PropertyPath.split('/').length - 2;
                if (!propertyName || propertyName == 'Home') {
                    return level <= currentLevel;
                } else {
                    if (property.PropertyPath.indexOf(propertyPath) >= 0) {
                        return currentLevel == level;
                    } else {
                        return false;
                    }
                }
            };

            var proxyFilter = function (property) {
                if (property.IsProxy)
                    return false;
                else {
                    if (!currentLevel)
                        currentLevel = 1;

                    return levelFilter(property);
                }
            };

            var result = inputArray;
            if (inputArray && inputArray[0].IsProxy)
                result = inputArray.filter(proxyFilter);
            else if (inputArray)
                result = inputArray.filter(levelFilter);

            return result;
        };

    });

})();