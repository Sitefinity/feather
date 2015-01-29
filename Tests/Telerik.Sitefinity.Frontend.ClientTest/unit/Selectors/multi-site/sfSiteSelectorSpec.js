describe('site selecor directive', function () {
    beforeEach(module('sfSelectors'));

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));

    var $q;
    var $rootScope;
    var provide;

    var sites = [{
        Id: '8d8b587d-5ac3-457f-8bb6-cafd0e79ea75',
        Name: 'site1',
        SiteMapRootNodeId: "f669d9a7-009d-4d83-ddaa-000000000002"
    },
    {
        Id: '487b05be-a32a-4476-bb2d-36c499eb770e',
        Name: 'site2',
        SiteMapRootNodeId: "f669d9a7-009d-4d83-ddaa-000000000003"
    }];

    beforeEach(module(function ($provide) {
        provide = $provide;
        $provide.value('sfMultiSiteService', sfMultisiteServiceMock);
    }));

    beforeEach(module(function ($provide) {
        var serverContext = {
            getRootedUrl: function (path) {
                return appPath + '/' + path;
            },
            getUICulture: function () {
                return null;
            },
            getCurrentUserId: function () {
                return 'f669d9a7-009d-4d83-ddaa-000000000002';
            },
            getCurrentFrontendRootNodeId: function () {
                return 'f669d9a7-009d-4d83-ddaa-000000000002';
            },
            isMultisiteEnabled: function () {
                return true;
            }
        };
        $provide.value('serverContext', serverContext);
    }));

    beforeEach(inject(function(_$q_, _$rootScope_) {
        $q = _$q_;
        $rootScope = _$rootScope_;
    }));

    var sfMultisiteServiceMock = {
        getSitesForUserPromise: jasmine.createSpy('getSitesForUserPromise')
            .andCallFake(function (defaultParams) {
                var result = $q.defer();

                result.resolve({
                    Items: sites
                });

                return result.promise;
            })
    };

    var sfMultisiteServiceErrorMock = {
        getSitesForUserPromise: jasmine.createSpy('getSitesForUserPromise')
            .andCallFake(function (defaultParams) {
                var result = $q.defer();

                result.reject('Error!');

                return result.promise;
            })
    };

    beforeEach(function () {
        commonMethods.mockServerContextToEnableTemplateCache();
    });

    afterEach(function () {
        //Tear down.
        var leftOver = $('.testDiv, .modal, .modal-backdrop');
        leftOver.empty();
        leftOver.remove();

        //Sets default sfMultiSiteService mock.
        provide.value('sfMultiSiteService', sfMultisiteServiceMock);
    });

    it('[GeorgiMateev] / should call the multisite service on load.',
    function () {
        var scope = $rootScope.$new();
        var template = '<sf-site-selector sf-site="site"><sf-site-selector/>';
        commonMethods.compileDirective(template, scope);

        expect(sfMultisiteServiceMock.getSitesForUserPromise)
            .toHaveBeenCalledWith({
                    sortExpression: 'Name'
            });
    });

    it('[GeorgiMateev] / should set the sites in the scope and select default one.',
    function () {
        var scope = $rootScope.$new();
        var template = '<sf-site-selector sf-site="site"><sf-site-selector/>';
        commonMethods.compileDirective(template, scope);

        //The scope of the selector is isolated, but it's child of the scope used for compilation.
        var s = scope.$$childHead;

        expect(s.sfSites).toEqualArrayOfObjects(sites, ['Id']);
        expect(scope.site).toEqual(sites[0]);
    });

    it('[GeorgiMateev] / should select option in the select element by given scope property.',
    function () {
        var scope = $rootScope.$new();
        scope.site = sites[1];
        var template = '<sf-site-selector id="siteSelector" sf-site="site"><sf-site-selector/>';
        commonMethods.compileDirective(template, scope);

        var siteSelector = $('#siteSelector select');
        expect(siteSelector.val()).toEqual(sites[1].SiteMapRootNodeId);
    });

    it('[GeorgiMateev] / should populate the select element in the html.',
    function () {
        var scope = $rootScope.$new();
        var template = '<sf-site-selector id="siteSelector" sf-site="site"><sf-site-selector/>';
        commonMethods.compileDirective(template, scope);

        var siteSelector = $('#siteSelector select');
        expect(siteSelector.val()).toEqual(sites[0].SiteMapRootNodeId);

        var options = $('#siteSelector select option').map(function (index, option) {
            return {
                value: option.value,
                text: option.text
            };
        });

        var sitesData = sites.map(function (site) {
            return {
                value: site.Id,
                text: site.Name
            };
        });

        expect(options).toEqualArrayOfObjects(sitesData, 'value', 'text');
    });

    it('[GeorgiMateev] / should display error message when the multisite service return error.',
    function () {
        provide.value('sfMultiSiteService', sfMultisiteServiceErrorMock);

        var scope = $rootScope.$new();
        var template = '<sf-site-selector sf-site="site"><sf-site-selector/>';
        commonMethods.compileDirective(template, scope);

        //The scope of the selector is isolated, but it's child of the scope used for compilation.
        var s = scope.$$childHead;
        expect(s.showError).toBe(true);
        expect(s.errorMessage).toBe('Error!');
    });
});