var simpleViewModule = angular.module('simpleViewModule', ['expander', 'sfCollection', 'sfSelectors', 'sfFields',
    'sfSortBox', 'sfAspectRatioSelection', 'sfThumbnailSizeSelection']);
angular.module('designer').requires.push('simpleViewModule');