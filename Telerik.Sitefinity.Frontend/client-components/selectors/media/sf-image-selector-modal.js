/*
* Image Selector Modal directive wraps the Image Selector in a modal dialog. It is a 
* convenience thin wrapper which can be used to open the Image Selector in a modal dialog.
*/
var sfSelectors = angular.module('sfSelectors');
sfSelectors.requires.push('sfImageSelectorModal');

angular.module('sfImageSelectorModal', ['sfServices', 'sfImageSelector'])
    .directive('sfImageSelectorModal', ['serverContext', function (serverContext) {

    var link = function ($scope) {

        $scope.insertImage = function () {
            $scope.$modalInstance.close();
        };

        $scope.cancel = function () {
            $scope.$modalInstance.close();
        };
    };

    return {
        restrict: 'E',
        templateUrl: function (elem, attrs) {
            var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
            var url = attrs.sfTemplateUrl || 'client-components/selectors/media/sf-image-selector-modal.html';
            return serverContext.getEmbeddedResourceUrl(assembly, url);
        },
        scope: true,
        link: link
    };
}]);