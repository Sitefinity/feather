(function ($) {
    var sfFields = angular.module('sfFields');
    sfFields.requires.push('sfHtmlField');

    angular.module('sfHtmlField', ['kendo.directives', 'sfServices', 'sfImageField', 'sfThumbnailSizeSelection', 'sfAspectRatioSelection'])
        .directive('sfHtmlField', ['serverContext', '$compile', 'sfMediaService', 'sfMediaMarkupService', function (serverContext, $compile, mediaService, mediaMarkupService) {
            var editor = null;
            var content = null;
            var fullScreenIcon = null;
            var showAllCommands = false;

            return {
                restrict: "E",
                scope: {
                    sfModel: '='
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/fields/html-field/sf-html-field.sf-cshtml';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    pre: function (scope, element) {
                        scope.htmlFieldCssUrl = serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'assets/dist/css/html-field.min.css');

                        scope.getPackageResourceUrl = function (resourcePath) {
                            return serverContext.getRootedUrl(resourcePath);
                        };

                        scope.$on('kendoWidgetCreated', function (event, widget) {
                            if (widget.wrapper && widget.wrapper.is('.k-editor')) {
                                widget.focus();
                                editor = widget;
                                content = editor.wrapper.find('iframe.k-content').first();

                                fullScreenIcon = $(".js-fullScreen");
                                fullScreenIcon.addClass("glyphicon-resize-full");

                                scope.toggleAllTools();
                            }
                        });

                        scope.toggleAllTools = function () {
                            if (!editor) {
                                return;
                            }
                            var toolbar = editor.toolbar.element.eq(0);

                            var commands = editor.element.eq(0).attr('sf-toggle-commands');

                            if (commands) {
                                commands.split(',').forEach(function (command) {
                                    var selector = String.format("select.k-{0},a.k-{0},span.k-{0}", command.trim());
                                    var anchor = toolbar.find(selector).parents('li');
                                    var func = showAllCommands ? anchor.show : anchor.hide;
                                    func.call(anchor);
                                });
                            }
                            else {
                                toolbar.find('.show-all-button').parents('li').hide();
                            }
                            if (showAllCommands) {
                                toolbar.find('.show-all-button').addClass('k-state-active');
                            } else {
                                toolbar.find('.show-all-button').removeClass('k-state-active');
                            }
                            showAllCommands = !showAllCommands;
                        };
                    },
                    post: function (scope, element) {
                        scope.htmlViewLabel = 'HTML';

                        var isInHtmlView = false;
                        var isFullScreen = false;
                        var originalEditorSizes = null;
                        var fullToolbar = null;
                        var shortToolbar = null;
                        var customButtons = null;

                        function getPropertiesFromTag(type, wrapperClass, tag) {
                            var range = editor.getRange();
                            var nodes = kendo.ui.editor.RangeUtils.textNodes(range);

                            var properties = null;
                            var wrapper = $(nodes).closest(wrapperClass);
                            if (wrapper.length) {
                                properties = mediaMarkupService[type].properties(wrapper[0].outerHTML);
                            }
                            else if ($(nodes).is(tag)) {
                                properties = mediaMarkupService[type].properties($(nodes)[0].outerHTML);
                            }
                            return properties;
                        }

                        function getAnchorElement(range) {
                            var command = editor.toolbar.tools.createLink.command({ range: range });

                            var nodes = kendo.ui.editor.RangeUtils.textNodes(range);
                            var aTag = nodes.length ? command.formatter.finder.findSuitable(nodes[0]) : null;
                            var msie = /msie/.test(navigator.userAgent.toLowerCase());

                            if (msie && !aTag) {
                                aTag = nodes.length >= 2 ? command.formatter.finder.findSuitable(nodes[1]) : null;
                            }

                            return aTag;
                        }

                        // If an anchor is in the range, preserve it and insert the given markup in it.
                        function preserveWrapperATag(newMarkup) {
                            var range = editor.getRange();
                            var anchor = getAnchorElement(range);
                            if (anchor) {
                                $(anchor).html(newMarkup);
                                return anchor.outerHTML;
                            }
                            else {
                                return newMarkup;
                            }
                        }

                        scope.openLinkSelector = function () {
                            var range = editor.getRange();
                            var aTag = getAnchorElement(range);

                            if (aTag) {
                                scope.selectedHtml = aTag;
                            }
                            else {
                                scope.selectedHtml = editor.selectedHtml();
                            }

                            angular.element("#linkSelectorModal").scope().$openModalDialog().then(function (data) {
                                scope.selectedHtml = data;
                                editor.exec("insertHtml", { html: data.outerHTML, split: true, range: range });
                            });
                        };

                        scope.openDocumentSelector = function () {
                            scope.mediaPropertiesDialog =
                                    serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/fields/html-field/sf-document-properties-content-block.sf-cshtml');
                            scope.sfMediaPropertiesController = "sfDocumentPropertiesController";

                            var range = editor.getRange();
                            var aTag = getAnchorElement(range);
                            var properties = aTag ? mediaMarkupService.document.properties(aTag.outerHTML) : null;

                            setTimeout(function () {
                                angular.element('.mediaPropertiesModal')
                                    .scope()
                                    .$openModalDialog({ sfModel: function () { return properties; } })
                                    .then(function (data) {
                                        properties = data;
                                        return mediaService.getLibrarySettings();
                                    })
                                    .then(function (settings) {
                                        var markup = mediaMarkupService.document.markup(properties, settings);
                                        editor.exec('insertHtml', { html: markup, split: true, range: range });
                                    });
                            }, 0);
                        };

                        scope.openImageSelector = function () {
                            scope.mediaPropertiesDialog =
                               serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/fields/html-field/sf-image-properties-content-block.sf-cshtml');
                            scope.sfMediaPropertiesController = "sfImagePropertiesController";

                            var properties = getPropertiesFromTag('image', 'span.sf-Image-wrapper', 'img');

                            setTimeout(function () {
                                angular.element('.mediaPropertiesModal').scope()
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

                                        // If the image is wrapped in a link, insertHtml will override the link with the image,
                                        // so we have to preserve the anchor and put the image inside it.
                                        markup = preserveWrapperATag(markup);

                                        editor.exec('insertHtml', { html: markup, split: true });
                                    });
                            }, 0);
                        };

                        scope.openVideoSelector = function () {
                            scope.mediaPropertiesDialog =
                                    serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/fields/html-field/sf-video-properties-content-block.sf-cshtml');
                            scope.sfMediaPropertiesController = "sfVideoPropertiesController";

                            var range = editor.getRange();
                            var properties = getPropertiesFromTag('video', null, 'video');

                            setTimeout(function () {
                                angular.element('.mediaPropertiesModal')
                                    .scope()
                                    .$openModalDialog({ sfModel: function () { return properties; } })
                                    .then(function (data) {
                                        properties = data;
                                        return mediaService.getLibrarySettings();
                                    })
                                    .then(function (settings) {
                                        var markup = mediaMarkupService.video.markup(properties, settings);
                                        editor.exec('insertHtml', { html: markup, split: true, range: range });
                                    });
                            }, 0);
                        };

                        scope.toggleHtmlView = function () {
                            if (editor === null)
                                return;

                            if (isInHtmlView === false) {
                                $(".js-htmlview").addClass("active");

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

                                fullToolbar.addClass("sf-toolbar-full");
                                shortToolbar.addClass("sf-toolbar-short");
                                shortToolbar.show();
                                shortToolbar.append(customButtons);
                            } else {
                                $(".js-htmlview").removeClass("active");
                                fullToolbar.removeClass("sf-toolbar-full");
                                shortToolbar.removeClass("sf-toolbar-short");

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
            }])
        .controller('sfDocumentPropertiesController', ['$scope', '$modalInstance', 'serverContext', 'sfModel',
            function ($scope, $modalInstance, serverContext, sfModel) {

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

                $scope.done = function () {
                    $modalInstance.close($scope.model);
                };

                $scope.cancel = function () {
                    $modalInstance.dismiss();
                };
            }])
        .controller('sfVideoPropertiesController', ['$scope', '$modalInstance', 'serverContext', 'sfModel',
            function ($scope, $modalInstance, serverContext, sfModel) {

                $scope.model = sfModel || { item: undefined };

                $scope.videoModel = {
                    aspectRatio: $scope.model.aspectRatio,
                    width: $scope.model.width,
                    height: $scope.model.height
                };

                $scope.$watch('model.item.Id', function (newVal) {
                    if (newVal === null) {
                        $scope.cancel();
                    }
                });

                $scope.done = function () {
                    $scope.model.width = $scope.videoModel.aspectRatio === 'auto' ? null : $scope.videoModel.width;
                    $scope.model.height = $scope.videoModel.aspectRatio === 'auto' ? null : $scope.videoModel.height;

                    $modalInstance.close($scope.model);
                };

                $scope.cancel = function () {
                    $modalInstance.dismiss();
                };
            }]);
})(jQuery);