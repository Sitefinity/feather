angular.module('sfServices')
.factory('sfUsersService', ['serviceHelper', 'sfGenericItemsService',
function (serviceHelper, sfGenericItemsService) {
    var itemType = 'Telerik.Sitefinity.Security.Model.User';
    var itemSurrogateType = 'Telerik.Sitefinity.Security.Web.Services.WcfMembershipUser';
    var identifier = 'UserName';

    function getUsers (ignoreAdminUsers, provider, allProviders, skip, take, search) {
        var filter = serviceHelper.filterBuilder()
            .searchFilter(search, null, identifier)
            .getFilter();

        var options = {
            itemType: itemType,
            itemSurrogateType: itemSurrogateType,
            skip: skip,
            take: take,
            identifier: identifier,
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
            identifier: identifier,
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