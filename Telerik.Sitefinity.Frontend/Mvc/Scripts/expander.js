(function ($) {
	angular.module('expander', [])
		.directive('expander', function () {
			return {
				restrict: 'EA',
				replace: true,
				transclude: true,
				scope: { title: '@expanderTitle', startExpanded: '@startExpanded' },
				templateUrl: sitefinity.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'Mvc/Scripts/Templates/expander.html'),
				link: function (scope, element, attrs) {
				    var getClass = function (isExpanded) {
				        if (isExpanded)
				            return 'glyphicon-chevron-down';
				        else
				            return 'glyphicon-chevron-right';
				    };

				    scope.isExpanded = scope.startExpanded === 'true' || scope.startExpanded === 'True';
					scope.classExpanded = getClass(scope.isExpanded);

					scope.toggle = function toggle() {
						scope.isExpanded = !scope.isExpanded;
						scope.classExpanded = getClass(scope.isExpanded);
					};
				}
			};
		});
})(jQuery);