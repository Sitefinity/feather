(function ($) {
    var sfFields = angular.module('sfFields');
    sfFields.requires.push('sfMediaField');

    angular.module('sfMediaField', ['sfServices'])
        .directive('sfMediaField', ['serverContext', 'sfMediaService', 'sfMediaFilter', function (serverContext, sfMediaService, sfMediaFilter) {
            return {
                restrict: "AE",
                scope: {
                    sfModel: '=',
                    sfMedia: '=?',
                    sfProvider: '=?',
                    sfAutoOpenSelector: '@'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/fields/media-field/sf-media-field.sf-cshtml';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    var autoOpenSelector = attrs.sfAutoOpenSelector !== undefined && attrs.sfAutoOpenSelector.toLowerCase() !== 'false';

                    var getDateFromString = function (dateStr) {
                        return (new Date(parseInt(dateStr.substring(dateStr.indexOf('Date(') + 'Date('.length, dateStr.indexOf(')')))));
                    };

                    var getMedia = function (id) {
                        sfMediaService.documents.getById(id, scope.sfProvider).then(function (data) {
                            if (data && data.Item) {
                                refreshScopeInfo(data.Item);
                            }
                        });
                    };

                    var refreshScopeInfo = function (item) {
                        scope.sfMedia = item;

                        scope.mediaSize = Math.ceil(item.TotalSize / 1024) + " KB";
                        scope.uploaded = getDateFromString(item.DateCreated);
                    };

                    scope.showEditPropertiesButton = (window && window.radopen);

                    var editDialog;
                    var editAllPropertiesUrl = serverContext.getRootedUrl('/Sitefinity/Dialog/ContentViewEditDialog?ControlDefinitionName=DocumentsBackend&ViewName=DocumentsBackendEdit&IsInlineEditingMode=true');

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

                    scope.editAllProperties = function () {
                        var parentId = scope.sfMedia.ParentId || scope.sfMedia.Library.Id;
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

                            getMedia(scope.model.selectedItems[0].Id || scope.model.selectedItems[0]);
                        }
                    };

                    scope.cancel = function () {
                        if (scope.sfModel === undefined) {
                            scope.sfModel = null;
                        }

                        scope.$modalInstance.dismiss();
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
                            scope.model.filterObject.set.parent.to(scope.sfMedia.FolderId || scope.sfMedia.Library.Id);
                        }

                        var mediaSelectorModalScope = angular.element('.mediaSelectorModal').scope();

                        if (mediaSelectorModalScope)
                            mediaSelectorModalScope.$openModalDialog();
                    };

                    // Initialize
                    if (scope.sfModel) {
                        getMedia(scope.sfModel);
                    }
                    else if (autoOpenSelector) {
                        scope.changeMedia();
                    }

                    scope.$on('sf-document-selector-document-uploaded', function (event, uploadedDocumentInfo) {
                        scope.sfProvider = scope.model.provider;
                        getMedia(uploadedDocumentInfo.ContentId);
                        scope.$modalInstance.dismiss();
                    });

                    scope.$watch('sfMedia.Id', function (newVal) {
                        scope.sfModel = newVal;
                    });
                }
            };
        }]);
})(jQuery);
