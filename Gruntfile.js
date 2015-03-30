module.exports = function (grunt) {
    'use strict';
	
	//Project Configuration
	grunt.initConfig({
		
		pkg: grunt.file.readJSON('package.json'),
		
		jshint: {
			//define the files to lint
			files: ['gruntfile.js',
					'Telerik.Sitefinity.Frontend/Designers/Scripts/*.js',
					'Telerik.Sitefinity.Frontend/MVC/Scripts/*.js',
					'Telerik.Sitefinity.Frontend/MVC/Scripts/Designer/*.js',
					'Telerik.Sitefinity.Frontend/client-components/**/*.js',
					'Tests/Telerik.Sitefinity.Frontend.ClientTest/unit/**/*.js'
			]
		},
		
		jasmine: {
			unit:{
				src: [
				'Tests/Telerik.Sitefinity.Frontend.ClientTest/templates.js',
				'Telerik.Sitefinity.Frontend/Designers/Scripts/*.js',
				'Telerik.Sitefinity.Frontend/MVC/Scripts/Designer/*.js',
				'Telerik.Sitefinity.Frontend/MVC/Scripts/*.js',
				'Telerik.Sitefinity.Frontend/client-components/selectors/common/sf-services.js',
				'Telerik.Sitefinity.Frontend/client-components/selectors/common/sf-selectors.js',
				'Telerik.Sitefinity.Frontend/client-components/selectors/common/sf-list-selector.js',
				'Telerik.Sitefinity.Frontend/client-components/selectors/common/sf-items-tree.js',
                'Telerik.Sitefinity.Frontend/client-components/selectors/common/sf-selected-items-view.js',
				'Telerik.Sitefinity.Frontend/client-components/fields/sf-fields.js',
				'Telerik.Sitefinity.Frontend/client-components/**/*.js',
				'!Telerik.Sitefinity.Frontend/Designers/Scripts/page-editor.js',
				'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/mocks/*.js'],
				options: {
					vendor:[
					'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/jquery.min.js',
					'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/angular.js',
					'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/angular-resource.js',
					'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/angular-route.js',
					'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/angular-mocks.js',
					'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/kendo.all.min.js',
					'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/common-methods.js',
					'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/jasmine-matchers.js',
					'Telerik.Sitefinity.Frontend/MVC/Scripts/Bootstrap/js/*.js',
					'!Telerik.Sitefinity.Frontend/Mvc/Scripts/Angular/**',
					'Telerik.Sitefinity.Frontend/MVC/Scripts/ng-tags-input.min/ng-tags-input.min.js'
					],
					specs: ['Tests/Telerik.Sitefinity.Frontend.ClientTest/unit/**/*.js'],
					junit: {
						path: 'Tests/Telerik.Sitefinity.Frontend.ClientTest/TestResults'
						},
					template: require('grunt-template-jasmine-istanbul'),
					templateOptions: {
						coverage: 'Tests/Telerik.Sitefinity.Frontend.ClientTest/coverage/coverage.json',
						report: [
							{type: 'html', options: {dir: 'Tests/Telerik.Sitefinity.Frontend.ClientTest/coverage'}},
							{type: 'cobertura', options: {dir: 'Tests/Telerik.Sitefinity.Frontend.ClientTest/coverage/cobertura'}},
							{type: 'text-summary'}
						]
					}
				}
			}
		},
		
		//Converts directive's external html templates into javascript strings and stores them in the Angular's $templateCache service.
		html2js: {
			options: {
			  singleModule: true,
			  module: 'templates',
			  base: 'Telerik.Sitefinity.Frontend'
			},
			main: {
			  src: ['Telerik.Sitefinity.Frontend/client-components/**/*.html', 'Telerik.Sitefinity.Frontend/client-components/**/*.sf-cshtml'],
			  dest: 'Tests/Telerik.Sitefinity.Frontend.ClientTest/templates.js'
			},
		},
	});
	
	//Load the needed plugins
	grunt.loadNpmTasks('grunt-contrib-jshint');
	grunt.loadNpmTasks('grunt-contrib-jasmine');
	grunt.loadNpmTasks('grunt-html2js');
	
	//Default task(s)
	grunt.registerTask('default', ['jshint','html2js', 'jasmine']);
	
};