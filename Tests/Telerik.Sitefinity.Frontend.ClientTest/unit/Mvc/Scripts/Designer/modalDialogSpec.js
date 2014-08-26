/* Tests for modal-dialog.js */
describe('modal directive', function () {
    var scope;
    var dialogResult = { result: null };
    var dialogResultDefer;
    var $modal = {
        open: jasmine.createSpy('$modal.open').andReturn(dialogResult)
    };

    beforeEach(module('modalDialog'));

    beforeEach(module(function ($provide) {
        $provide.value('$modal', $modal);
    }));

    beforeEach(inject(function ($rootScope, $q) {
        scope = $rootScope.$new();
        scope.test = 'test prop';

        dialogResultDefer = $q.defer();
        dialogResult.result = dialogResultDefer.promise;
    }));

    afterEach(function () {
        $('#testDiv').empty();
        $('#testDiv').remove();
    });

    /* Helper methods */
    var compileDirective = function (template) {
        inject(function ($compile) {
            directiveElement = $compile(template)(scope);
            $('body').append($('<div/>').attr('id', 'testDiv')
                .append(directiveElement));
        });
        
        // $digest is necessary to finalize the directive generation
        scope.$digest();
    }

    var assertOpenWithController = function () {
        var mostRecent = $modal.open.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        var args = mostRecent.args[0];

        expect(args.backdrop).toBe('static');
        expect(args.scope).toBeUndefined();
        expect(args.templateUrl).toBe('dialog-template');
        expect(args.controller).toBe('TestCtrl');

        $modal.open.reset();
    }

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
    }

    it('[GMateev / should open dialog on page load with given controller.]', function () {
        var template =
        '<div class="designer" modal dialog-controller="TestCtrl" template-url="dialog-template" auto-open="true">' +
            '<script type="text/ng-template" id="dialog-template">' +
                '<div class="modal-body">' +
                    '<h2>Body</h2>' +
                '</div>'+
            '</script>'
        '</div>';

        compileDirective(template);

        assertOpenWithController();
    });

    it('[GMateev / should open dialog on page load with already existing scope.]', function () {
        var template =
        '<div class="designer" modal existing-scope="true" template-url="dialog-template" auto-open="true">' +
            '<script type="text/ng-template" id="dialog-template">' +
                '<div class="modal-body">' +
                    '<h2>Body</h2>' +
                '</div>' +
            '</script>'
        '</div>';

        compileDirective(template);

        assertOpenWithScope();
    });

    it('[GMateev / should open dialog when button is clicked.]', function () {
        var template =
        '<button id="openSelectorBtn">Open</button>' +
        '<div class="designer" modal existing-scope="true" template-url="dialog-template" open-button="#openSelectorBtn">' +
            '<script type="text/ng-template" id="dialog-template">' +
                '<div class="modal-body">' +
                    '<h2>Body</h2>' +
                '</div>' +
            '</script>'
        '</div>';

        compileDirective(template);

        expect($modal.open).not.toHaveBeenCalled()

        $('#openSelectorBtn').click();

        assertOpenWithScope();
    });
});