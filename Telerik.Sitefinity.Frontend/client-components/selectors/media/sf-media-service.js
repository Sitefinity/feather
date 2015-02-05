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
                    hierarchyMode: options.recursive ? null : 'true'
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
            getImages: function (options) {
                return getItems(options, 'true', constants.images.imagesServiceUrl, constants.images.itemType);
            },
            getContent: function (options) {
                return getItems(options, null, constants.images.imagesServiceUrl, constants.images.itemType);
            }
        };

        return {
            images: imagesObj
        };
    }]);
})();