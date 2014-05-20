
/**
 * Override this object to create a custom configuration for the designer AngularJs module or to register additional dependencies.
 */
var designerExtensions = {
    /**
     * Configuration function of the designer module.
     */
    config: function ($routeProvider) {
        $routeProvider
            //the route points to a MVC controller action that returns a proper view
            .when('/', {
                templateUrl: 'property-grid-view', controller: 'propertyGridCtrl'
            });
    },

    /**
     * Dependencies of the designer module.
     */
    dependencies: ['pageEditorServices', 'breadcrumb', 'ngRoute', 'modalDialog']
};