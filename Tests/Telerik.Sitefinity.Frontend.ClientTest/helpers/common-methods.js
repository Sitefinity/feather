/**
 * Global methods
 */

if (!String.prototype.format) {
    /**
     * A format method, attached to the String.prototype.
     * Allows string formating with given params.
     * When called on a string, all occurrences of '{n}' n=0,1,2... will be replaced with the params.
     * @return {String} the formatted string
     */
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
        /**
         * Compiles the given template with the given scope
         * and inserts the produced html in the given container marked with a css class.
         * @param  {String} template  the raw Angular template
         * @param  {Object} scope     the scope that will be applied to the template
         * @param  {String} container jQuery selector that will contain the produced html
         * @param  {String} cssClass  class that will mark the container
         * @return {undefined}
         */
        compileDirective: function (template, scope, container, cssClass) {
            var container = container || 'body';
            var cssClass = cssClass || 'testDiv';
            var directiveElement = null;

            inject(function ($compile) {
                directiveElement = $compile(template)(scope);
                $(container).append($('<div/>').addClass(cssClass)
                    .append(directiveElement));
            });

            // $digest is necessary to finalize the directive generation
            scope.$digest();

            return directiveElement;
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