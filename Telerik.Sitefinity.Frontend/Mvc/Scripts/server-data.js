(function ($) {
    var serverDataModule = angular.module('serverDataModule', []);

    serverDataModule.provider('serverData', function () {
        var serverData = {};
        
        var serverDataService = {
            get: function (key) {
                return serverData[key];
            },

            set: function (key, value) {
                serverData[key] = value;
                return value;
            },

            update: function () {
                $('server-data')
                    .each(function () {
                        $.each(this.attributes, function (i, attribute) {
                            serverData[$.camelCase(attribute.name)] = attribute.value;
                        });
                    })
                    .remove();
            }
        };

        return $.extend(serverDataService, {
            $get: function () {
                return serverDataService;
            }
        });
    });

    serverDataModule.directive('serverData', ['serverData', function (serverData) {
        return {
            restrict: 'E',
            replace: true,
            link: function (scope, element, attrs) {
                angular.forEach(attrs.$attrs, function (value, key) {
                    serverData.set(attrs[key], value);
                });
            }
        };
    }]);
})(jQuery);