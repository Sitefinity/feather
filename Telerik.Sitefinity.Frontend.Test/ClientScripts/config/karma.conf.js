module.exports = function(config){
  config.set({
      basePath: '../../',

      files : [
	  'ClientScripts/helpers/jquery-1.8.3.min.js',
	  'ClientScripts/helpers/angular.js',
      'ClientScripts/helpers/angular-route.js',
      '../Telerik.Sitefinity.Frontend/Mvc/Scripts/Bootstrap/js/ui-bootstrap-tpls-0.10.0.min.js',
      '../Telerik.Sitefinity.Frontend/Mvc/Scripts/ModalDialogModule.js',
      '../Telerik.Sitefinity.Frontend/Mvc/Scripts/*.js',
      'ClientScripts/unit/*.js',
      'ClientScripts/helpers/PageControlDataServiceMock.js',
      'ClientScripts/helpers/PropertyDataServiceMock.js'
    ],

    exclude : [
      '../Telerik.Sitefinity.Frontend/Mvc/Scripts/Angular/*.min.js',
	  '../Telerik.Sitefinity.Frontend/Mvc/Scripts/Angular/angular-loader.js',
	  '../Telerik.Sitefinity.Frontend/Mvc/Scripts/Angular/angular-scenario.js'
    ],

    autoWatch: true,

    singleRun: true,

    frameworks: ['jasmine'],

    browsers: ['PhantomJS'],

    plugins : [
            'karma-junit-reporter',
            'karma-chrome-launcher',
            'karma-firefox-launcher',
            'karma-script-launcher',
            'karma-phantomjs-launcher',
            'karma-jasmine'
            ],
			
	reporters: ['progress', 'junit'],

    junitReporter : {
        outputFile: 'test-results.xml'
    }
  });
};
