; (function ($) {
    var sfSelectors = angular.module('sfSelectors');
    sfSelectors.requires.push('sfMediaSelector');

    var sfMediaSelector = angular.module('sfMediaSelector', ['sfServices', 'sfInfiniteScroll', 'sfCollection', 'sfTree', 'sfSearchBox', 'sfSortBox', 'sfDragDrop', 'expander']);
    sfMediaSelector.factory('sfMediaSelector', []);
})(jQuery);