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
				src: ['Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/jquery-1.8.3.min.js',
	  'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/angular.js',
      'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/angular-route.js',
      'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/angular-mocks.js',
      'Telerik.Sitefinity.Frontend/**/*.js',     
      '!Telerik.Sitefinity.Frontend/Mvc/Scripts/Angular/**',
	  '!Telerik.Sitefinity.Frontend/Designers/Scripts/page-editor.js',
	  'Tests/Telerik.Sitefinity.Frontend.ClientTest/helpers/mocks/*.js'],
				options: {
					specs: 'Tests/Telerik.Sitefinity.Frontend.ClientTest/unit/**/*.js',
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
		}
	});
	
	//Load the needed plugins
	grunt.loadNpmTasks('grunt-contrib-jshint');
	grunt.loadNpmTasks('grunt-contrib-jasmine');
	
	//Default task(s)
	grunt.registerTask('default', ['jshint', 'jasmine']);
	
};