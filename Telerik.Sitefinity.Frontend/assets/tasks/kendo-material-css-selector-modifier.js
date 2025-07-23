const fs = require('fs');

module.exports = function (grunt) {
    grunt.registerMultiTask('kendo_material_css_selector_modifier', 'Prepends :root:root:root:root:root to all CSS selectors.', function () {
        // Iterate over all specified file groups
        this.files.forEach(function (file) {
            file.src.filter(function (filepath) {
                // Warn on and remove invalid source files
                if (!grunt.file.exists(filepath)) {
                    grunt.log.warn('Source file "' + filepath + '" not found.');
                    return false;
                } else {
                    return true;
                }
            }).forEach(function (filepath) {
                // Read the input CSS file
                const cssContent = grunt.file.read(filepath);

                // Regular expression to match CSS selectors
                const pattern = /([^\{\}]+)\{/g;

                // Function to prepend ":root:root:root:root:root" to each selector
                const modifiedCss = cssContent.replace(pattern, (match, selectors) => {
                    selectors = selectors.trim();

                    // Avoid prepending to @-rules like @media, @keyframes, etc.
                    if (selectors.startsWith('@')) {
                        return match;
                    }

                    // Split selectors by commas and process each one
                    const processedSelectors = selectors.split(',')
                        .map(selector => {
                            selector = selector.trim();

                            // Check if the selector starts with ":root"
                            if (selector.startsWith(":root")) {
                                // If it already starts with ":root", return it as-is
                                return selector;
                            }

                            // Check if the selector starts with "/**"
                            if (selector.startsWith("/**")) {
                                // If it already starts with "/**", return it as-is
                                return selector;
                            }

                            // Prepend ":root:root:root:root:root" to the selector
                            return `:root:root:root:root:root ${selector}`;
                        })
                        .join(', '); // Join the processed selectors back with commas

                    // Return the modified selectors with the opening brace
                    return `${processedSelectors} {`;
                });

                // Write the modified CSS to the output file
                grunt.file.write(file.dest, modifiedCss);

                // Print a success message
                grunt.log.writeln('File "' + file.dest + '" created.');
            });
        });
    });
};