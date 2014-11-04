(function ($) {
    angular.module('designer')
    .directive('sfLoading', [function () {
        return {
            restrict: 'EA',
            replace: true,
            templateUrl: sitefinity.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'Mvc/Scripts/Templates/sf-loading.html')
        };
    }]);
})(jQuery);
