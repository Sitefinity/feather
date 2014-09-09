(function ($) {
	angular.module('expander', [])
		.directive('expander', function () {
			return {
				restrict: 'EA',
				replace: true,
				transclude: true,
				scope: { title: '@expanderTitle' },
				templateUrl: sitefinity.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'Mvc/Scripts/Templates/expander.html'),
				link: function (scope, element, attrs) {
					scope.isExpanded = false;
					scope.classExpanded = 'notExpanded';

					scope.toggle = function toggle() {
						scope.isExpanded = !scope.isExpanded;
						if (scope.isExpanded)
							scope.classExpanded = 'glyphicon-chevron-down';
						else
							scope.classExpanded = 'glyphicon-chevron-right';
					};
				}
			};
		});
})(jQuery);