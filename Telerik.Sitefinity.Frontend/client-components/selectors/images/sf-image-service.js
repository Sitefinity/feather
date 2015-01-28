(function () {
    angular.module('sfServices').factory('sfImageService', ['serviceHelper', 'serverContext', function (serviceHelper, serverContext) {
        var albumServiceUrl = serverContext.getRootedUrl('Sitefinity/Services/Content/AlbumService.svc/folders/'),
            imageServiceUrl = serverContext.getRootedUrl('Sitefinity/Services/Content/ImageService.svc/');

        var callImageService = function (options, excludeFolders) {
            options = options || {};

            var url = options.parent ? imageServiceUrl + 'parent/' + options.parent + "/" : imageServiceUrl;
            return serviceHelper.getResource(url).get(
                {
                    itemType: 'Telerik.Sitefinity.Libraries.Model.Image',
                    filter: options.filter,
                    provider: options.provider,
                    skip: options.skip,
                    take: options.take,
                    sortExpression: options.sort,
                    includeSubFolderItems: options.recursive ? 'true' : null,
                    excludeFolders: excludeFolders
                }).$promise;
        };

        var getImages = function (options) {
            return callImageService(options, 'true');
        };

        var getFolders = function (options) {
            options = options || {};

            var url = options.parent ? albumServiceUrl + options.parent + "/" : albumServiceUrl;
            return serviceHelper.getResource(url).get(
                {
                    filter: options.filter,
                    provider: options.provider,
                    skip: options.skip,
                    take: options.take,
                    sortExpression: options.sort,
                    hierarchyMode: options.recursive ? null : 'true'
                }).$promise;
        };

        var getContent = function (options) {
            return callImageService(options, null);
        };

        return {
            getImages: getImages,
            getFolders: getFolders,
            getContent: getContent
        };
    }]);
})();