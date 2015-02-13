﻿(function ($) {
    var sfFields = angular.module('sfFields');
    sfFields.requires.push('sfHtmlField');

    angular.module('sfHtmlField', ['kendo.directives', 'sfServices', 'sfImageField', 'sfThumbnailSizeSelection'])
        .directive('sfHtmlField', ['serverContext', '$compile', 'sfMediaService', 'sfMediaMarkupService', function (serverContext, $compile, mediaService, mediaMarkupService) {
            return {
                restrict: "E",
                scope: {
                    sfModel: '='
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
                    var originalEditorSizes = null;
                    var fullToolbar = null;
                    var shortToolbar = null;
                    var customButtons = null;
                    var fullScreenIcon = null;

                    scope.$on('kendoWidgetCreated', function (event, widget) {
                        if (widget.wrapper && widget.wrapper.is('.k-editor')) {
                            widget.focus();
                            editor = widget;
                            content = editor.wrapper.find('iframe.k-content').first();

                            fullScreenIcon = $(".js-fullScreen");
                            fullScreenIcon.addClass("glyphicon-resize-full");
                        }
                    });

                    scope.openLinkSelector = function () {
                        var range = editor.getRange();
                        var command = editor.toolbar.tools.createLink.command({ range: range });

                        var nodes = kendo.ui.editor.RangeUtils.textNodes(range);
                        var aTag = nodes.length ? command.formatter.finder.findSuitable(nodes[0]) : null;

                        if (jQuery.browser.msie && !aTag) {
                            aTag = nodes.length >= 2 ? command.formatter.finder.findSuitable(nodes[1]) : null;
                        }

                        if (aTag) {
                            scope.selectedHtml = aTag;
                        }
                        else {
                            scope.selectedHtml = editor.selectedHtml();
                        }

                        angular.element("#linkSelectorModal").scope().$openModalDialog().then(function (data) {
                            scope.selectedHtml = data;
                            editor.exec("insertHtml", { html: data.outerHTML, split: true });
                        });
                    };

                    scope.imagePropertiesDialog =
                        serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/selectors/media/sf-image-properties.html');

                    scope.openImageSelector = function () {

                        var range = editor.getRange();
                        var nodes = kendo.ui.editor.RangeUtils.textNodes(range);

                        var properties = null;

                        var imageWrapper = $(nodes).closest('span.sfImageWrapper');
                        if (imageWrapper.length) {
                            properties = mediaMarkupService.image.properties(imageWrapper[0].outerHTML, settings);
                        }
                        else if ($(nodes).is('img')) {
                            properties = mediaMarkupService.image.properties($(nodes)[0].outerHTML, settings);
                        }

                        angular.element(".imagePropertiesModal").scope()
                            .$openModalDialog({ sfModel: function () { return properties; } })
                            .then(function (data) {
                                properties = data;

                                return mediaService.getLibrarySettings();
                            })
                            .then(function (settings) {
                                var wrapIt = true;
                                var markup = mediaMarkupService.image.markup(properties, settings, wrapIt);
                                editor.exec("insertHtml", { html: markup, split: true });
                            });
                    };

                    scope.toggleHtmlView = function () {
                        if (editor === null)
                            return;

                        if (isInHtmlView === false) {
                            scope.htmlViewLabel = 'Design';

                            var htmlEditor = $('<textarea class="html k-content" ng-model="sfModel" style="resize: none">');
                            $compile(htmlEditor)(scope);
                            htmlEditor.insertAfter(content);
                            content.hide();

                            if (!fullToolbar) {
                                fullToolbar = $(".k-editor-toolbar");
                            }

                            if (!customButtons) {
                                customButtons = fullToolbar.children().filter(function (child) {
                                    return $(this).children(".js-custom-tool").length > 0;
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
                        if (editor === null) {
                            return;
                        }

                        fullScreenIcon = $(".js-fullScreen");

                        var modalHeaderAndFooter = $(".modal-dialog > .modal-content > .modal-header, .modal-dialog > .modal-content > .modal-footer");

                        var mainDialog = $(".modal-dialog");

                        if (isFullScreen === false) {
                            mainDialog.addClass("modal-full-screen");
                            fullScreenIcon.removeClass("glyphicon-resize-full");
                            fullScreenIcon.addClass("glyphicon-resize-small");
                        }
                        else {
                            mainDialog.removeClass("modal-full-screen");

                            fullScreenIcon.removeClass("glyphicon-resize-small");
                            fullScreenIcon.addClass("glyphicon-resize-full");
                        }

                        modalHeaderAndFooter.toggle();
                        isFullScreen = !isFullScreen;
                    };
                }
            };
        }])
        .controller('sfImagePropertiesController', ['$scope', '$modalInstance', 'serverContext', 'sfModel',
            function ($scope, $modalInstance, serverContext, sfModel) {
                $scope.model = sfModel;

                $scope.imageId = null;

                $scope.done = function () {
                    $modalInstance.close($scope.model);
                };

                $scope.cancel = $modalInstance.dismiss;

                $scope.thumbnailSizeTempalteUrl = serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/selectors/media/sf-thumbnail-size-selection.html');
            }]);
})(jQuery);
