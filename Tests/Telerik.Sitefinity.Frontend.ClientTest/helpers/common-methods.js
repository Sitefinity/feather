/**
 * Global methods
 */

if (!String.prototype.format) {
    String.prototype.format = function () {
        var newStr = this;

        for (var i = 0; i < arguments.length; i++) {
            var pattern = new RegExp("\\{"+ i +"\\}", "g");
            newStr = newStr.replace(pattern, arguments[i]);
        }

        return newStr;
    }
};

var commonMethods = (function () {
    /**
     * Private methods and variables.
     */

    /**
     * Return public interface.
     */
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
        },

        /**
         * Mocks the serverContext's method getEmbeddedResourceUrl to return only the url without the assembly name.
         * This is needed if you want to load a directive's template from the $templateCache,
         * because the template's key is its url and the directive's property templateUrl is using the mocked method.
         */
        mockServerContextToEnableTemplateCache: function () {
            inject(function (serverContext) {
                serverContext.getEmbeddedResourceUrl = function (assembly, url) {
                    return url;
                };
            });
        }
    };
})();