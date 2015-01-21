/*
     * Link Selector Modal directive wraps the Link selector in a modal dialog. It is a 
     * convenience thin wrapper which can be used to open the Link Selector in a modal dialog.
     */

angular.module('sfSelectors').directive('sfLinkSelectorModal', function ($injector) {

    var serverContext = $injector.get('serverContext');
    var linkService = $injector.get('sfLinkService');

    var link = function ($scope) {

        $scope.insertLink = function () {
            var selectedItem = angular.element("#linkSelector").scope().selectedItem;
            var htmlLinkObj = linkService.getHtmlLink(selectedItem);
            $scope.selectedHtml = htmlLinkObj[0];
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
            var url = attrs.sfTemplateUrl || 'client-components/selectors/tools/sf-link-selector-modal.html';
            return serverContext.getEmbeddedResourceUrl(assembly, url);
        },
        scope: true,
        link: link
    };

});