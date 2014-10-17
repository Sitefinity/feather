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
					'Telerik.Sitefinity.Frontend/Selectors/*.js',
					'Telerik.Sitefinity.Frontend/Services/*.js'
			]
		},
		
		jasmine: {
			unit:{
				src: [
				'Tests/Telerik.Sitefinity.Frontend.ClientTest/templates.js',
				'Telerik.Sitefinity.Frontend/Designers/Scripts/*.js',
				'Telerik.Sitefinity.Frontend/MVC/Scripts/Designer/*.js',
				'Telerik.Sitefinity.Frontend/MVC/Scripts/*.js',
				'Telerik.Sitefinity.Frontend/Services/services.js',
				'Telerik.Sitefinity.Frontend/Services/news-item-service.js',
				'Telerik.Sitefinity.Frontend/Services/data-service.js',
				'Telerik.Sitefinity.Frontend/Services/flat-taxon-service.js',
				'Telerik.Sitefinity.Frontend/Selectors/selectors.js',
				'Telerik.Sitefinity.Frontend/Selectors/list-selector.js',
				'Telerik.Sitefinity.Frontend/Selectors/news-selector.js',
				'Telerik.Sitefinity.Frontend/Selectors/dynamic-items-selector.js',
				'Telerik.Sitefinity.Frontend/Selectors/taxon-selector.js',
				'!Telerik.Sitefinity.Frontend/Designers/Scripts/page-editor.js',
				'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/mocks/*.js'],
				options: {
					vendor:[
					'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/jquery-1.8.3.min.js',
					'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/angular.js',
					'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/angular-resource.js',
					'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/angular-route.js',
					'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/angular-mocks.js',
					'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/kendo.web.min.js',
					'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/angular-kendo.js',
					'Telerik.Sitefinity.Frontend/MVC/Scripts/Bootstrap/js/*.js',
					'!Telerik.Sitefinity.Frontend/Mvc/Scripts/Angular/**'
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
			  src: ['Telerik.Sitefinity.Frontend/Selectors/*.html'],
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