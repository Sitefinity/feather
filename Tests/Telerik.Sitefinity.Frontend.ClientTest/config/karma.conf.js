module.exports = function (config) {
    config.set({
        basePath: '../',

        files: [
	              'helpers/jquery.min.js',
	              'helpers/angular.js',
                  'helpers/angular-route.js',
                  'helpers/angular-resource.js',
                  'helpers/angular-mocks.js',
                  'helpers/kendo.all.min.js',
                  'helpers/common-methods.js',
                  'helpers/jasmine-matchers.js',
                  '../../Telerik.Sitefinity.Frontend/Mvc/Scripts/Bootstrap/js/ui-bootstrap-tpls.min.js',

				  '../../Tests/Telerik.Sitefinity.Frontend.ClientTest/templates.js',
                  '../../Telerik.Sitefinity.Frontend/Designers/Scripts/*.js',
	              '../../Telerik.Sitefinity.Frontend/MVC/Scripts/Designer/*.js',
	              '../../Telerik.Sitefinity.Frontend/MVC/Scripts/*.js',
				  '../../Telerik.Sitefinity.Frontend/MVC/Scripts/ng-tags-input.min/ng-tags-input.min.js',
                  '../../Telerik.Sitefinity.Frontend/client-components/selectors/common/sf-services.js',
                  '../../Telerik.Sitefinity.Frontend/client-components/selectors/common/sf-selectors.js',
				  '../../Telerik.Sitefinity.Frontend/client-components/fields/sf-fields.js',
				  '../../Telerik.Sitefinity.Frontend/client-components/fields/**/*.js',
                  '../../Telerik.Sitefinity.Frontend/client-components/search/**/*.js',
                  '../../Telerik.Sitefinity.Frontend/client-components/selectors/common/sf-bubbles-selection.sf-cshtml',
                  '../../Telerik.Sitefinity.Frontend/client-components/selectors/common/sf-list-group-selection.sf-cshtml',
	              '../../Telerik.Sitefinity.Frontend/client-components/selectors/common/sf-list-selector.js',
                  '../../Telerik.Sitefinity.Frontend/client-components/selectors/common/sf-list-selector.sf-cshtml',
                  '../../Telerik.Sitefinity.Frontend/client-components/selectors/common/sf-items-tree.js',
                  '../../Telerik.Sitefinity.Frontend/client-components/selectors/common/sf-items-tree.html',
                  '../../Telerik.Sitefinity.Frontend/client-components/selectors/common/sf-selected-items-view.js',
                  '../../Telerik.Sitefinity.Frontend/client-components/selectors/common/sf-selected-items-view.sf-cshtml',
				  '../../Telerik.Sitefinity.Frontend/client-components/selectors/**/*.html',
				  '../../Telerik.Sitefinity.Frontend/client-components/selectors/**/*.js',
                  '../../Telerik.Sitefinity.Frontend/client-components/**/*.js',
                  'helpers/mocks/*.js',
                  'unit/**'
        ],

        exclude: [
              '../../Telerik.Sitefinity.Frontend/Mvc/Scripts/Angular/*.min.js',
	          '../../Telerik.Sitefinity.Frontend/Mvc/Scripts/Angular/angular-loader.js',
	          '../../Telerik.Sitefinity.Frontend/Mvc/Scripts/Angular/angular-scenario.js',
              '../../Telerik.Sitefinity.Frontend/Designers/Scripts/page-editor.js'
        ],

        preprocessors: {
            '../../Telerik.Sitefinity.Frontend/Designers/Scripts/*.js': 'coverage',
            '../../Telerik.Sitefinity.Frontend/Mvc/Scripts/*.js': 'coverage',
            '../../Telerik.Sitefinity.Frontend/Mvc/Scripts/Designer/*.js': 'coverage',
            '../../Telerik.Sitefinity.Frontend/client-components/**/*.html': ['ng-html2js']
        },

        //Converts directive's external html templates into javascript strings and stores them in the Angular's $templateCache service.
        ngHtml2JsPreprocessor: {
            // setting this option will create only a single module that contains templates
            // from all the files, so you can load them all with module('template')
            moduleName: 'templates',

            // Returns the id of the template in $templateCache. To get a template in a test use id like 'client-components/selectors/common/sf-list-selector.sf-cshtml'
            cacheIdFromPath: function (filepath) {
                // filepath is the path to the template on the disc 
                return filepath.split('Telerik.Sitefinity.Frontend/')[1];
            }
        },

        autoWatch: true,

        singleRun: true,

        frameworks: ['jasmine'],

        browsers: ['PhantomJS'],

        plugins: [
            'karma-junit-reporter',
            'karma-chrome-launcher',
            'karma-firefox-launcher',
            'karma-script-launcher',
            'karma-phantomjs-launcher',
            'karma-jasmine',
            'karma-coverage',
            'karma-ng-html2js-preprocessor'
        ],

        reporters: ['progress', 'junit', 'coverage'],

        junitReporter: {
            outputFile: 'test-results.xml'
        },

        coverageReporter: {
            type: 'html',
            dir: 'coverage/',
            file: 'coverage.xml'
        }
    });
};
