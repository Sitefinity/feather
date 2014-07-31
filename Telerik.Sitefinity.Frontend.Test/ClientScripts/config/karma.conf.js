module.exports = function(config){
  config.set({
      basePath: '../../',

      files : [
	  'ClientScripts/helpers/jquery-1.8.3.min.js',
	  'ClientScripts/helpers/angular.js',
      'ClientScripts/helpers/angular-route.js',
      'ClientScripts/helpers/angular-mocks.js',
      '../Telerik.Sitefinity.Frontend/Mvc/Scripts/Bootstrap/js/ui-bootstrap-tpls-0.11.0.min.js',
      '../Telerik.Sitefinity.Frontend/Designers/Scripts/*.js',
      '../Telerik.Sitefinity.Frontend/Mvc/Scripts/*.js',
      '../Telerik.Sitefinity.Frontend/Mvc/Scripts/Designer/*.js',
      'ClientScripts/helpers/mocks/*.js',
      'ClientScripts/unit/*.js',
      'ClientScripts/unit/Designer/*.js'
    ],

    exclude : [
      '../Telerik.Sitefinity.Frontend/Mvc/Scripts/Angular/*.min.js',
	  '../Telerik.Sitefinity.Frontend/Mvc/Scripts/Angular/angular-loader.js',
	  '../Telerik.Sitefinity.Frontend/Mvc/Scripts/Angular/angular-scenario.js',
      '../Telerik.Sitefinity.Frontend/Designers/Scripts/page-editor.js'
    ],

    preprocessors : {
        '../Telerik.Sitefinity.Frontend/Designers/Scripts/*.js': 'coverage',
        '../Telerik.Sitefinity.Frontend/Mvc/Scripts/*.js': 'coverage',
        '../Telerik.Sitefinity.Frontend/Mvc/Scripts/Designer/*.js': 'coverage'
    },

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
            'karma-jasmine',
            'karma-coverage'
            ],
			
    reporters: ['progress', 'junit', 'coverage'],

    junitReporter : {
        outputFile: 'test-results.xml'
    },
      
    coverageReporter : {
        type: 'html',
        dir: 'coverage/',
        file: 'coverage.xml'
    }
  });
};
