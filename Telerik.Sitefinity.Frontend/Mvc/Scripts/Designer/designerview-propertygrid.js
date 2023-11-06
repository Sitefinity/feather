﻿(function () {
    var propertyGridModule = angular.module('PropertyGridModule', ['designer', 'pageEditorServices', 'breadcrumb']);
    angular.module('designer').requires.push('PropertyGridModule');

    //basic controller for the advanced designer view
    propertyGridModule.controller('PropertyGridCtrl', ['$scope', 'propertyService',
        function ($scope, propertyService) {

            var onGetPropertiesSuccess = function (data) {
                if (data && data.Items)
                    $scope.items = data.Items;
            };

            var onGetError = function (errorData) {
                $scope.feedback.showError = true;
                if (errorData && errorData.data)
                    $scope.feedback.errorMessage = errorData.data.Detail;
            };

            propertyService.get().then(onGetPropertiesSuccess, onGetError)
                .finally(function () {
                    $scope.feedback.showLoadingIndicator = false;
                });

            $scope.feedback.showLoadingIndicator = true;

            $scope.drillDownPropertyHierarchy = function (propertyPath, propertyName) {
                $scope.propertyPath = propertyPath;
                $scope.propertyName = propertyName;
            };

            $scope.isUndefined = function (value) {
                return typeof value === 'undefined';
            };
        }]);

    //filters property hierarchy
    propertyGridModule.filter('propertyHierarchy', function () {

        return function (inputArray, propertyName, propertyPath) {
            var currentLevel = 0;
            if (propertyPath)
                currentLevel = propertyPath.split('/').length - 1;

            var levelFilter = function (property) {
                var level = property.PropertyPath.split('/').length - 2;
                if (currentLevel == 1 && property.PropertyName == 'FieldName' && property.PropertyPath.indexOf("MetaField") > -1) {
                    return true;
                }

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