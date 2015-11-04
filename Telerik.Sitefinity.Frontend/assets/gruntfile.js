'use strict';
var path = require('path');

// match one level down:
// e.g. 'bar/foo/{,*/}*.js'
// use this if you want to recursively match all subfolders:
// e.g. 'bar/foo/**/*.js'

module.exports = function (grunt) {
  'use strict';

  // load all grunt tasks
  require('load-grunt-tasks')(grunt);

  // show elapsed time at the end
  require('time-grunt')(grunt);
  // Init
  grunt.initConfig({
    timestamp: '<%= new Date().getTime() %>',
    pkg: grunt.file.readJSON('package.json'),

    src: {
      path       : 'src',
      sass       : '**/*.{scss,sass}',
    },
    dist: {
      path       : 'dist'
    },

    // clean all generated files
    clean: {
      all: {
        files: [{
          src: [
            '<%= dist.path %>/**/*.css',
            '<%= dist.path %>/**/*.js',
            '<%= dist.path %>/**/*.{png,jpg,gif}'
          ]
        }]
      },
      sprites: {
        files: [{
          src: [
            '<%= pkg.name %>/images/src/sprites/*.png',
            '<%= pkg.name %>/images/dist/sprites/*.png'
          ]
        }]
      }
    },

    // use always with target e.g. `csslint:doc` or `csslint:dev`
    // unfortunately there is no point to run csslint on compressed css so
    // csslint runs once, when you use `grunt` and it lints on documentation's css
    // csslint runs on every save when you use `grunt dev` and it lints the original file you are working on -> `style.css`
    csslint: {
      options: {
        csslintrc: 'csslint.json'
      },
      dev: {
        src: '<%= dist.path %>/styles.css'
      }
    },

    sass: {
      dist: {
        files: {
          '<%= dist.path %>/css/sitefinity-backend.css': '<%= src.path %>/sass/sitefinity-backend.sass',
          '<%= dist.path %>/css/html-field.css': '<%= src.path %>/sass/html-field.sass',
          '<%= dist.path %>/css/loading.css': '<%= src.path %>/sass/loading.sass'
        }
      },
      dev: {
        options: {
          sourceComments: 'normal'
        },
        files: {
          '<%= dist.path %>/css/sitefinity-backend.css': '<%= src.path %>/sass/sitefinity-backend.sass',
          '<%= dist.path %>/css/html-field.css': '<%= src.path %>/sass/html-field.sass',
          '<%= dist.path %>/css/loading.css': '<%= src.path %>/sass/loading.sass'
        }
      }
    },

    uncss: {
      dist: {
        options: {
          ignore       : ['#added_at_runtime', /test\-[0-9]+/],
          media        : ['(min-width: 700px) handheld and (orientation: landscape)'],
          csspath      : '../../../',
          stylesheets  : ['<%= dist.path %>/css/styles.css'],
          ignoreSheets : [/fonts.googleapis/],
          // urls         : ['http://localhost:3000/mypage', '...'], // Deprecated
          timeout      : 1,
          report       : 'gzip'
        },
        files: {
          '<%= dist.path %>/css/styles.tidy.css': 'MVC/**/*.cshtml'
        }
      }
    },

    webfont: {
      icons: {
        src: 'src/icons/*.svg',
        dest: 'dist/fonts/',
        destCss: 'src/sass/icons/',
        options: {
          destHtml: '',
          engine: 'node',
          font: 'sf-icon-font',
          stylesheet: 'scss',
          partialPrefix: true,
          relativeFontPath: '../fonts/',
          template: 'feather-icons.css',
          types: 'eot,woff,ttf,svg',
          order: 'eot,woff,ttf,svg',
          startCodepoint: 0x00b1,
          normalize: true,
          fontHeight: 4096,
          ascent: 4096,
          descent: 0
        }
      }
    },

    cssmin: {
      minify: {
        expand: true,
        cwd: '<%= dist.path %>/css/',
        src: ['*.css', '!*.min.css'],
        dest: '<%= dist.path %>/css/',
        ext: '.min.css'
      }
    },

    // Concat & minify
    // this processes only the files described in 'jsfiles.json'
    uglify: {
      options: {
        report: 'gzip',
        warnings: true
      },
      dist: {
        options: {
          mangle: true,
          compress: true,
        },
        files: {
          '<%= dist.path %>/js/all.min.js' : ['<%= src.path %>/js/{,*/}*.js']
        }
      }
    },

    // Image Optimization
    imagemin: {
      dist: {
        options: {
          optimizationLevel: 4,
          progressive: true
        },
        files: [
          {
            expand: true,
            cwd: '<%= src.path %>/img',
            src: ['**/*.{png,jpg,gif,jpeg}'],
            dest: '<%= dist.path %>/img'
          }
        ]
      }
    },

    watch: {
      options: {
        spawn: false
      },
      styles: {
        files: ['<%= src.path %>/**/*.{scss,sass}'],
        tasks: ['sass:dist', 'cssmin']
        // tasks: ['sass:dist', 'uncss', 'cssmin']
      },
      js: {
        files: ['<%= src.path %>/**/*.js'],
        tasks: ['uglify:dist']
      },
    },

    concurrent: {
      dev: {
        tasks: ['watch:styles', 'watch:js'],
        options: {
          logConcurrentOutput: true
        }
      }
    }
  });

  grunt.registerTask('iconfont', [
    'webfont'
  ]);

  // Tasks
  grunt.registerTask('default', [
    'clean:all',
    'sass:dist',
    // 'uncss',
    'cssmin',
    'uglify:dist',
    'newer:csslint:dev',
    'newer:imagemin',
    'concurrent:dev'
  ]);

  // Dev
  grunt.registerTask('dev', [
    'clean:all',
    'sass:dev',
    'cssmin',
    'uglify:dist',
    'newer:csslint:dev',
    'newer:imagemin',
    'concurrent:dev'
  ]);

  // Run this task to strip the CSS of unused classes and to minify it
  grunt.registerTask('stripcss', [
    'clean:all',
    'sass:dist',
    'uncss',
    'cssmin'
  ]);

};
