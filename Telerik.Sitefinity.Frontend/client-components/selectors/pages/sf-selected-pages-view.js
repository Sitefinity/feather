(function ($) {
    angular.module('sfSelectors')
        .directive('sfSelectedPagesView', ['serverContext', function (serverContext) {
            return {
                require: '^sfSelectedItemsView',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        var baseScope = scope.$$childTail;
                        ctrl.isItemSelected = function (id) {
                            if (baseScope.sfSelectedItems) {
                                for (var i = 0; i < baseScope.sfSelectedItems.length; i++) {
                                    if (id && baseScope.sfSelectedItems[i].Id === id) {
                                        return true;
                                    }
                                }
                            }

                            return false;
                        };

                        ctrl.itemClicked = function (item) {
                            if (!baseScope.sfSelectedItems) {
                                baseScope.sfSelectedItems = [];
                            }

                            var selectedItemIndex;
                            var alreadySelected = false;
                            for (var i = 0; i < baseScope.sfSelectedItems.length; i++) {
                                if (item.Id && baseScope.sfSelectedItems[i].Id === item.Id) {
                                    selectedItemIndex = i;
                                    alreadySelected = true;
                                    break;
                                }
                            }

                            if (alreadySelected) {
                                baseScope.sfSelectedItems.splice(selectedItemIndex, 1);
                            }
                            else {
                                baseScope.sfSelectedItems.push(item);
                            }
                        };
                    }
                }
            };
        }]);
})(jQuery);
