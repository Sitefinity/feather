(function () {
    var controlPropertyServices = angular.module('controlPropertyServices', []);

    if (typeof ($telerik) != "undefined") {
        $telerik.$(document).on("needsModalDialog", function (event, args) {

            var wrapperLoadingDivId = "modalDialog" + new Date().getTime();
            var wrapperLoadingDiv = jQuery("<div id='" + wrapperLoadingDivId + "' />");
            wrapperLoadingDiv.css("opacity", "0.5");
            wrapperLoadingDiv.css("width", "100%");
            wrapperLoadingDiv.css("z-index", "3100");
            wrapperLoadingDiv.css("position", "absolute");
            wrapperLoadingDiv.css("top", "0");
            wrapperLoadingDiv.css("height", "100%");
            var loadingImageSource = args.AppPath + "Frontend-Assembly/Telerik.Sitefinity.Frontend/Mvc/Styles/Images/loading.gif";
            wrapperLoadingDiv.css("background", "black center no-repeat url(" + loadingImageSource + ")");
            jQuery("body").append(wrapperLoadingDiv);

            var onsuccess = function (data, textStatus, jqXHR) {
                jQuery(wrapperLoadingDiv).remove();
                var wrapperDivId = "modalDialog" + new Date().getTime();
                var wrapperDiv = jQuery("<div id='" + wrapperDivId + "' />");
                jQuery("body").append(wrapperDiv);
                wrapperDiv.append(data);
                $telerik.$(document).one("modalDialogClosed", function () {
                    wrapperDiv.remove();
                });
            };

            if (args && args.url) {

                controlPropertyServices.factory('PageControlDataService', function () {
                    return {
                        data: args
                    };
                });

                jQuery.get(args.url, args, onsuccess);
            }
        });
    }
})();