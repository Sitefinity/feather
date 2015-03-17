; (function ($) {
    var sfSelectors = angular.module('sfSelectors');
    sfSelectors.requires.push('sfDocumentSelector');

    var sfDocumentSelector = angular.module('sfDocumentSelector', ['sfMediaSelector']);
    sfDocumentSelector.directive('sfDocumentSelector', ['sfMediaSelector', function (sfMediaSelector) {

    }]);
})(jQuery);