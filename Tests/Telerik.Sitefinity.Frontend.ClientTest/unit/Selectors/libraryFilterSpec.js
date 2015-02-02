describe('library filter', function () {
    beforeEach(module('sfLibraryFilter'));

    var $rootScope;
    var sfMediaService;
    var templateCache;

    var getFoldersObj = {
        getFolders: function (parent) {
            return [
                { Title: 'Title', Id: '1' }
            ];
        }
    };

    beforeEach(inject(function (_$rootScope_, $templateCache) {
        $rootScope = _$rootScope_;
        templateCache = $templateCache;
    }));

    beforeEach(inject(function ($injector) {
        sfMediaService = $injector.get('sfMediaService');
    }));

    it('[dzhenko] / should properly throw exception if passed filter object is not of a kind sfMediaService.newFilter()', function () {
        var scope = $rootScope.$new();

        scope.filterObject = { };

        sfMediaService.specTest = getFoldersObj;

        templateCache.put('/Frontend-Assembly/Telerik.Sitefinity.Frontend/client-components/selectors/media/sf-library-filter.html', '');

        var directiveMarkup = '<span sf-library-filter ng-model="filterObject" sf-media-type="specTest"></span>';
        
        expect(function () {
            commonMethods.compileDirective(directiveMarkup, scope);
        }).toThrow();
    });

    it('[dzhenko] / should properly throw exception if passed sf-media-type is not present in sfMediaSerice', function () {
        var scope = $rootScope.$new();

        scope.filterObject = sfMediaService.newFilter();

        sfMediaService.specTest = getFoldersObj;

        templateCache.put('/Frontend-Assembly/Telerik.Sitefinity.Frontend/client-components/selectors/media/sf-library-filter.html', '');

        var directiveMarkup = '<span sf-library-filter ng-model="filterObject" sf-media-type="unexistingType"></span>';

        expect(function () {
            commonMethods.compileDirective(directiveMarkup, scope);
        }).toThrow();
    });

    it('[dzhenko] / should properly throw exception if passed sf-media-type object does not contain getFolders method', function () {
        var scope = $rootScope.$new();

        scope.filterObject = sfMediaService.newFilter();

        sfMediaService.specTest = {
            notGetFoldersFunc: function () {
                return [];
            }
        };

        templateCache.put('/Frontend-Assembly/Telerik.Sitefinity.Frontend/client-components/selectors/media/sf-library-filter.html', '');

        var directiveMarkup = '<span sf-library-filter ng-model="filterObject" sf-media-type="specTest"></span>';

        expect(function () {
            commonMethods.compileDirective(directiveMarkup, scope);
        }).toThrow();
    });

    it('[dzhenko] / should not throw exception when passed filterObject and media type are correct', function () {
        var scope = $rootScope.$new();

        scope.filterObject = sfMediaService.newFilter();

        sfMediaService.specTest = getFoldersObj;

        templateCache.put('/Frontend-Assembly/Telerik.Sitefinity.Frontend/client-components/selectors/media/sf-library-filter.html', '');

        var directiveMarkup = '<span sf-library-filter ng-model="filterObject" sf-media-type="specTest"></span>';

        commonMethods.compileDirective(directiveMarkup, scope);

        // will fail if above method throws exception
        expect(true).toBe(true);
    });
});