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

					scope.isExpanded = scope.startExpanded === 'true' || scope.startExpanded === 'True';

					scope.toggle = function toggle() {
					    scope.isExpanded = !scope.isExpanded;
					};
				}
			};
		});
})(jQuery);
