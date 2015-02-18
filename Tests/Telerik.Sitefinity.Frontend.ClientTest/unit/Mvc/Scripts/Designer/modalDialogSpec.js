/* Tests for modal-dialog.js */
describe('modal directive', function () {
    var scope;
    var dialogResult = { result: null };
    var dialogResultDefer;
    var $modal = {
        open: jasmine.createSpy('$modal.open').andReturn(dialogResult)
    };
    var dialogsService;

    beforeEach(module('modalDialog'));

    beforeEach(module(function ($provide) {
        $provide.value('$modal', $modal);
    }));

    beforeEach(inject(function ($rootScope, $q, _dialogsService_) {
        scope = $rootScope.$new();
        scope.test = 'test prop';
        scope.open = function () {
            this.$openModalDialog();
        };

        dialogsService = _dialogsService_;

        dialogResultDefer = $q.defer();
        dialogResult.result = dialogResultDefer.promise;
    }));

    afterEach(function () {
        $('.testDiv').empty();
        $('.testDiv').remove();
    });

    var assertOpenWithController = function (templateId, skipClean) {
        var tmplId = templateId || 'dialog-template';

        var mostRecent = $modal.open.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        var args = mostRecent.args[0];

        expect(args.backdrop).toBe('static');
        expect(args.scope).toBeUndefined();
        expect(args.templateUrl).toBe(tmplId);
        expect(args.controller).toBe('TestCtrl');

        if (!skipClean) {
            $modal.open.reset();
        }
    };

    var assertOpenWithScope = function () {
        var mostRecent = $modal.open.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        var args = mostRecent.args[0];

        expect(args.backdrop).toBe('static');
        expect(args.scope).toBeDefined();
        expect(args.scope.test).toBe('test prop');
        expect(args.templateUrl).toBe('dialog-template');
        expect(args.controller).toBeUndefined();

        $modal.open.reset();
    };

    it('[GMateev] / should open dialog on page load with given controller.]', function () {
        var template =
        '<div class="designer" modal dialog-controller="TestCtrl" template-url="dialog-template" auto-open="true">' +
            '<script type="text/ng-template" id="dialog-template">' +
                '<div class="modal-body">' +
                    '<h2>Body</h2>' +
                '</div>'+
            '</script>'+
        '</div>';

        commonMethods.compileDirective(template, scope);

        assertOpenWithController();
    });

    it('[GMateev] / should open dialog on page load with already existing scope.]', function () {
        var template =
        '<div class="designer" modal existing-scope="true" template-url="dialog-template" auto-open="true">' +
            '<script type="text/ng-template" id="dialog-template">' +
                '<div class="modal-body">' +
                    '<h2>Body</h2>' +
                '</div>' +
            '</script>' +
        '</div>';

        commonMethods.compileDirective(template, scope);

        assertOpenWithScope();
    });

    it('[GMateev] / should open dialog when button is clicked.]', function () {
        var template =
        '<button id="openSelectorBtn" ng-click="open()">Open</button>' +
        '<div class="designer" modal existing-scope="true" template-url="dialog-template">' +
            '<script type="text/ng-template" id="dialog-template">' +
                '<div class="modal-body">' +
                    '<h2>Body</h2>' +
                '</div>' +
            '</script>' +
        '</div>';

        commonMethods.compileDirective(template, scope);

        expect($modal.open).not.toHaveBeenCalled();

        $('#openSelectorBtn').click();

        assertOpenWithScope();
    });

    it('[GMateev] / should open two nested dialogs.]', function () {
        var template =
        '<div class="designer" modal dialog-controller="TestCtrl" template-url="outside-dialog-template" auto-open="true">' +
            '<script type="text/ng-template" id="outside-dialog-template">' +
               '<div id="dialogWrapper"></div>' +
            '</script>' +
        '</div>';

        var template2 = '<button id="openSelectorBtn" ng-click="open()">Open</button>' +
                '<div class="designer" modal existing-scope="true" template-url="dialog-template">' +
                    '<script type="text/ng-template" id="dialog-template">' +
                        '<div class="modal-body">' +
                            '<h2>Body</h2>' +
                        '</div>' +
                    '</script>' +
                '</div>';

        commonMethods.compileDirective(template, scope);        

        expect($modal.open.callCount).toBe(1);
        expect(dialogsService.count()).toBe(1);

        assertOpenWithController("outside-dialog-template", true);

        commonMethods.compileDirective(template2, scope);

        $('#openSelectorBtn').click();

        expect($modal.open.callCount).toBe(2);
        expect(dialogsService.count()).toBe(2);

        assertOpenWithScope();
    });
});