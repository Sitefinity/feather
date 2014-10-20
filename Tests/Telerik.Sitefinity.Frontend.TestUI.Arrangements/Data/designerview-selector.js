(function ($) {
    var designerModule = angular.module('designer');

    angular.module('designer').requires.push('selectors');

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

        $scope.itemType = "Telerik.Sitefinity.DynamicTypes.Model.DuplicateRelatedDataModule.Duplicaterelateddata";

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
    }]);
})(jQuery);