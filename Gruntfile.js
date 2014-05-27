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
					'Telerik.Sitefinity.Frontend/MVC/Scripts/Designer/*.js'
			]
		},
		
		jasmine: {
			unit:{
				src: 'Mvc/Scripts/**/*.js',
				options: {
					specs: 'JsTest/**/*.js'
				}
			}			
		}
	});
	
	//Load the needed plugins
	grunt.loadNpmTasks('grunt-contrib-jshint');
	grunt.loadNpmTasks('grunt-contrib-jasmine');
	
	//Default task(s)
	grunt.registerTask('default', ['jshint']);
	
};