(function ($) {
    var serverDataModule = angular.module('serverDataModule', []);

    serverDataModule.provider('serverData', function () {
        var serverData = {};

        var serverDataService = {
            get: function (key) {
                return serverData[key];
            },

            has: function (key) {
                return serverData.hasOwnProperty(key);
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
                serverDataService.update();
                return serverDataService;
            }
        });
    });
})(jQuery);