var sfSelectors = angular.module('sfSelectors');
    sfSelectors.requires.push('sfVideoSelector');

var sfVideoSelector = angular.module('sfVideoSelector', ['sfMediaSelector', 'serverDataModule']);

sfVideoSelector.directive('sfVideoSelector',
    ['serverContext', 'serverData',
    function (serverContext, serverData) {
    return {
        restrict: 'E',
        scope: {
            selectedItems: '=?sfModel',
            filterObject: '=?sfFilter',
            provider: '=?sfProvider',
            sfMultiselect: '@',
            sfDeselectable: '@',
        },
        templateUrl: function (elem, attrs) {
            var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
            var url = attrs.sfTemplateUrl || 'client-components/selectors/media/videos/sf-video-selector.sf-cshtml';
            return serverContext.getEmbeddedResourceUrl(assembly, url);
        },
        link: function (scope, element, attrs) {
            serverData.refresh();
            scope.labels = serverData.getAll();

            // Ensures that modal is styled correctly.

            element.parents('.modal[role=dialog]').first().addClass('modal-fluid');
        }
    };
}]);