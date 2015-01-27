(function ($) {
    angular.module('sfSelectors')
        .directive('sfSelectedPagesView', ['serverContext', function (serverContext) {
            return {
                require: '^sfSelectedItemsView',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {

                        ctrl.isItemSelected = function (id, externalPageId) {
                            if (scope.sfSelectedItems) {
                                for (var i = 0; i < scope.sfSelectedItems.length; i++) {
                                    if ((id && scope.sfSelectedItems[i].Id === id) ||
                                        (externalPageId && scope.sfSelectedItems[i].ExternalPageId === externalPageId)) {
                                        return true;
                                    }
                                }
                            }

                            return false;
                        };

                        ctrl.itemClicked = function (item) {
                            if (!scope.sfSelectedItems) {
                                scope.sfSelectedItems = [];
                            }

                            var selectedItemIndex;
                            var alreadySelected = false;
                            for (var i = 0; i < scope.sfSelectedItems.length; i++) {
                                if ((item.Id && scope.sfSelectedItems[i].Id === item.Id)||
                                    (!item.Id && scope.sfSelectedItems[i].ExternalPageId === item.ExternalPageId)) {
                                    selectedItemIndex = i;
                                    alreadySelected = true;
                                    break;
                                }
                            }

                            if (alreadySelected) {
                                scope.sfSelectedItems.splice(selectedItemIndex, 1);
                            }
                            else {
                                scope.sfSelectedItems.push(item);
                            }
                        };
                    }
                }
            };
        }]);
})(jQuery);
