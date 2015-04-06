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
                    var url = attrs.sfTemplateUrl || 'client-components/fields/html-field/sf-html-field.sf-cshtml';
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

                    scope.htmlFieldCssUrl =
                        serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'assets/dist/css/html-field.min.css');

                    scope.getPackageResourceUrl = function (resourcePath) {
                        return serverContext.getRootedUrl(resourcePath);
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

                $scope.model.aspectRatio = 'auto';

                $scope.$watch('model.item.Id', function (newVal) {
                    if (newVal === null) {
                        $scope.cancel();
                    }
                });

                $scope.$watch('model.aspectRatio', function (newVal) {
                    if (newVal === '4x3') {
                        $scope.model.width = 600;
                        $scope.model.height = 450;
                        $scope.model.aspectRatioCoefficient = 4 / 3;
                    }
                    else if (newVal === '16x9') {
                        $scope.model.width = 600;
                        $scope.model.height = 338;
                        $scope.model.aspectRatioCoefficient = 16 / 9;
                    }
                    else if (newVal === 'auto') {
                        if (!$scope.item) return;
                        $scope.model.width = $scope.item.Width;
                        $scope.model.height = $scope.item.Height;
                    }
                    else if (newVal === 'custom') {
                        $scope.model.width = "";
                        $scope.model.height = "";
                    }
                });

                $scope.updateWidth = function () {
                    if ($scope.model.aspectRatio != '16x9' && $scope.model.aspectRatio != '4x3') return;

                    $scope.model.width = Math.round($scope.model.height * $scope.model.aspectRatioCoefficient);
                };

                $scope.updateHeight = function () {
                    if ($scope.model.aspectRatio != '16x9' && $scope.model.aspectRatio != '4x3') return;

                    $scope.model.height = Math.round($scope.model.width / $scope.model.aspectRatioCoefficient);
                };

                $scope.done = function () {
                    $modalInstance.close($scope.model);
                };

                $scope.cancel = function () {
                    $modalInstance.dismiss();
                };
            }]);
})(jQuery);
