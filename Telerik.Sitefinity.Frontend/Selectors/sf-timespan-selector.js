(function ($) {
    angular.module('selectors')
        .directive('sfTimespanSelector', ['$timeout', function ($timeout) {

            return {
                restrict: 'E',
                transclude: true,
                scope: {
                    selectedItem: '=?',                   
                    change: '='
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'Selectors/sf-timespan-selector.html';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    pre: function (scope, element, attrs, ctrl, transclude) {
                        // ------------------------------------------------------------------------
                        // helper methods
                        // ------------------------------------------------------------------------
                        formatTimeSpanItem = function (item) {
                            if (!item)
                                return;

                            if (item.periodType == 'periodToNow')
                                item.displayText = 'Last ' + item.timeSpanValue + ' ' + item.timeSpanInterval;
                            else if (item.periodType == "customRange") {
                                if (item.fromDate && item.toDate)
                                    item.displayText = item.fromDate.toLocaleString() + '-' + item.toDate.toLocaleString();
                                else if (item.fromDate)
                                    item.displayText = 'from: ' + item.fromDate.toLocaleString();
                                else if(item.toDate)
                                    item.displayText = 'to: ' + item.toDate.toLocaleString();
                            }
                            else
                                item.displayText = 'Any time';
                        };

                        validate = function (item) {
                            if (item.periodType == 'periodToNow' && !item.timeSpanValue) {
                                scope.errorMessage = 'Invalid period!';
                                scope.showError = true;

                                return false;
                            }
                            else if (item.periodType == 'customRange' && item.fromDate && item.toDate) {
                                var isValid = item.fromDate < item.toDate;

                                if (!isValid) {
                                    scope.errorMessage = 'Invalid date range! The expiration date must be after the publication date.';
                                    scope.showError = true;
                                }

                                return isValid;
                            }
                            else {
                                scope.showError = false;
                                scope.errorMessage = '';

                                return true;
                            }
                        };

                        // ------------------------------------------------------------------------
                        // Scope variables and setup
                        // ------------------------------------------------------------------------

                        var timeoutPromise = false;
                        var selectorId;
                        if (attrs.id) {
                            selectorId = attrs.id;
                        }
                        else {
                            //selectorId will be set to the id of the wrapper div of the template. This way we avoid issues when there are several selectors on one page.
                            selectorId = 'sf' + Math.floor((Math.random() * 1000) + 1);
                            scope.selectorId = selectorId;
                        }

                        // This id is used by the modal dialog to know which button will open him.
                        scope.openSelectorButtonId = '#' + selectorId + ' .openSelectorBtn';

                        scope.selectItem = function () {
                            if (validate(scope.selectedItemInTheDialog)) {
                                formatTimeSpanItem(scope.selectedItemInTheDialog);

                                if (scope.change) {
                                    var changeArgs = {
                                        'newSelectedItem': scope.selectedItemInTheDialog,
                                        'oldSelectedItem': jQuery.extend(true, {}, scope.selectedItem)
                                    };
                                    scope.change.call(scope.$parent, changeArgs);
                                }

                                scope.selectedItem = scope.selectedItemInTheDialog;

                                scope.$modalInstance.close();
                            }
                        };

                        scope.cancel = function () {
                            try {
                                scope.showError = false;
                                scope.errorMessage = '';
                                scope.$modalInstance.close();
                            } catch (e) { }
                        };

                        scope.open = function () {
                            scope.showError = false;
                            scope.errorMessage = '';
                            scope.selectedItemInTheDialog = jQuery.extend(true, {}, scope.selectedItem);
                        };

                        formatTimeSpanItem(scope.selectedItem);
                    }
                }
            };
        }]);
})(jQuery);