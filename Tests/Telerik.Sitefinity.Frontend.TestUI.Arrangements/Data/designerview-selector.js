(function ($) {
    var designerModule = angular.module('designer');

    angular.module('designer').requires.push('sfSelectors');

    //This is basic controller for the "ManageItems" designer view.
    designerModule.controller('SelectorCtrl', ['$scope', 'propertyService', function ($scope, propertyService) {
        $scope.feedback.showLoadingIndicator = true;
        $scope.$watch("selectedNewsItem.Title", function(newValue){
            $scope.properties["DummyText"].PropertyValue = newValue;
        });

        $scope.$watch("selectedTaxonItem.Title", function (newValue) {
            $scope.properties["DummyText"].PropertyValue = newValue;
        });

        $scope.$watch("selectedDynamicItem.Title.Value", function (newValue) {
            $scope.properties["DummyText"].PropertyValue = newValue;
        });

        $scope.$watch('properties.SelectedIdsNewsItems.PropertyValue', function (newValue, oldValue) {
            $scope.SelectedIdsNewsItems = newValue ? newValue.split(',') : null;
        });

        $scope.$watch('SelectedIdsNewsItems', function (newValue, oldValue) {
            $scope.properties.SelectedIdsNewsItems.PropertyValue = newValue ? newValue.join(',') : null;
        });

        $scope.$watch('properties.SelectedIdsTags.PropertyValue', function (newValue, oldValue) {
            $scope.SelectedIdsTags = newValue ? newValue.split(',') : null;
        });

        $scope.$watch('SelectedIdsTags', function (newValue, oldValue) {
            $scope.properties.SelectedIdsTags.PropertyValue = newValue ? newValue.join(',') : null;
        });

        $scope.$watch('properties.SelectedIdsDynamicItems.PropertyValue', function (newValue, oldValue) {
            $scope.SelectedIdsDynamicItems = newValue ? newValue.split(',') : null;
        });

        $scope.$watch('SelectedIdsDynamicItems', function (newValue, oldValue) {
            $scope.properties.SelectedIdsDynamicItems.PropertyValue = newValue ? newValue.join(',') : null;
        });

        $scope.itemType = "Telerik.Sitefinity.DynamicTypes.Model.DuplicateRelatedDataModule.Duplicaterelateddata";

        $scope.$watch('properties.Ids.PropertyValue', function (newValue, oldValue) {
            if (newValue) {
                $scope.ids = newValue.split(',');
            }
        });
        $scope.$watch('ids', function (newValue, oldValue) {
            if (newValue) {
                $scope.properties.Ids.PropertyValue = newValue.join(',');
            }
        });

        propertyService.get()
            .then(function (data) {
                if (data) {
                    $scope.properties = propertyService.toAssociativeArray(data.Items);
                    $scope.selectedItem = $scope.properties.SelectedNews.PropertyValue;
                    $scope.testDynamicModules = $scope.properties.TestDynamicModules.PropertyValue;              
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
    }]);
})(jQuery);