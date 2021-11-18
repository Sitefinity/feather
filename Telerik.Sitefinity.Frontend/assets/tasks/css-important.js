var rework = require('rework');
var cssmin = require('cssmin');

module.exports = function(grunt) {
    // Copied from https://www.npmjs.com/package/grunt-css-important and done some modification in order not to set !important to every property
    grunt.registerMultiTask('css_important', 'Adds !important tags to your CSS styles.', function () {
        // Merge task-specific and/or target-specific options with these defaults.
        var options = this.options({
            punctuation: '.',
            separator: '; '
        });
        var styleSheetSource = '';

        // Iterate over all specified file groups.
        this.files.forEach(function (file) {
            
            // Concat specified files.
            var src = file.src.filter(function (filepath) {
                // Warn on and remove invalid source files (if nonull was set).
                if (!grunt.file.exists(filepath)) {
                    grunt.log.warn('Source file "' + filepath + '" not found.');
                    return false;
                } else {
                    return true;
                }
            }).forEach(function (filepath) {
                // Read file source.
                styleSheetSource = grunt.file.read(filepath);
                // Remove nested selector in @font-face
                removeNestedSelectorInFontFace(styleSheetSource);
                
                var css = rework(styleSheetSource).use(function (values) {
                    changeRules(values.rules);
                }).toString();

                if (options.minified) {
                    css = cssmin(css);
                }

                // Write the destination file.
                grunt.file.write(file.dest, css);

                // Print a success message.
                grunt.log.writeln('File "' + file.dest + '" created.');
            });
        });

        // Fix nested rules
        function changeRules(rules) {
            rules.forEach(function (r) {
                if (r.declarations) {
                    r.declarations.forEach(function (d) {                
                        if (checkIfShouldSetImportant(d)) {
                            d.value += ' !important';
                        }
                    });
                }

                if (r.rules) {
                    changeRules(r.rules);
                }
            });
        }

        function checkIfShouldSetImportant(declaration) {
            var selectors = declaration.parent.selectors;
            var restrictedProperties = [];
            var restrictedSelectors = [];

            if (options.skipProperties) {
                restrictedProperties = options.skipProperties;
            }

            if (options.skipSelectors) {
                restrictedSelectors = options.skipSelectors;
            }

            // This excludes sourcemap entries and font-face            
            var isAllowedType = declaration.parent.type !== 'font-face' && selectors.indexOf('filename') === -1 && selectors.indexOf('line') === -1;
            var isAllowedProperty = restrictedProperties.indexOf(declaration.property) === -1;
            var isAllowedSelector = true;

            restrictedSelectors.forEach(rs => {
                if (selectors) {
                    selectors.forEach(sel => {
                        if (sel.indexOf(rs.selector) !== -1 && rs.properties.indexOf(declaration.property) !== -1) {
                            isAllowedSelector = false;
                        }
                    })
                }
            });

            if (isAllowedType && isAllowedProperty && isAllowedSelector) {
                // Don't add important twice
                return declaration.value && declaration.value.indexOf('!important') === -1
            } else {
                return false;
            }
        }

        function removeNestedSelectorInFontFace(str, startingIndex) {
            if (!startingIndex) {
                startingIndex = 0;
            }

            var searchAgain = true;
            var fontFaceSelector = '@font-face';
            var openBraket = '{';
            var closeBraket = '}';
            var nestedSelector = ':root:root:root:root:root .sf-backend-wrp'

            var fontFaceIndex = str.indexOf(fontFaceSelector, startingIndex);

            if (fontFaceIndex > -1) {
                var closeBraketIndex = str.indexOf(closeBraket, fontFaceIndex);
                var innerRules = str.substring(fontFaceIndex, closeBraketIndex + 1);

                if (innerRules.indexOf(nestedSelector) > -1) {
                    var nestedSelectorIndex = str.indexOf(nestedSelector, fontFaceIndex);
                    var startSelector = str.substring(nestedSelectorIndex, str.indexOf(openBraket, nestedSelectorIndex) + 1);

                    // Remove the unnecessary selectors 
                    var end = str.indexOf(closeBraket, nestedSelectorIndex);
                    str = str.slice(0, end) + str.slice(end + 1);
                    str = str.replace(startSelector, '');
                }

                startingIndex = str.indexOf(closeBraket, fontFaceIndex);
                searchAgain = str.indexOf(fontFaceSelector, startingIndex) > -1;
            } else {
                searchAgain = false;
            }

            if (searchAgain) {
                removeNestedSelectorInFontFace(str, startingIndex);
            } else {
                styleSheetSource = str;
            }
        }
    });
}