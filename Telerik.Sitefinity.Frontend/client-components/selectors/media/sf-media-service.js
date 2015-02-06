(function () {
    angular.module('sfServices').factory('sfMediaService', ['serviceHelper', 'serverContext', function (serviceHelper, serverContext) {
        var constants = {
            images: {
                itemType: 'Telerik.Sitefinity.Libraries.Model.Image',
                albumsServiceUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/AlbumService.svc/folders/'),
                imagesServiceUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/ImageService.svc/')
            }
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
                    callback = imagesObj.getMedia;
                }
                else if (filterObject.basic) {
                    // Defaul filter is used (Recent / My / All)
                    if (filterObject.basic === filterObject.constants.basic.recentItems) {

                        // When the filter is Recent items, the number of displayed items is fixed and we should not append more.
                        if (appendItems) return;

                        callback = imagesObj.getMedia;
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

                return callback(options);
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
            }
        };

        return {
            images: imagesObj
        };
    }]);
})();