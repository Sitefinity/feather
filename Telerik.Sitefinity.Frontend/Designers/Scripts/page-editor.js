/* global $telerik, document, kendo */

var sitefinity = sitefinity || {};

(function ($) {
    $(function () {
        var loader,
        loaderMarkup = '<div class="sf-loading-wrapper"><div class="sf-loading"><span></span></div></div>',
        loaderTemplate = kendo.template(loaderMarkup),
        dialog,
        pageScripts = $('script').map(function () {
            return $(this).attr('src');
        }).toArray();

        function isScriptTag(tag) {
            return tag.tagName == 'SCRIPT' && (!tag.type || tag.type.toLowerCase() == 'text/javascript');
        }

        function extractScripts(markup) {
            var div = document.createElement('div');
            div.innerHTML = markup;
            var scripts = div.getElementsByTagName('script');

            var result = [];
            for (var i = 0; i < scripts.length; i++) {
                if (isScriptTag(scripts[i])) {
                    result.push(scripts[i]);
                }
            }

            return result;
        }

        function stripScripts(markup) {
            var div = document.createElement('div');
            div.innerHTML = markup;
            var scripts = div.getElementsByTagName('script');

            var i = scripts.length;
            while (i--) {
                if (isScriptTag(scripts[i])) {
                    scripts[i].parentNode.removeChild(scripts[i]);
                }
            }

            return div.innerHTML;
        }

        function loadScripts(container, scriptTags, loadHandler) {
            var lab = $LAB.setOptions({
                AlwaysPreserveOrder: true,
                AllowDuplicates: true,
            });

            var labNoDuplicates = $LAB.setOptions({
                AlwaysPreserveOrder: true,
                AllowDuplicates: false
            });

            for (var i = 0; i < scriptTags.length; i++) {
                if (scriptTags[i].src) {
                    // Hack to not load bootstrap multiple times, should be removed with the next version of bootstrap or with feather designer script refactoring
                    if (scriptTags[i].src.indexOf('Mvc/Scripts/Bootstrap/js/bootstrap.min.js') >= 0) {
                        var src = $(scriptTags[i]).attr('src');

                        if (pageScripts.indexOf(src) < 0) {
                            labNoDuplicates.script(src);
                        }
                    }
                    else {
                        lab = lab.script(scriptTags[i].src);
                    }
                }
                else if (scriptTags[i].text) {
                    var text = scriptTags[i].text;
                    lab = lab.wait(function () { eval(text); }); // jshint ignore:line
                }
            }

            labNoDuplicates.wait(function () {
                lab.wait(loadHandler);
            });
        }

        /**
         * Represents the Sitefinity page editor.
         */
        sitefinity.pageEditor = {
            /**
             * Shows the loading animation in the page editor.
             *
             * @param {String} appPath The relative path of the application used to resolve the loading image.
             */
            showLoader: function (appPath) {
                if (loader) {
                    loader.show();
                } else {
                    loader = $(loaderTemplate({ appPath: appPath }));
                    $('body').append(loader);
                }
            },

            /**
             *  Hides the loading animation in the page editor.
             */
            hideLoader: function () {
                $(loader).hide();
            },

            /**
             * Renders the dialog within page editor with the provided markup.
             *
             * @param {String} markup The HTML markup to be rendered within the dialog.
             */
            renderDialog: function (markup) {
                dialog = $('<div />');
                $('body').append(dialog);

                var scriptTags = extractScripts(markup);
                markup = stripScripts(markup);

                dialog.append(markup);
                dialog.on('hidden.bs.modal', this.destroyDialog);

                var that = this;
                loadScripts(dialog[0], scriptTags, function () {
                    that.hideLoader();

                    if (typeof ($telerik) != 'undefined') {
                        $telerik.$(document).trigger('dialogRendered');
                    }
                });
            },

            /**
             * Event handler that handles the needDialog event from the Sitefinity
             * page editor.
             */
            openDialog: function (ev, args) {
                this.showLoader(args.AppPath);
                this.widgetContext = args;

                var separator;
                if (this.widgetContext.url.indexOf('?') > -1)
                    separator = '&';
                else
                    separator = '?';

                var url = this.widgetContext.url + separator + 'controlId=' + this.widgetContext.Id;
                if (this.widgetContext.ModuleName) {
                    url += '&moduleName=' + this.widgetContext.ModuleName;
                }
                $.get(url)
                    .done($.proxy(this.renderDialog, this))
                    .fail(function (data) {
                        alert('There is a problem with loading the widget designer: ' + data);
                    });
            },

            openGridDialog: function (ev, args) {
                this.showLoader(args.AppPath);
                this.gridContext = args;

                var separator;
                if (this.gridContext.url.indexOf('?') > -1)
                    separator = '&';
                else
                    separator = '?';

                var url = this.gridContext.url + separator + 'controlId=' + this.gridContext.Id;
                url += '&gridTitle=' + this.gridContext.GridTitle;
                $.get(url)
                    .done($.proxy(this.renderDialog, this))
                    .fail(function (data) {
                        alert('There is a problem with loading the grid designer: ' + data);
                    });
            },

            openPersonalizationDialog: function (ev, args) {
                this.showLoader(args.AppPath);
                this.widgetContext = args;

                var url = this.widgetContext.AppPath + "Telerik.Sitefinity.Frontend/PersonalizationDesigner/Master/PersonalizationDesigner/";
                $.get(url)
                    .done($.proxy(this.renderDialog, this))
                    .fail(function (data) {
                        alert('There is a problem with loading the widget designer: ' + data);
                    });
            },

            /**
             * Destroys the currently opened dialog.
             */
            destroyDialog: function () {
                if (dialog) {
                    dialog.remove();
                    this.widgetContext = null;
                    this.gridContext = null;
                }
            },

            /**
             * Provides the context for the currently active widget.
             */
            widgetContext: null,
            gridContext: null

        };

        /**
         * Register the global Sitefinity events with the pageEditor component.
         */
        if (typeof ($telerik) != 'undefined') {
            $telerik.$(document).on('needsModalDialog', $.proxy(sitefinity.pageEditor.openDialog, sitefinity.pageEditor));

            $telerik.$(document).on('needsPersonalizationModalDialog', $.proxy(sitefinity.pageEditor.openPersonalizationDialog, sitefinity.pageEditor));

            $telerik.$(document).on('needsGridModalDialog', $.proxy(sitefinity.pageEditor.openGridDialog, sitefinity.pageEditor));
            $telerik.$(document).on('modalDialogClosed', $.proxy(sitefinity.pageEditor.destroyDialog, sitefinity.pageEditor));
            $telerik.$(document).on('gridModalDialogClosed', $.proxy(sitefinity.pageEditor.destroyDialog, sitefinity.pageEditor));
        }
    }); 
})(jQuery);
