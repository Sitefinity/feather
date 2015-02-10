(function () {
    angular.module('sfServices').factory('sfMediaService',
            ['$http',
             'serviceHelper',
             'serverContext',
             '$interpolate',
             '$q',
             '$window',
             function ($http, serviceHelper, serverContext, $interpolate, $q, $window) {
                 var constants = {
                     images: {
                         itemType: 'Telerik.Sitefinity.Libraries.Model.Image',
                         albumsServiceUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/AlbumService.svc/folders/'),
                         imagesServiceUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/ImageService.svc/'),
                         createImageUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/ImageService.svc/parent/{{libraryId}}/{{itemId}}/?itemType={{itemType}}&provider={{provider}}&parentItemType={{parentItemType}}&newParentId={{newParentId}}')
                     },
                     albums: {
                         albumItemType: 'Telerik.Sitefinity.Libraries.Model.Album',
                     },
                     files: {
                         uploadHandlerUrl: serverContext.getRootedUrl('Telerik.Sitefinity.Html5UploadHandler.ashx'),
                     },
                     librarySettingsServiceUrl: serverContext.getRootedUrl('Sitefinity/Services/Configuration/ConfigSectionItems.svc/')
                 };

                 var getItems = function (options, excludeFolders, serviceUrl, itemType) {
                     options = options || {};

                     var url = options.parent ? serviceUrl + 'parent/' + options.parent + "/" : serviceUrl;

                     return serviceHelper.getResource(url).get(
                         {
                             itemType: itemType,
                             filter: options.filter,
                             provider: options.provider,
                             skip: options.skip,
                             take: options.take,
                             sortExpression: options.sort,
                             includeSubFolderItems: options.recursive ? 'true' : null,
                             excludeFolders: excludeFolders
                         }).$promise;
                 };

                 var getFolders = function (options, serviceUrl) {
                     options = options || {};

                     var url = options.parent ? serviceUrl + options.parent + "/" : serviceUrl;
                     return serviceHelper.getResource(url).get(
                         {
                             filter: options.filter,
                             provider: options.provider,
                             skip: options.skip,
                             take: options.take,
                             sortExpression: options.sort,
                             hierarchyMode: options.recursive ? null : 'true',
                             excludeNeighbours: options.excludeNeighbours
                         }).$promise.then(function (data) {
                             data.Items.map(function (obj) {
                                 obj.IsFolder = true;
                             });
                             return data;
                         });
                 };

                 var getEmptyArray = function () {
                     var defer = $q.defer();
                     defer.resolve([]);
                     return defer.promise;
                 };

                 var toWcfDate = function (date) {
                     return '/Date(' + date.getTime() + '-0000)/';
                 };

                 var createImage = function (settings) {
                     var nowToWcfDate = toWcfDate(new Date()),
                         url = $interpolate(constants.images.createImageUrl)(settings),
                         image = {
                             Item: {
                                 Title: {
                                     PersistedValue: settings.title,
                                     Value: settings.title
                                 },
                                 AlternativeText: {
                                     PersistedValue: settings.alternativeText,
                                     Value: settings.alternativeText
                                 },
                                 DateCreated: nowToWcfDate,
                                 PublicationDate: nowToWcfDate,
                                 LastModified: nowToWcfDate,
                                 Tags: settings.tags,
                                 Category: settings.categories
                             }
                         };

                     return $http.put(url, image);
                 };

                 var FileUpload = function () {
                     var deferred = $q.defer();

                     FileUpload.prototype.upload = function (url, formData) {
                         var xhr = new $window.XMLHttpRequest();
                         xhr.onload = function (e) {
                             if (xhr.readyState === 4) {
                                 if (xhr.status === 200) {
                                     deferred.resolve(JSON.parse(xhr.responseText));
                                 } else {
                                     deferred.reject(xhr.statusText);
                                 }
                             }
                         };
                         xhr.upload.onprogress = function (e) {
                             var done = e.position || e.loaded,
                                 total = e.totalSize || e.total,
                                 present = Math.floor(done / total * 100);
                             deferred.notify(present);
                         };
                         xhr.onerror = function (e) {
                             deferred.reject(xhr.statusText);
                         };

                         xhr.open('POST', url);
                         xhr.send(formData);
                     };

                     FileUpload.prototype.getDeferred = function () {
                         return deferred;
                     };
                 };

                 var uploadImage = function (settings) {
                     var fileUpload = new FileUpload();

                     var deferred = fileUpload.getDeferred();

                     createImage(settings)
                         .success(function (data) {
                             var formData = new FormData();
                             formData.append('ContentType', constants.images.itemType);
                             formData.append('LibraryId', settings.libraryId);
                             formData.append('ContentId', data.Item.Id);
                             formData.append('Workflow', 'Upload');
                             formData.append('ProviderName', settings.provider || '');
                             formData.append('SkipWorkflow', 'true');
                             formData.append('ImageFile', settings.file);

                             fileUpload.upload(constants.files.uploadHandlerUrl, formData);
                         })
                         .error(function () {
                             deferred.reject('Image creation error!');
                         });

                     return deferred.promise;
                 };

                 var imagesObj = {
                     getFolders: function (options) {
                         return getFolders(options, constants.images.albumsServiceUrl);
                     },
                     getMedia: function (options) {
                         return getItems(options, 'true', constants.images.imagesServiceUrl, constants.images.itemType);
                     },
                     getContent: function (options) {
                         return getItems(options, null, constants.images.imagesServiceUrl, constants.images.itemType);
                     },
                     get: function (options, filterObject, appendItems) {
                         var callback;
                         if (filterObject.query) {
                             callback = imagesObj.getContent;
                         }
                         else if (filterObject.basic) {
                             // Defaul filter is used (Recent / My / All)
                             if (filterObject.basic === filterObject.constants.basic.recentItems) {
                                 // When the filter is Recent items, the number of displayed items is fixed and we should not append more.
                                 if (appendItems) {
                                     callback = getEmptyArray;
                                 }
                                 else {
                                     callback = imagesObj.getMedia;
                                 }
                             }
                             else if (filterObject.basic === filterObject.constants.basic.ownItems) {
                                 callback = imagesObj.getMedia;
                             }
                             else if (filterObject.basic === filterObject.constants.basic.allLibraries) {
                                 callback = imagesObj.getFolders;
                             }
                             else {
                                 throw { message: 'Unknown basic filter object option.' };
                             }
                         }
                         else {
                             // custom filter is used (Libraries / Taxons / Dates)
                             callback = imagesObj.getContent;
                         }

                         var callbackWrap = function () {
                             var allLanguageSearch = enableAllLanguagesSearch.toLowerCase() === "true";
                             options.filter = filterObject.composeExpression(allLanguageSearch);
                             return callback(options);
                         };

                         if (enableAllLanguagesSearch === null) {
                             return getEnableAllLanguagesSearch()
                                 .then(callbackWrap);
                         }
                         else {
                             return callbackWrap();
                         }
                     },
                     getPredecessorsFolders: function (id) {
                         if (!id) {
                             return;
                         }
                         var options = {
                             parent: 'predecessors/' + id,
                             excludeNeighbours: true
                         };
                         return getFolders(options, constants.images.albumsServiceUrl)
                                   .then(function (data) {
                                       return data.Items;
                                   });
                     },
                     upload: function (model) {

                         var defaultLibraryId = '4ba7ad46-f29b-4e65-be17-9bf7ce5ba1fb';
                         var libraryId = model.ParentId || defaultLibraryId;

                         var settings = {
                             libraryId: libraryId,
                             newParentId: libraryId,
                             itemId: serviceHelper.emptyGuid(),
                             itemType: constants.images.itemType,
                             provider: model.provider,
                             parentItemType: constants.albums.albumItemType,
                             title: model.Title || model.file.name,
                             alternativeText: model.AlternativeText,
                             categories: model.Categories,
                             tags: model.Tags
                         };
                         return uploadImage(settings);
                     }
                 };

                 var enableAllLanguagesSearch = null;

                 var getLibrarySettings = function () {
                     var url = constants.librarySettingsServiceUrl;
                     return serviceHelper.getResource(url).get(
                         {
                             nodeName: 'librariesConfig_0',
                             mode: 'Form'
                         }).$promise;
                 };

                 var getEnableAllLanguagesSearch = function () {
                     if (enableAllLanguagesSearch === null) {
                         return getLibrarySettings().then(function (data) {
                             if (data && data.Items) {
                                 enableAllLanguagesSearch = data.Items.filter(function (item) { return item.Key == "EnableAllLanguagesSearch"; })[0].Value;
                             }
                             else {
                                 enableAllLanguagesSearch = false;
                             }

                             return enableAllLanguagesSearch;
                         });
                     } else {
                         var deferred = $q.defer();
                         deferred.resolve(enableAllLanguagesSearch);
                         return deferred.promise;
                     }
                 };

                 return {
                     images: imagesObj
                 };
             }]);
})();