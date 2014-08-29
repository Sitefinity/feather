angular.module('modalDialog').factory('modalInstanceMock', function () {
    return {
        isClosed: false,
        close: function () {
            this.isClosed = true;
        }
    };
});