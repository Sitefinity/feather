(function ($) {
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

                        var container;
                        if (range.startContainer && range.startContainer === range.endContainer && range.startContainer.tagName && range.startContainer.tagName.toLowerCase() !== 'a' && range.startContainer.hasChildNodes()) {
                            container = $(range.startContainer.outerHTML);
                        }
                        else {
                            container = null;
                        }

                        angular.element("#linkSelectorModal").scope().$openModalDialog().then(function (data) {
                            scope.selectedHtml = data;
                            var result = data.outerHTML;
                            if (container && container.length === 1) {
                                container.html(result);
                                result = container[0].outerHTML;
                            }

                            editor.exec("insertHtml", { html: result, split: true });
                        });
                    };

                    scope.imagePropertiesDialog =
                        serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/fields/html-field/sf-image-properties-content-block.html');

                    scope.openImageSelector = function () {

                        var range = editor.getRange();
                        var nodes = kendo.ui.editor.RangeUtils.textNodes(range);

                        var properties = null;

                        var imageWrapper = $(nodes).closest('span.sf-Image-wrapper');
                        if (imageWrapper.length) {
                            properties = mediaMarkupService.image.properties(imageWrapper[0].outerHTML);
                        }
                        else if ($(nodes).is('img')) {
                            properties = mediaMarkupService.image.properties($(nodes)[0].outerHTML);
                        }

                        var container;
                        if (range.startContainer && range.startContainer === range.endContainer && range.startContainer.tagName && range.startContainer.tagName.toLowerCase() !== 'img' && range.startContainer.hasChildNodes()) {
                            container = $(range.startContainer.outerHTML);
                        }
                        else {
                            container = null;
                        }

                        angular.element('.imagePropertiesModal').scope()
                            .$openModalDialog({ sfModel: function () { return properties; } })
                            .then(function (data) {
                                properties = data;

                                if (data.customSize)
                                    return mediaService.checkCustomThumbnailParams(data.customSize.Method, data.customSize);
                                else
                                    return '';

                            })
                            .then(function (errorMessage) {
                                if (properties.thumbnail && properties.thumbnail.url) {
                                    return properties.thumbnail.url;
                                }
                                else if (properties.customSize) {
                                    return mediaService.getCustomThumbnailUrl(properties.item.Id, properties.customSize);
                                }
                                else {
                                    return '';
                                }
                            })
                            .then(function (thumbnailUrl) {
                                if (thumbnailUrl) {
                                    properties.thumbnail = properties.thumbnail || {};
                                    properties.thumbnail.url = thumbnailUrl;
                                }

                                return mediaService.getLibrarySettings();
                            })
                            .then(function (settings) {
                                var wrapIt = true;
                                var markup = mediaMarkupService.image.markup(properties, settings, wrapIt);

                                if (container && container.length === 1) {
                                    container.html(markup);
                                    markup = container[0].outerHTML;
                                }

                                editor.exec('insertHtml', { html: markup, split: true });
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
                                fullToolbar = $('.k-editor-toolbar');
                            }

                            if (!customButtons) {
                                customButtons = fullToolbar.children().filter(function (child) {
                                    return $(this).children('.js-custom-tool').length > 0;
                                });
                            }

                            if (!shortToolbar) {
                                shortToolbar = fullToolbar.clone(true);
                                shortToolbar.html('');
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

                        fullScreenIcon = $('.js-fullScreen');

                        var modalHeaderAndFooter = $('.modal-dialog > .modal-content > .modal-header, .modal-dialog > .modal-content > .modal-footer');

                        var mainDialog = $('.modal-dialog');

                        if (isFullScreen === false) {
                            mainDialog.addClass('modal-full-screen');
                            fullScreenIcon.removeClass('glyphicon-resize-full');
                            fullScreenIcon.addClass('glyphicon-resize-small');
                        }
                        else {
                            mainDialog.removeClass('modal-full-screen');

                            fullScreenIcon.removeClass('glyphicon-resize-small');
                            fullScreenIcon.addClass('glyphicon-resize-full');
                        }

                        modalHeaderAndFooter.toggle();
                        isFullScreen = !isFullScreen;
                    };
                }
            };
        }])
        .controller('sfImagePropertiesController', ['$scope', '$modalInstance', 'serverContext', 'sfModel',
            function ($scope, $modalInstance, serverContext, sfModel) {
                // undefined, because the image-field sets it to null if cancel is pressed and the watch is triggered
                $scope.model = sfModel || { item: undefined };

                $scope.$watch('model.item.Id', function (newVal) {
                    if (newVal === null) {
                        $scope.cancel();
                    }
                });

                $scope.$watch('model.item.Title.Value', function (newVal, oldVal) {
                    if ($scope.model.item && $scope.model.item.Title && (oldVal === $scope.model.title || !$scope.model.title))
                        $scope.model.title = $scope.model.item.Title.Value;
                });

                $scope.$watch('model.item.AlternativeText.Value', function (newVal, oldVal) {
                    if ($scope.model.item && $scope.model.item.AlternativeText && (oldVal === $scope.model.alternativeText || !$scope.model.alternativeText))
                        $scope.model.alternativeText = $scope.model.item.AlternativeText.Value;
                });

                $scope.done = function () {
                    $modalInstance.close($scope.model);
                };

                $scope.cancel = function () {
                    $modalInstance.dismiss();
                };

                $scope.thumbnailSizeTempalteUrl = serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/selectors/media/sf-thumbnail-size-selection.html');
            }]);
})(jQuery);
