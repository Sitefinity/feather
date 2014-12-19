var commonMethods = (function () {
	return {
		compileDirective: function (template, scope, container, cssClass) {
			var container = container || 'body';
			var cssClass = cssClass || 'testDiv';

	        inject(function ($compile) {
	            var directiveElement = $compile(template)(scope);
	            $(container).append($('<div/>').addClass(cssClass)
	                .append(directiveElement));
	        });

	        // $digest is necessary to finalize the directive generation
	        scope.$digest();
		}
	};
})();