var simpleViewModule = angular.module('simpleViewModule', ['expander', 'sfSelectors', 'sfFields', 'sfTree',
    'sfCollection', 'sfInfiniteScroll', 'kendo.directives', 'sfDragDrop', 'sfFileUrlField', 'sfFlatTaxonField',
    'sfFormField', 'sfHtmlField', 'sfCodeArea', 'sfSearchBox', 'sfSortBox', 'sfBootstrapPopover', 'sfFileUrlSelector', 'sfImageSelector', 'sfDocumentSelector',
    'sfVideoSelector', 'sfMediaSelector', 'sfThumbnailSizeSelection', 'sfAspectRatioSelection']);
angular.module('designer').requires.push('simpleViewModule');
simpleViewModule.controller('SimpleCtrl', ['$scope', function ($scope) {

}]);
// ADD ALL COMPONENTS

