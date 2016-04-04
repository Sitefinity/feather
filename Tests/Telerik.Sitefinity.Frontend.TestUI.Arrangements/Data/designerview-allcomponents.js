var simpleViewModule = angular.module('simpleViewModule', ['expander', 'sfCollection', 'sfSelectors', 'sfFields', 'sfSearchBox',
    'sfSortBox', 'sfAspectRatioSelection', 'sfThumbnailSizeSelection']);
angular.module('designer').requires.push('simpleViewModule');