describe('Library filter', function () {
    var $rootScope;
    var mediaService;
    var templateCache;
    var $q;

    var getFoldersObj = {
        getFolders: function (parent) {
            var items = [
                { Title: 'Title1', Id: '1' },
                { Title: 'Title2', Id: '2' }
            ];

            var defer = $q.defer();
            defer.resolve({ Items: items });
            return defer.promise;
        }
    };

    beforeEach(module('templates'));
    beforeEach(module('sfLibraryFilter'));

    beforeEach(inject(function ($injector) {
        mediaService = $injector.get('sfMediaService');
    }));

    beforeEach(inject(function (_$rootScope_, _$q_, $templateCache) {
        $rootScope = _$rootScope_;
        $q = _$q_;
        templateCache = $templateCache;
    }));

    beforeEach(function () {
        commonMethods.mockServerContextToEnableTemplateCache();
    });

    it('[dzhenko] / should properly set parent id of filter object', function () {
        var scope = $rootScope.$new();

        scope.filterObject = mediaService.newFilter();

        mediaService.specTest = getFoldersObj;

        var directiveMarkup = '<span sf-library-filter sf-model="filterObject" sf-media-type="specTest"></span>';

        commonMethods.compileDirective(directiveMarkup, scope);

        expect(scope.filterObject.parent).toBe(null);

        $('ul li span:contains("Title1")').click();
        scope.$digest();

        expect(scope.filterObject.parent).toEqual('1');
    });

    it('[dzhenko] / should properly set basic property to null when parent Id is selected', function () {
        var scope = $rootScope.$new();

        scope.filterObject = mediaService.newFilter();

        mediaService.specTest = getFoldersObj;

        var directiveMarkup = '<span sf-library-filter sf-model="filterObject" sf-media-type="specTest"></span>';

        commonMethods.compileDirective(directiveMarkup, scope);

        scope.filterObject.basic = "Initial Value";
        scope.$digest();

        expect(scope.filterObject.parent).toBe(null);
        expect(scope.filterObject.basic).toEqual("Initial Value");

        $('ul li span:contains("Title1")').click();
        scope.$digest();

        expect(scope.filterObject.parent).toEqual('1');
        expect(scope.filterObject.basic).toBe(null);
    });

    it('[dzhenko] / should properly set parent to null when clicked second time (deselected)', function () {
        var scope = $rootScope.$new();

        scope.filterObject = mediaService.newFilter();

        mediaService.specTest = getFoldersObj;

        var directiveMarkup = '<span sf-library-filter sf-model="filterObject" sf-media-type="specTest"></span>';

        commonMethods.compileDirective(directiveMarkup, scope);

        expect(scope.filterObject.parent).toBe(null);

        $('ul li span:contains("Title1")').click();
        scope.$digest();
        expect(scope.filterObject.parent).toEqual('1');

        $('ul li span:contains("Title1")').click();
        scope.$digest();
        expect(scope.filterObject.parent).toBe(null);
    });

    it('[dzhenko] / should properly throw exception if passed filter object is not of a kind sfMediaService.newFilter()', function () {
        var scope = $rootScope.$new();

        scope.filterObject = { };

        mediaService.specTest = getFoldersObj;

        var directiveMarkup = '<span sf-library-filter sf-model="filterObject" sf-media-type="specTest"></span>';
        
        expect(function () {
            commonMethods.compileDirective(directiveMarkup, scope);
        }).toThrow();
    });

    it('[dzhenko] / should properly throw exception if passed sf-media-type is not present in sfMediaSerice', function () {
        var scope = $rootScope.$new();

        scope.filterObject = mediaService.newFilter();

        mediaService.specTest = getFoldersObj;

        var directiveMarkup = '<span sf-library-filter sf-model="filterObject" sf-media-type="unexistingType"></span>';

        expect(function () {
            commonMethods.compileDirective(directiveMarkup, scope);
        }).toThrow();
    });

    it('[dzhenko] / should properly throw exception if passed sf-media-type object does not contain getFolders method', function () {
        var scope = $rootScope.$new();

        scope.filterObject = mediaService.newFilter();

        mediaService.specTest = {
            notGetFoldersFunc: function () {
                return [];
            }
        };

        var directiveMarkup = '<span sf-library-filter sf-model="filterObject" sf-media-type="specTest"></span>';

        expect(function () {
            commonMethods.compileDirective(directiveMarkup, scope);
        }).toThrow();
    });

    it('[dzhenko] / should not throw exception when passed filterObject and media type are correct', function () {
        var scope = $rootScope.$new();

        scope.filterObject = mediaService.newFilter();

        mediaService.specTest = getFoldersObj;

        var directiveMarkup = '<span sf-library-filter sf-model="filterObject" sf-media-type="specTest"></span>';

        commonMethods.compileDirective(directiveMarkup, scope);

        // will fail if above method throws exception
        expect(true).toBe(true);
    });
});