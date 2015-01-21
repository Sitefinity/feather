(function ($) {
    var module = angular.module('sfFields', ['kendo.directives', 'sfServices']);

    module.directive('sfHtmlField', ['serverContext', '$compile', function (serverContext, $compile) {
        return {
            restrict: "E",
            scope: {
                ngModel: '='
            },
            templateUrl: function (elem, attrs) {
                var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                var url = attrs.sfTemplateUrl || 'client-components/fields/html-field/sf-html-field.html';
                return serverContext.getEmbeddedResourceUrl(assembly, url);
            },
            link: function (scope, element) {
                scope.htmlViewLabel = 'HTML';

                var isInHtmlView = false;
                var isFullScreen = false;
                var editor = null;
                var content = null;
                var editorWrapperInitialStyle = null;
                var fullToolbar = null;
                var shortToolbar = null;
                var customButtons = null;

                scope.$on('kendoWidgetCreated', function (event, widget) {
                    widget.focus();
                    editor = widget;
                    content = editor.wrapper.find('iframe.k-content').first();
                });

                scope.toggleHtmlView = function () {
                    if (editor === null)
                        return;

                    if (isInHtmlView === false) {
                        scope.htmlViewLabel = 'Design';

                        var htmlEditor = $('<textarea class="html k-content" ng-model="ngModel" style="resize: none">');
                        $compile(htmlEditor)(scope);
                        htmlEditor.insertAfter(content);
                        content.hide();

                        if (!fullToolbar) {
                            fullToolbar = $(".k-editor-toolbar");
                        }

                        if (!customButtons) {
                            customButtons = fullToolbar.children().filter(function (child) {
                                return $(this).children("[ng-click]").length > 0;
                            });
                        }
                        
                        if (!shortToolbar) {
                            shortToolbar = fullToolbar.clone(true);
                            shortToolbar.html("");
                            fullToolbar.after(shortToolbar);
                        }

                        fullToolbar.hide();
                        shortToolbar.show();
                        shortToolbar.append(customButtons);
                        $compile(shortToolbar.html())(scope);
                    } else {
                        scope.htmlViewLabel = 'HTML';

                        shortToolbar.hide();
                        fullToolbar.show();
                        fullToolbar.append(customButtons);

                        var html = editor.wrapper.find('.html');
                        html.remove();
                        content.show();
                    }

                    isInHtmlView = !isInHtmlView;
                };

                scope.toggleFullScreen = function () {
                    if (editor === null)
                        return;

                    var dialog = $(".modal-dialog");
                    var editorBodies = $(".modal-body iframe.k-content, .modal-body textarea.k-content");

                    if (!editorWrapperInitialStyle) {
                        editorWrapperInitialStyle = {
                            dialog: {
                                margin: dialog.css('margin'),
                                width: dialog.width()
                            },
                            editorBodies: {
                                height: editorBodies.height()
                            }
                        };
                    }

                    if (isFullScreen === false) {
                        dialog.css({
                            margin: 0,
                            width: $("body").width()
                        });

                        // // For full screen with no scroller 
                        editorBodies.height($(document).height() - $('.modal-footer').outerHeight() - $('.modal-header').outerHeight() -
                           ($('.modal-body').outerHeight() - editorWrapperInitialStyle.editorBodies.height));

                        // For full screen editor area (scroller for Save, Cancel and Advanced buttons)
                        // editorBodies.height($(document).height());
                    } else {
                        dialog.css({
                            margin: editorWrapperInitialStyle.dialog.margin,
                            width: editorWrapperInitialStyle.dialog.width
                        });
                        editorBodies.height(editorWrapperInitialStyle.editorBodies.height);
                    }

                    isFullScreen = !isFullScreen;
                };
            }
        };
    }]);
})(jQuery);