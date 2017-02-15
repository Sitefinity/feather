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
		            scope.customSelectedClass = null;
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

                    scope.$watch(
                        'selectedClass',
                        function (newSelectedClass, oldSelectedClass) {
                            if (newSelectedClass !== oldSelectedClass) {
                                if ((scope.customSelectedClass !== newSelectedClass) && !scope.getElementByPropValue(scope.cssClasses, 'value', newSelectedClass))
                                    scope.customSelectedClass = newSelectedClass;
                            }
                        },
                        true
                    );

                    scope.getElementByPropValue = function (arr, prop, value) {
                        if (arr && prop && value) {
                            for (var index in arr) {
		                        if (arr[index][prop] === value)
		                            return arr[index];
		                    }
                        }

		                return null;
                    };

		            try {
		                allCssClasses = $injector.get('cssClasses');

		                if (scope.viewName && allCssClasses) 
		                    scope.cssClasses = allCssClasses[scope.viewName];

		                if (scope.selectedClass && !scope.getElementByPropValue(scope.cssClasses, 'value', scope.selectedClass))
		                    scope.customSelectedClass = scope.selectedClass;

		            } catch (e) { }
		        }
		    };
		}]);
})(jQuery);
