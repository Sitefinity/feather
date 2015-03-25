(function ($) {
    angular.module('designer')
		.directive('styleDropdown', ['$injector', function ($injector) {
		    return {
		        restrict: 'EA',
		        scope: {
		            selectedClass: '=',
		            viewName: '='
		        },
		        templateUrl: sitefinity.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'Mvc/Scripts/Templates/style-dropdown.sf-cshtml'),
		        link: function (scope, element, attrs) {
		            scope.cssClasses = [];
		            var allCssClasses;

		            scope.$watch(
                        'viewName',
                        function (newViewName, oldViewName) {
                            if ((newViewName !== oldViewName) && allCssClasses) {
                                scope.cssClasses = allCssClasses[newViewName];
                            }
                        },
                        true
                    );

		            try {
		                allCssClasses = $injector.get('cssClasses');

		                if (scope.viewName && allCssClasses)
		                    scope.cssClasses = allCssClasses[scope.viewName];
		            } catch (e) { }
		        }
		    };
		}]);
})(jQuery);