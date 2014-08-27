(function ($) {
    var serverDataModule = angular.module('serverDataModule', []);

    serverDataModule.provider('serverData', function () {
        var serverData = {
            'applicationRoot': '/',
            'widgetName': 'MockedWidget'
        };

        var serverDataService = {
            get: function (key) {
                return serverData[key];
            },

            has: function (key) {
                return serverData.hasOwnProperty(key);
            },

            refresh: function () {
                return this;
            }
        };

        return {
            $get: function () {
                return serverDataService;
            }
        };
    });
})(jQuery);