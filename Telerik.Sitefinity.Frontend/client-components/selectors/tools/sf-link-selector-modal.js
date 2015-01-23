/*
     * Link Selector Modal directive wraps the Link selector in a modal dialog. It is a 
     * convenience thin wrapper which can be used to open the Link Selector in a modal dialog.
     */

angular.module('sfSelectors').directive('sfLinkSelectorModal', function ($injector) {

    var serverContext = $injector.get('serverContext');
    var linkService = $injector.get('sfLinkService');
    var linkMode = $injector.get("sfLinkMode");

    var link = function ($scope) {

        $scope.insertLink = function () {
            var selectedItem = angular.element("#linkSelector").scope().selectedItem;
            var htmlLinkObj = linkService.getHtmlLink(selectedItem);
            $scope.$emit('selectedHtmlChanged', htmlLinkObj[0]);
            $scope.$modalInstance.close();
        };

        $scope.cancel = function () {
            $scope.$modalInstance.close();
        };

        $scope.isDisabled = function (selectedItem) {
            if (selectedItem) {
                if (selectedItem.mode == linkMode.WebAddress) {
                    return isEmpty(selectedItem.displayText) || isEmpty(selectedItem.webAddress, "http://");
                }
                else if (selectedItem.mode == linkMode.InternalPage) {
                    return isEmpty(selectedItem.displayText) || isEmpty(selectedItem.pageId);
                }
                else if (selectedItem.mode == linkMode.Anchor) {
                    return isEmpty(selectedItem.displayText) || isEmpty(selectedItem.selectedAnchor);
                }
                else {
                    return isEmpty(selectedItem.displayText) || isEmpty(selectedItem.emailAddress);
                }
            }

            return true;
        };

        isEmpty = function (value, initialValue) {
            if (initialValue) {
                return value == null || value.length === 0 || value == initialValue;
            }
            else {
                return value == null || value.length === 0;
            }
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