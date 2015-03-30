(function ($) {
    angular.module('sfSelectors')
        .directive('sfTimespanSelector', ['$timeout', '$filter', function ($timeout, $filter) {

            this.filter = $filter;
            var self = this;

            return {
                restrict: 'E',
                transclude: true,
                scope: {
                    sfSelectedItem: '=?',                   
                    sfChange: '='
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/date-time/sf-timespan-selector.sf-cshtml';
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

                            if (item.periodType == 'periodToNow') {
                                var label = $('#periodToNow').parent().text().trim();
                                item.displayText = label + ' ' + item.timeSpanValue + ' ' + item.timeSpanInterval;
                            }
                            else if (item.periodType == 'customRange') {
                                var fromLabel = $('#fromLabel').text();
                                var toLabel = $('#toLabel').text();


                                if (item.fromDate && item.toDate)
                                    item.displayText = fromLabel + ' ' + _getFormatedDate(item.fromDate) + ' ' + toLabel.toLowerCase() + ' ' + _getFormatedDate(item.toDate);
                                else if (item.fromDate)
                                    item.displayText = fromLabel + ' ' + _getFormatedDate(item.fromDate);
                                else if (item.toDate)
                                    item.displayText = toLabel + ' ' + _getFormatedDate(item.toDate);
                            }
                            else {
                                item.displayText = $('#anyTime').parent().text().trim();
                            }
                        };

                        _getFormatedDate = function (date) {
                            if (!date)
                                return;

                            var format = 'd MMM, y';
                        
                            if (date.getHours() !== 0 || date.getMinutes() !== 0) {
                                format = 'd MMM, y H:mm';
                            }

                            var result = self.filter('date')(date, format);

                            return result;
                        };

                        clearErrors = function () {
                            scope.showError = false;
                            scope.errorMessage = '';
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
                                else {
                                    clearErrors();
                                }

                                return isValid;
                            }
                            else {
                                clearErrors();

                                return true;
                            }
                        };

                        // ------------------------------------------------------------------------
                        // Scope variables and setup
                        // ------------------------------------------------------------------------                      

                        scope.selectItem = function () {
                            if (validate(scope.selectedItemInTheDialog)) {
                                formatTimeSpanItem(scope.selectedItemInTheDialog);

                                if (scope.sfChange) {
                                    var changeArgs = {
                                        'newSelectedItem': scope.selectedItemInTheDialog,
                                        'oldSelectedItem': jQuery.extend(true, {}, scope.sfSelectedItem)
                                    };
                                    scope.sfChange.call(scope.$parent, changeArgs);
                                }

                                scope.sfSelectedItem = scope.selectedItemInTheDialog;

                                scope.$modalInstance.close();
                            }
                        };

                        scope.isItemSelected = function () {

                            if (scope.sfSelectedItem) {
                                return scope.sfSelectedItem.displayText !== "";
                            }

                            return false;
                        };

                        scope.cancel = function () {
                            try {
                                scope.showError = false;
                                scope.errorMessage = '';
                                scope.$modalInstance.close();
                            } catch (e) { }
                        };

                        scope.open = function () {
                            scope.$openModalDialog();

                            scope.showError = false;
                            scope.errorMessage = '';
                            scope.selectedItemInTheDialog = jQuery.extend(true, {}, scope.sfSelectedItem);
                        };

                        formatTimeSpanItem(scope.sfSelectedItem);
                    }
                }
            };
        }]);
})(jQuery);