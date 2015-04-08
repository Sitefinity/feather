angular.module('sfServices')
.factory('sfGenericItemsService', ['serviceHelper', 'serverContext', 
function (serviceHelper, serverContext) {
    var url = serverContext.getRootedUrl('Sitefinity/Services/Common/GenericItemsService.svc/');

    return {
        getItems: function getItems (options) {
            return serviceHelper.getResource(url).get(options).$promise;
        }
    };
}]);