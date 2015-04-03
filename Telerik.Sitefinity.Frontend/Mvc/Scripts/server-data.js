(function ($) {
    var serverDataModule = angular.module('serverDataModule', []);

    serverDataModule.provider('serverData', function () {
        var serverData = {};

        var updateServerData = function () {
            $('server-data')
                .each(function () {
                    $.each(this.attributes, function (i, attribute) {
                        serverData[$.camelCase(attribute.name)] = attribute.value;
                    });
                })
                .remove();
        };

        var serverDataService = {
            getAll: function () {
                return serverData;
            },

            get: function (key) {
                return serverData[key];
            },

            has: function (key) {
                return serverData.hasOwnProperty(key);
            },

            refresh: function () {
                updateServerData();
                return this;
            }
        };

        return {
            $get: function () {
                updateServerData();
                return serverDataService;
            }
        };
    });
})(jQuery);