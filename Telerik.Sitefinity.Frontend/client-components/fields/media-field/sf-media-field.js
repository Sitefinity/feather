﻿(function ($) {
    var sfFields = angular.module('sfFields');
    sfFields.requires.push('sfMediaField');
    sfFields.requires.push('serverDataModule');

    angular.module('sfMediaField', ['sfServices'])
        .service("sfMediaTypeResolver", ['sfMediaService', 'serverContext', function (sfMediaService, serverContext) {
            var Document = function () {
                this.getEditAllPropertiesUrl = function () {
                    return serverContext.getRootedUrl('/Sitefinity/Dialog/ContentViewEditDialog?ControlDefinitionName=DocumentsBackend&ViewName=DocumentsBackendEdit&IsInlineEditingMode=true');
                };
                this.getMediaUploadedEvent = function () {
                    return 'sf-document-selector-document-uploaded';
                };
                this.service = sfMediaService.documents;

                this.getParentId = function (document) {
                    return document.FolderId || document.Library.Id;
                };

                this.getDefaultSelectorTemplateUrl = function () {
                    return serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/fields/document-field/sf-document-modal-template.sf-cshtml');
                };

                this.getDefaultTemplateUrl = function () {
                    return serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/fields/document-field/sf-document-field.sf-cshtml');
                };
            };

            var Videos = function () {
                this.getEditAllPropertiesUrl = function () {
                    return serverContext.getRootedUrl('/Sitefinity/Dialog/ContentViewEditDialog?ControlDefinitionName=VideosBackend&ViewName=VideosBackendEdit&IsInlineEditingMode=true');
                };
                this.getMediaUploadedEvent = function () {
                    return 'sf-media-selector-item-uploaded';
                };
                this.service = sfMediaService.videos;

                this.getParentId = function (video) {
                    return video.FolderId || video.Library.Id;
                };

                this.getDefaultSelectorTemplateUrl = function () {
                    return serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/fields/video-field/sf-video-modal-template.sf-cshtml');
                };

                this.getDefaultTemplateUrl = function () {
                    return serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/fields/video-field/sf-video-field.sf-cshtml');
                };
            };

            var Images = function () {
                this.getEditAllPropertiesUrl = function () {
                    return serverContext.getRootedUrl('/Sitefinity/Dialog/ContentViewEditDialog?ControlDefinitionName=ImagesBackend&ViewName=ImagesBackendEdit&IsInlineEditingMode=true');
                };
                this.getMediaUploadedEvent = function () {
                    return 'sf-image-selector-image-uploaded';
                };
                this.service = sfMediaService.images;

                this.getParentId = function (image) {
                    return image.FolderId || image.Album.Id;
                };

                this.getDefaultSelectorTemplateUrl = function () {
                    return '';
                };

                this.getDefaultTemplateUrl = function () {
                    return '';
                };
            };

            this.get = function (mediaType) {
                if (mediaType === 'documents') {
                    return new Document();
                }
                if (mediaType === "videos") {
                    return new Videos();
                }
                return new Images();
            };
        }])
        .directive('sfMediaField', ['serverContext', 'sfMediaService', 'sfMediaFilter', 'sfMediaTypeResolver', 'serverData',
        function (serverContext, sfMediaService, sfMediaFilter, sfMediaTypeResolver, serverData) {
            return {
                restrict: "AE",
                scope: {
                    sfModel: '=',
                    sfMedia: '=?',
                    sfProvider: '=?',
                    sfAutoOpenSelector: '@',
                    sfMediaType: '@',
                    sfSelectorModelTemplate: '@',
                    sfMediaSettings: '=?'
                },
                controller: function ($scope) {
                    if (!$scope.sfSelectorModelTemplate) {
                        var mediaType = sfMediaTypeResolver.get($scope.sfMediaType);
                        $scope.sfSelectorModelTemplate = mediaType.getDefaultSelectorTemplateUrl();
                    }
                },
                templateUrl: function (elem, attrs) {
                    if (attrs.sfMediaType && !attrs.sfTemplateUrl) {
                        var mediaType = sfMediaTypeResolver.get(attrs.sfMediaType);
                        return mediaType.getDefaultTemplateUrl();
                    }
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/fields/media-field/sf-media-field.sf-cshtml';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    var emptyGuid = '00000000-0000-0000-0000-000000000000';

                    serverData.refresh();
                    scope.labels = serverData.getAll();
                    scope.mediaItemDeleted = false;

                    var autoOpenSelector = attrs.sfAutoOpenSelector !== undefined && attrs.sfAutoOpenSelector.toLowerCase() !== 'false';
                    
                    var mediaElement = element;
                    
                    var mediaType = sfMediaTypeResolver.get(scope.sfMediaType);

                    var getDateFromString = function (dateStr) {
                        return (new Date(parseInt(dateStr.substring(dateStr.indexOf('Date(') + 'Date('.length, dateStr.indexOf(')')))));
                    };

                    var getMedia = function (id) {
                        mediaType.service.getById(id, scope.sfProvider).then(function (data) {
                            if (data && data.Item && data.Item.Visible) {
                                refreshScopeInfo(data.Item);
                            } else {
                                refreshScopeInfo(null);
                            }
                        }, function (err) {
                            refreshScopeInfo(null);
                        });
                    };

                    var refreshScopeInfo = function (item) {
                        if (item) {
                            scope.mediaItemDeleted = false;
                            scope.sfMedia = item;
                            scope.mediaSize = Math.ceil(item.TotalSize / 1024) + " KB";
                            scope.uploaded = getDateFromString(item.DateCreated);
                        } else {
                            scope.mediaItemDeleted = true;
                            scope.sfMedia = null;
                            scope.mediaSize = null;
                            scope.uploaded = null;
                        }
                    };

                    scope.showEditPropertiesButton = (window && window.radopen);

                    var editDialog;
                    var editAllPropertiesUrl = mediaType.getEditAllPropertiesUrl();

                    var createDialog = function (dialogManager) {
                        editWindow = window.radopen(editAllPropertiesUrl);
                        var dialogName = editWindow.get_name();
                        var dialog = dialogManager.getDialogByName(dialogName);
                        dialog.add_close(closeEditAllProperties);

                        return dialog;
                    };

                    var getDialogContext = function (dialog, parentId) {
                        var itemsList = {};
                        itemsList.getBinder = function () {
                            var binder = {};
                            binder.get_provider = function () {
                                return scope.sfProvider;
                            };
                            return binder;
                        };

                        var dialogContext = {
                            commandName: 'edit',
                            itemsList: itemsList,
                            dataItem: {
                                Id: scope.sfMedia.Id,
                                ProviderName: scope.sfProvider
                            },
                            dialog: dialog,
                            params: {
                                IsEditable: true,
                                parentId: parentId
                            },
                            key: { Id: scope.sfMedia.Id },
                            commandArgument: { languageMode: 'edit', language: serverContext.getUICulture() }
                        };

                        return dialogContext;
                    };

                    var closeEditAllProperties = function (sender, args) {
                        if (args && args.get_argument() && args.get_argument() == 'rebind') {
                            getMedia(scope.sfModel);
                        }
                        sender.remove_close(closeEditAllProperties);
                    };

                    var forceAngularJsDigest = function () {
                        scope.$digest();

                        document.removeEventListener("forceAngularJsDigest", forceAngularJsDigest);
                    };

                    var getCultureFromUrl = function () {
                        var editSegment = "Edit/";
                        var path = this.window.location.pathname;
                        var culture = path.substr(path.indexOf(editSegment) + editSegment.length);
                        return culture;
                    };

                    scope.editAllProperties = function () {
                        var parentId = scope.sfMedia.ParentId || scope.sfMedia.Library.Id;

                        var isAdminAppActive = window.location.pathname.toLowerCase().includes("adminapp");

                        if (isAdminAppActive) {
                            var irisEvent = new CustomEvent("openIrisContent", {
                                detail: {
                                    parentId: parentId,
                                    itemId: scope.sfMedia.OriginalContentId,
                                    mediaType: scope.sfMediaType,
                                    provider: scope.sfProvider,
                                    culture: serverContext.getUICulture() || getCultureFromUrl()
                                }
                            });
                            document.dispatchEvent(irisEvent);

                            document.addEventListener("forceAngularJsDigest", forceAngularJsDigest);
                        } else {
                            var fullEditAllPropertiesUrl = editAllPropertiesUrl + ('&parentId=' + parentId);

                            var dialogManager = window.top.GetDialogManager();
                            editDialog = createDialog(dialogManager);
                            editDialog.setUrl(fullEditAllPropertiesUrl);

                            var dialogName = editDialog.get_name();
                            var dialogContext = getDialogContext(editDialog, parentId);

                            dialogManager.openDialog(dialogName, null, dialogContext);
                        }   
                    };

                    scope.done = function () {
                        scope.$uibModalInstance.close();

                        if (scope.model.selectedItems && scope.model.selectedItems.length) {
                            scope.sfProvider = scope.model.provider;

                            getMedia(scope.model.selectedItems[0].Id || scope.model.selectedItems[0]);
                        }
                    };

                    scope.cancel = function () {
                        if (scope.sfModel === undefined) {
                            scope.sfModel = null;
                        }

                        scope.$uibModalInstance.dismiss();
                    };

                    scope.changeMedia = function () {
                        scope.model = {
                            selectedItems: [],
                            filterObject: null,
                            provider: scope.sfProvider
                        };

                        if (scope.sfMedia && scope.sfMedia.Id) {
                            scope.model.selectedItems.push(scope.sfMedia);
                            scope.model.filterObject = sfMediaFilter.newFilter();
                            scope.model.filterObject.set.parent.to(mediaType.getParentId(scope.sfMedia));
                            scope.model.filterObject.status = attrs.sfMaster === 'true' || attrs.sfMaster === 'True' ? 'master' : 'live';
                        }

                        var mediaSelector = angular.element('.mediaSelectorModal');

                        //Add below lines of code to solve multiple sf-media-field selector in same designer view
                        if (mediaElement && mediaElement.length > 0) {
                            var el = mediaElement[0].querySelector('.mediaSelectorModal');
                            if (el) {
                                var ngEl = angular.element(el);
                                if (ngEl) {
                                    mediaSelector = ngEl;
                                }
                            }
                        }

                        var mediaSelectorModalScope = mediaSelector.scope();

                        if (mediaSelectorModalScope)
                            mediaSelectorModalScope.$openModalDialog();
                    };

                    // Initialize
                    if (scope.sfModel && scope.sfModel !== emptyGuid) {
                        getMedia(scope.sfModel);
                    }
                    else if (autoOpenSelector) {
                        scope.changeMedia();
                    }

                    scope.$on(mediaType.getMediaUploadedEvent(), function (event, uploadedMediaInfo) {
                        if (!scope.model.provider && uploadedMediaInfo.ContentItem && uploadedMediaInfo.ContentItem.ProviderName) {
                            scope.model.provider = uploadedMediaInfo.ContentItem.ProviderName;
                        }

                        scope.sfProvider = scope.model.provider;
                        getMedia(uploadedMediaInfo.ContentId);
                        scope.$uibModalInstance.dismiss();
                    });

                    scope.$watch('sfMedia.Id', function (newVal) {
                        scope.sfModel = newVal;
                    });
                }
            };
        }])
        .controller('SfVideoFieldCtrl', ['$scope', function ($scope) {
            $scope.showVideo = false;

            $scope.playVideo = function (elementSelector) {
                $scope.showVideo = true;
                angular.element(elementSelector)[0].play();
            };

            $scope.pauseVideo = function (elementSelector) {
                $scope.showVideo = false;
                angular.element(elementSelector)[0].pause();
            };
        }]);
})(jQuery);
