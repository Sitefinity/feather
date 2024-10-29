﻿(function ($) {
    var sfFields = angular.module('sfFields');
    sfFields.requires.push('sfHtmlField');

    angular.module('sfHtmlField', ['kendo.directives', 'sfServices', 'sfImageField', 'sfThumbnailSizeSelection', 'sfAspectRatioSelection'])
        .directive('sfHtmlField', ['serverContext', '$compile', 'sfMediaService', 'sfMediaMarkupService', function (serverContext, $compile, mediaService, mediaMarkupService) {
            return {
                restrict: "E",
                scope: {
                    sfModel: '=',
                    sfDocumentsSettings: '=?',
                    sfVideosSettings: '=?',
                    sfImagesSettings: '=?',
                    sfViewType: '@'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/fields/html-field/sf-html-field.sf-cshtml';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    pre: function (scope, element) {
                        var htmlElement = element;
                        scope.editor = null;
                        scope.content = null;
                        scope.fullScreenIcon = null;
                        scope.showAllCommands = false;


                        scope.htmlFieldCssUrl = serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'assets/dist/css/html-field.min.css');

                        scope.getPackageResourceUrl = function (resourcePath) {
                            return serverContext.getRootedUrl(resourcePath);
                        };

                        scope.$on('kendoWidgetCreated', function (event, widget) {
                            if (widget.wrapper && widget.wrapper.is('.k-editor')) {
                                widget.focus();
                                scope.editor = widget;
                                scope.content = scope.editor.wrapper.find('iframe.k-content').first();

                                scope.fullScreenIcon = htmlElement.find(".js-fullScreen");
                                
                                scope.toggleAllTools();
                                scope.removeCommandAttributes();
                                scope.overrideClickEvents();
                            }
                        });

                        scope.toggleAllTools = function () {
                            if (!scope.editor) {
                                return;
                            }
                            var toolbar = scope.editor.toolbar.element.eq(0);

                            var commands = scope.editor.element.eq(0).attr('sf-toggle-commands');

                            if (commands) {
                                commands.split(',').forEach(function (command) {
                                    var selector = String.format("select.k-{0},a.k-{0},span.k-{0},select.k-i-{0},a.k-i-{0},span.k-i-{0},.k-svg-i-{0}", command.trim());
                                    var anchor = toolbar.find(selector);
                                    var parent = anchor.parents('.k-toolbar-tool');

                                    if (!anchor.length) {
                                        parent = toolbar.find(".k-toolbar-tool[data-command=" + command + "]");
                                    }

                                    parent.toggleClass("invisible-group", !scope.showAllCommands);
                                });
                            }
                            else {
                                toolbar.find('.show-all-button').parents('.k-toolbar-tool').hide();
                            }
                            if (scope.showAllCommands) {
                                toolbar.find('.show-all-button').addClass('k-selected');
                            } else {
                                toolbar.find('.show-all-button').removeClass('k-selected');
                            }
                            scope.showAllCommands = !scope.showAllCommands;
                        };

                        scope.removeCommandAttributes = function () {
                            var PROPERTY_NAME = 'command';
                            var TOOLS_TO_REMOVE_COMAND_ATTRIBUTE_FROM = ['createLink', 'insertImage', 'insertFile'];

                            TOOLS_TO_REMOVE_COMAND_ATTRIBUTE_FROM.forEach(function (tool) {
                                var el = $(htmlElement.find('[data-command="' + tool + '"]'));
                                el.removeAttr('data-' + PROPERTY_NAME);
                                el.removeData('PROPERTY_NAME');
                            });
                        };

                        scope.overrideClickEvents = function () {
                            $('[title="Insert hyperlink"]').click(function () {
                                scope.openLinkSelector();
                            });

                            $('[title="Insert image"]').click(function () {
                                scope.openImageSelector();
                            });

                            $('[title="Insert file"]').click(function (e) {
                                scope.openDocumentSelector();
                            });
                        };
                    },
                    post: function (scope, element) {
                        scope.htmlViewLabel = 'HTML';

                        var htmlElement = element;
                        var isInHtmlView = false;
                        var isFullScreen = false;
                        var fullToolbar = null;
                        var shortToolbar = null;
                        var customButtons = null;

                        function getPropertiesFromTag(type, wrapperClass, tag) {
                            var range = scope.editor.getRange();
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
                            var command = scope.editor.tools.createLink.command({ range: range });

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
                            var range = scope.editor.getRange();
                            var anchor = getAnchorElement(range);
                            if (anchor) {
                                $(anchor).html(newMarkup);
                                return anchor.outerHTML;
                            }
                            else {
                                return newMarkup;
                            }
                        }

                        function addMissingAttributesToPropertyModal (modalId, templateUrl, dialogController) {
                            var el = document.getElementById(modalId);

                            el.setAttribute("template-url", templateUrl);
                            el.setAttribute("dialog-controller", dialogController);
                            $compile(el)(scope);
                        }

                        scope.openLinkSelector = function () {
                            var range = scope.editor.getRange();
                            var aTag = getAnchorElement(range);

                            if (aTag) {
                                scope.selectedHtml = aTag;
                            }
                            else {
                                scope.selectedHtml = scope.editor.selectedHtml();
                            }

                            angular.element(htmlElement.find("#linkSelectorModal")).scope().$openModalDialog().then(function (data) {
                                if (data) {
                                    scope.selectedHtml = data;
                                    scope.editor.exec("insertHtml", { html: data.outerHTML, split: true, range: range });
                                }
                            });
                        };

                        scope.openDocumentSelector = function () {
                            var mediaPropertiesDialog =
                                    serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/fields/html-field/sf-document-properties-content-block.sf-cshtml');
                            var sfMediaPropertiesController = "sfDocumentPropertiesController";

                            var range = scope.editor.getRange();
                            var aTag = getAnchorElement(range);
                            var properties = aTag ? mediaMarkupService.document.properties(aTag.outerHTML) : null;

                            addMissingAttributesToPropertyModal("mediaPropertiesModal", mediaPropertiesDialog, sfMediaPropertiesController);

                            setTimeout(function () {
                                angular.element(htmlElement.find('#mediaPropertiesModal'))
                                    .scope()
                                    .$openModalDialog({ sfModel: function () { return properties; } })
                                    .then(function (data) {
                                        properties = data;
                                        if (scope.sfDocumentsSettings)
                                            return;

                                        return mediaService.getLibrarySettings();
                                    }, function () { })
                                    .then(function (settings) {
                                        if (properties) {
                                            var markup = mediaMarkupService.document.markup(properties, settings);
                                            scope.editor.exec('insertHtml', { html: markup, split: true, range: range });
                                        }
                                    });
                            }, 0);
                        };

                        scope.openImageSelector = function () {
                            var mediaPropertiesDialog =
                               serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/fields/html-field/sf-image-properties-content-block.sf-cshtml');
                            var sfMediaPropertiesController = "sfImagePropertiesController";

                            var range = scope.editor.getRange();
                            var properties = getPropertiesFromTag('image', 'span.sf-Image-wrapper', 'img');

                            addMissingAttributesToPropertyModal("mediaPropertiesModal", mediaPropertiesDialog, sfMediaPropertiesController);

                            setTimeout(function () {
                                angular.element(htmlElement.find('#mediaPropertiesModal'))
                                    .scope()
                                    .$openModalDialog({ sfModel: function () { return properties; } })
                                    .then(function (data) {
                                        if (data) {
                                            properties = data;

                                            if (data.customSize)
                                                return mediaService.checkCustomThumbnailParams(data.customSize.Method, data.customSize, scope.sfImagesSettings);
                                            else
                                                return '';
                                        }
                                    }, function () { })
                                    .then(function (errorMessage) {
                                        if (properties && properties.thumbnail && properties.thumbnail.url) {
                                            return properties.thumbnail.url;
                                        }
                                        else if (properties && properties.customSize) {
                                            return mediaService.getCustomThumbnailUrl(properties.item.Id, properties.customSize);
                                        }
                                        else {
                                            return '';
                                        }
                                    })
                                    .then(function (thumbnailUrl) {
                                        if (properties && thumbnailUrl) {
                                            properties.thumbnail = properties.thumbnail || {};
                                            properties.thumbnail.url = thumbnailUrl;
                                        }

                                        if (scope.sfImagesSettings)
                                            return;

                                        return mediaService.getLibrarySettings();
                                    })
                                    .then(function (settings) {
                                        if (properties) {
                                            var wrapIt = true;
                                            var markup = mediaMarkupService.image.markup(properties, settings, wrapIt);

                                            // If the image is wrapped in a link, insertHtml will override the link with the image,
                                            // so we have to preserve the anchor and put the image inside it.
                                            markup = preserveWrapperATag(markup);

                                            scope.editor.exec('insertHtml', { html: markup, split: true, range: range });
                                        }
                                    });
                            }, 0);
                        };

                        scope.openVideoSelector = function () {
                            scope.mediaPropertiesDialog =
                                    serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/fields/html-field/sf-video-properties-content-block.sf-cshtml');
                            scope.sfMediaPropertiesController = "sfVideoPropertiesController";

                            var range = scope.editor.getRange();
                            var properties = getPropertiesFromTag('video', null, 'video');

                            setTimeout(function () {
                                angular.element(htmlElement.find('.mediaPropertiesModal'))
                                    .scope()
                                    .$openModalDialog({ sfModel: function () { return properties; } })
                                    .then(function (data) {
                                        properties = data;

                                        if (scope.sfVideosSettings)
                                            return;

                                        return mediaService.getLibrarySettings();
                                    }, function () { })
                                    .then(function (settings) {
                                        if (properties) {
                                            var markup = mediaMarkupService.video.markup(properties, settings);
                                            scope.editor.exec('insertHtml', { html: markup, split: true, range: range });
                                        }
                                    });
                            }, 0);
                        };

                        $(".show-all-button").click(function () {
                            scope.toggleAllTools();
                        });

                        $(".js-htmlview").click(function () {
                            scope.toggleHtmlView();
                        });

                        $(".toggle-full-screen").click(function () {
                            scope.toggleFullScreen();
                        });

                        $(".open-video").click(function () {
                            scope.openVideoSelector();
                        });

                        scope.toggleHtmlView = function () {
                            if (scope.editor === null)
                                return;

                            if (isInHtmlView === false) {
                                $(htmlElement.find(".js-htmlview")).addClass("active");

                                var htmlEditor = $('<textarea class="html k-content" ng-model="sfModel" style="resize: none">');
                                $compile(htmlEditor)(scope);
                                htmlEditor.insertAfter(scope.content);
                                scope.content.hide();

                                if (!fullToolbar) {
                                    fullToolbar = $(htmlElement.find(".k-editor-toolbar"));
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
                                scope.beautify();
                            } else {
                                $(htmlElement.find(".js-htmlview")).removeClass("active");
                                fullToolbar.removeClass("sf-toolbar-full");
                                shortToolbar.removeClass("sf-toolbar-short");

                                scope.sfModel = scope.editor.value();

                                shortToolbar.hide();
                                fullToolbar.show();
                                fullToolbar.append(customButtons);

                                var html = scope.editor.wrapper.find('.html');
                                html.remove();
                                scope.content.show();
                            }

                            isInHtmlView = !isInHtmlView;
                        };

                        scope.toggleFullScreen = function () {
                            if (scope.editor === null) {
                                return;
                            }

                            scope.fullScreenIcon = htmlElement.find(".js-fullScreen");

                            var modalHeaderAndFooter = htmlElement.find(".modal-dialog > .modal-content > .modal-header, .modal-dialog > .modal-content > .modal-footer");

                            var mainDialog = htmlElement.closest(".modal-dialog");

                            if (isFullScreen === false) {
                                mainDialog.addClass("modal-full-screen");
                                scope.fullScreenIcon.addClass("sf-minimize");
                            }
                            else {
                                mainDialog.removeClass("modal-full-screen");
                                scope.fullScreenIcon.removeClass("sf-minimize");

                            }

                            modalHeaderAndFooter.toggle();
                            isFullScreen = !isFullScreen;
                        };

                        scope.$on("close", function () {
                            scope.sfModel = scope.editor.value();
                        });

                        scope.beautify = function () {

                            if (!html_beautify) return;

                            var source = scope.editor.value();
                            var opts = {};
                            var output = html_beautify(source, opts);

                            scope.sfModel = output;
                        };
                    }
                }
            };
        }])
        .controller('sfImagePropertiesController', ['$scope', '$uibModalInstance', 'serverContext', 'sfModel',
            function ($scope, $uibModalInstance, serverContext, sfModel) {
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
                    $uibModalInstance.close($scope.model);
                };

                $scope.cancel = function () {
                    $uibModalInstance.dismiss();
                };

                $scope.thumbnailSizeTempalteUrl = serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/selectors/media/sf-thumbnail-size-selection.html');
            }])
        .controller('sfDocumentPropertiesController', ['$scope', '$uibModalInstance', 'serverContext', 'sfModel',
            function ($scope, $uibModalInstance, serverContext, sfModel) {

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
                    $uibModalInstance.close($scope.model);
                };

                $scope.cancel = function () {
                    $uibModalInstance.dismiss();
                };
            }])
        .controller('sfVideoPropertiesController', ['$scope', '$uibModalInstance', 'serverContext', 'sfModel',
            function ($scope, $uibModalInstance, serverContext, sfModel) {

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

                    $uibModalInstance.close($scope.model);
                };

                $scope.cancel = function () {
                    $uibModalInstance.dismiss();
                };
            }]);
})(jQuery);