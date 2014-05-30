(function ($) {
	angular.module('expander', [])
		.directive('expander', function () {
			return {
				restrict: 'EA',
				replace: true,
				transclude: true,
				scope: { title: '@expanderTitle' },
				template: '<div class="s-more-options-wrp">' +
					'<span class="s-title" ng-click="toggle()"><span ng-class="classExpanded" class="glyphicon glyphicon-chevron-right"></span> {{title}}</span>' +
					'<div class="s-body" ng-show="isExpanded" ng-transclude></div>' +
					'</div>',
				link: function (scope, element, attrs) {
					scope.isExpanded = false;
					scope.classExpanded = "notExpanded";

					scope.toggle = function toggle() {
						scope.isExpanded = !scope.isExpanded;
						if (scope.isExpanded)
							scope.classExpanded = "glyphicon-chevron-down";
						else
							scope.classExpanded = "glyphicon-chevron-right";
					}
				}
			}
		});
})(jQuery);