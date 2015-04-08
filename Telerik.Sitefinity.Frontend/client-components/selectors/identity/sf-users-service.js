angular.module('sfServices')
.factory('sfUsersService', ['serviceHelper', 'sfGenericItemsService',
function (serviceHelper, sfGenericItemsService) {
    var itemType = 'Telerik.Sitefinity.Security.Model.User';
    var itemSurrogateType = 'Telerik.Sitefinity.Security.Web.Services.WcfMembershipUser';
    var sortExpression = 'UserName';

    function getUsers (ignoreAdminUsers, provider, allProviders, skip, take, search) {
        var filter = serviceHelper.filterBuilder()
            .searchFilter(search)
            .getFilter();

        var options = {
            itemType: itemType,
            itemSurrogateType: itemSurrogateType,
            skip: skip,
            take: take,
            sortExpression: sortExpression,
            providerName: provider,
            allProviders: allProviders,
            ignoreAdminUsers: ignoreAdminUsers,
            filter: filter
        };

        return sfGenericItemsService.getItems(options);
    }

    function getSpecificUsers (ids, ignoreAdminUsers, provider, allProviders) {
        var filter = serviceHelper.filterBuilder()
            .specificItemsFilter(ids)
            .getFilter();

        var options = {
            itemType: itemType,
            itemSurrogateType: itemSurrogateType,
            sortExpression: sortExpression,
            providerName: provider,
            allProviders: allProviders,
            ignoreAdminUsers: ignoreAdminUsers,
            filter: filter
        };

        return sfGenericItemsService.getItems(options);
    }

    return {
        getUsers: getUsers,
        getSpecificUsers: getSpecificUsers
    };
}]);