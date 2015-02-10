/* link selector modal */
describe("link selector modal", function () {
    var scope;
    var serviceResult;
    var $q;
    var provide;
    var $rootScope;
    
    var appPath = 'http://mysite.com:9999/myapp';
    var sfLinkMode;

    var testLink = '<a href="url">link</a>';

    var linkServiceMock = {
        getHtmlLink: jasmine.createSpy('getHtmlLink')
            .andCallFake(function (selectedItem) {
                return [testLink];
            })
    };

    var linkSelectorScopeMock;
    beforeEach(function () {
        linkSelectorScopeMock = jasmine.createSpy('scope')
            .andCallFake(function () {
                return {

                };
            });
    });

    var angularElementSpy;
    beforeEach(function () {
        var angularElementOriginal = angular.element;
        angularElementSpy = spyOn(angular, 'element').andCallFake(function (selector) {
            if (selector === '#linkSelector') {
                return {
                    scope: linkSelectorScopeMock
                };
            }
            else {
                return angularElementOriginal(selector);
            }
        });
    });

    //Load the module responsible for the modal dialog
    beforeEach(module('modalDialog'));

    //Load the module under test.
    beforeEach(module('sfSelectors'));

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));

    beforeEach(module(function ($provide) {
        //Force angular to use the mock.
        $provide.value('sfLinkService', linkServiceMock);
        provide = $provide;
    }));

    beforeEach(inject(function (_$rootScope_, $httpBackend, _$q_, $templateCache, _$timeout_, $injector) {
        //Build the scope with whom the directive will be compiled.
        $rootScope = _$rootScope_;
        $q = _$q_;
        $timeout = _$timeout_;
        serviceResult = _$q_.defer();
        sfLinkMode = $injector.get('sfLinkMode');
    }));

    beforeEach(function () {
        commonMethods.mockServerContextToEnableTemplateCache();
    });

    afterEach(function () {
        //Tear down.
        var leftOver = $('.testDiv, .modal, .modal-backdrop');
        leftOver.empty();
        leftOver.remove();
    });

    describe('link selector modal logic', function () {
        it('[GeorgiMateev] / should create link and close the dialog with it when insertLink method is called.', function () {
            var template = '<sf-link-selector-modal></sf-link-selector-modal>';
            var scope = $rootScope.$new();

            commonMethods.compileDirective(template, scope);

            // Since the compileDirective method is having trouble compiling the inner modal dialog,
            // we mock the $modalInstance here.
            scope.$$childHead.$modalInstance = {
                close: jasmine.createSpy('close')
            };

            scope.$$childHead.insertLink();

            expect(linkSelectorScopeMock).toHaveBeenCalled();
            expect(scope.$$childHead.$modalInstance.close).toHaveBeenCalledWith(testLink);
        });

        describe('when isDisabled is called', function () {
            var scope;
            beforeEach(function () {
                var template = '<sf-link-selector-modal></sf-link-selector-modal>';
                scope = $rootScope.$new();

                commonMethods.compileDirective(template, scope);
            });

            it('[GeorgiMateev] / should determine state based on webAddress and displayText.', function () {
                // True when both are empty.
                expect(scope.$$childHead.isDisabled({
                    mode: sfLinkMode.WebAddress,
                    webAddress: 'http://',
                    displayText: ''
                })).toBe(true);

                // False when both are filled.
                expect(scope.$$childHead.isDisabled({
                    mode: sfLinkMode.WebAddress,
                    webAddress: 'url',
                    displayText: 'text'
                })).toBe(false);
            });

            it('[GeorgiMateev] / should determine state based on selectedPage and displayText are empty.', function () {
                expect(scope.$$childHead.isDisabled({
                    mode: sfLinkMode.InternalPage,
                    displayText: ''
                })).toBe(true);

                expect(scope.$$childHead.isDisabled({
                    mode: sfLinkMode.InternalPage,
                    selectedPage: 'page',
                    displayText: 'text'
                })).toBe(false);
            });

            it('[GeorgiMateev] / should determine state based on selectedAnchor and displayText.', function () {
                expect(scope.$$childHead.isDisabled({
                    mode: sfLinkMode.Anchor,
                    displayText: ''
                })).toBe(true);

                expect(scope.$$childHead.isDisabled({
                    mode: sfLinkMode.Anchor,
                    selectedAnchor: 'anchor',
                    displayText: 'text'
                })).toBe(false);
            });

            it('[GeorgiMateev] / should determine state based on emailAddress and displayText.', function () {
                expect(scope.$$childHead.isDisabled({
                    displayText: ''
                })).toBe(true);

                expect(scope.$$childHead.isDisabled({
                    displayText: 'text',
                    emailAddress: 'email'
                })).toBe(false);
            });
        });

        it('[GeorgiMateev] / it should inherit parent scope properties (selectedHtml/selectedItem/ngModel).', function () {
            var template = '<sf-link-selector-modal id="linkModal"></sf-link-selector-modal>';
                scope = $rootScope.$new();
                scope.ngModel = 'ngModel';
                scope.selectedItem = {
                    selectedPage: 'page'
                };
                scope.selectedHtml = '<a>link</a>';

                commonMethods.compileDirective(template, scope);

                var childScope = angular.element('#linkModal').scope();

                expect(childScope.ngModel).toBe(scope.ngModel);
                expect(childScope.selectedItem).toBe(scope.selectedItem);
                expect(childScope.selectedHtml).toBe(scope.selectedHtml);
        });
    });
});