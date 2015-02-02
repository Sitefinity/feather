module.exports = function(grunt) {
	'use strict';

	// Project configuration.
	grunt.initConfig({

	pkg: grunt.file.readJSON('package.json'),

	less: {
		production: {
			options: {
				paths: ["less"],
				syncImport: true,
				strictImports: true,
				cleancss: true
			},
			files: {
				'css/feather.css' : 'less/feather.less'
			}
		}
	},

	uglify: {
		options: {
			mangle: false
		},
		production: {
			options: {
				mangle: false,
				compress: false,
				beautify: false,
				preserveComments: false
			},
			files: [{
				'feather.min.js': [
					'bower_components/angular/angular.min.js',
					'bower_components/angular-route/angular-route.min.js',
					'bower_components/angular-scroll/angular-scroll.min.js',
					'bower_components/angular-bootstrap/ui-bootstrap-tpls.min.js',
					'bower_components/moment/min/moment.min.js',
					'feather.js'
				] 
			}]
		}
	},

	watch: {
		less: {
			files: ['less/**/*.less'],
			tasks: ['less'],
			options: {
				livereload: true
			}
		},
		js: {
			files: ['feather.js'],
			tasks: ['uglify'],
			options: {
				livereload: true
			}
		}
	},
	});

	// Load the needed plugins
	grunt.loadNpmTasks('grunt-contrib-less');
	grunt.loadNpmTasks('grunt-contrib-watch');
	grunt.loadNpmTasks('grunt-contrib-uglify');

	// Default task(s).
	grunt.registerTask('default', ['less', 'uglify']);

	grunt.event.on('watch', function(action, filepath, target) {
		grunt.log.writeln(target + ': ' + filepath + ' has ' + action);
	});
};