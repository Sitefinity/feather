(function ($) {
    var sfFields = angular.module('sfFields');
    sfFields.requires.push('sfImageField');

    angular.module('sfImageField', ['sfServices', 'sfImageSelector'])
        .directive('sfImageField', ['serverContext', 'sfMediaService', 'sfMediaFilter', function (serverContext, sfMediaService, sfMediaFilter) {
            return {
                restrict: "AE",
                scope: {
                    sfModel: '=',
                    sfImage: '=?',
                    sfProvider: '=?',
                    sfAutoOpenSelector: '@'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/fields/image-field/sf-image-field.sf-cshtml';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    var autoOpenSelector = attrs.sfAutoOpenSelector !== undefined && attrs.sfAutoOpenSelector.toLowerCase() !== 'false';

                    var getDateFromString = function (dateStr) {
                        return (new Date(parseInt(dateStr.substring(dateStr.indexOf('Date(') + 'Date('.length, dateStr.indexOf(')')))));
                    };

                    var getImage = function (id) {
                        sfMediaService.images.getById(id, scope.sfProvider).then(function (data) {
                            if (data && data.Item) {
                                refreshScopeInfo(data.Item);
                            }
                        });
                    };

                    var refreshScopeInfo = function (item) {
                        scope.sfImage = item;

                        scope.imageSize = Math.ceil(item.TotalSize / 1024) + " KB";
                        scope.uploaded = getDateFromString(item.DateCreated);
                    };

                    scope.showEditPropertiesButton = (window && window.radopen);

                    var editDialog;
                    var editAllPropertiesUrl = serverContext.getRootedUrl('/Sitefinity/Dialog/ContentViewEditDialog?ControlDefinitionName=ImagesBackend&ViewName=ImagesBackendEdit&IsInlineEditingMode=true');

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
                                Id: scope.sfImage.Id,
                                ProviderName: scope.sfProvider
                            },
                            dialog: dialog,
                            params: {
                                IsEditable: true,
                                parentId: parentId
                            },
                            key: { Id: scope.sfImage.Id },
                            commandArgument: { languageMode: 'edit', language: serverContext.getUICulture() }
                        };

                        return dialogContext;
                    };

                    var closeEditAllProperties = function (sender, args) {
                        if (args && args.get_argument() && args.get_argument() == 'rebind') {
                            getImage(scope.sfModel);
                        }
                        sender.remove_close(closeEditAllProperties);
                    };

                    scope.editAllProperties = function () {
                        var parentId = scope.sfImage.ParentId || scope.sfImage.Album.Id;
                        var fullEditAllPropertiesUrl = editAllPropertiesUrl + ('&parentId=' + parentId);

                        var dialogManager = window.top.GetDialogManager();
                        editDialog = createDialog(dialogManager);
                        editDialog.setUrl(fullEditAllPropertiesUrl);

                        var dialogName = editDialog.get_name();
                        var dialogContext = getDialogContext(editDialog, parentId);

                        dialogManager.openDialog(dialogName, null, dialogContext);
                    };

                    scope.done = function () {
                        scope.$modalInstance.close();

                        if (scope.model.selectedItems && scope.model.selectedItems.length) {
                            scope.sfProvider = scope.model.provider;

                            // Id is passed -> get image
                            getImage(scope.model.selectedItems[0].Id || scope.model.selectedItems[0]);
                        }
                    };

                    scope.cancel = function () {
                        // cancels the image properties if no image is selected
                        if (scope.sfModel === undefined) {
                            scope.sfModel = null;
                        }

                        scope.$modalInstance.dismiss();
                    };

                    scope.changeImage = function () {
                        scope.model = {
                            selectedItems: [],
                            filterObject: null,
                            provider: scope.sfProvider
                        };

                        if (scope.sfImage && scope.sfImage.Id) {
                            scope.model.selectedItems.push(scope.sfImage);
                            scope.model.filterObject = sfMediaFilter.newFilter();
                            scope.model.filterObject.set.parent.to(scope.sfImage.FolderId || scope.sfImage.Album.Id);
                        }

                        var imageSelectorModalScope = element.find('.imageSelectorModal').scope();

                        if (imageSelectorModalScope)
                            imageSelectorModalScope.$openModalDialog();
                    };

                    // Initialize
                    if (scope.sfModel) {
                        getImage(scope.sfModel);
                    }
                    else if (autoOpenSelector) {
                        scope.changeImage();
                    }

                    scope.$on('sf-image-selector-image-uploaded', function (event, uploadedImageInfo) {
                        scope.sfProvider = scope.model.provider;
                        getImage(uploadedImageInfo.ContentId);
                        scope.$modalInstance.dismiss();
                    });

                    scope.$watch('sfImage.Id', function (newVal) {
                        scope.sfModel = newVal;
                    });
                }
            };
        }]);
})(jQuery);
