module.exports = function (grunt) {
    'use strict';
	
	//Project Configuration
	grunt.initConfig({
		
		pkg: grunt.file.readJSON('package.json'),
		
		jshint: {
			//define the files to lint
			files: ['gruntfile.js',
					'Mvc/Scripts/**/*.js',
					'JsTest/**/*.js'
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
	grunt.registerTask('default', ['jshint', 'jasmine']);
	
};