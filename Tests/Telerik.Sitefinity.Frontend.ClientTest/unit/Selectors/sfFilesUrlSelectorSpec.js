describe('sfFileUrlSelector', function () {
    var rootScope;
    var $q;
    var provide;
    var directiveMarkup = '<sf-file-url-selector sf-model="selectedUrl" sf-extension="css"></sf-file-url-selector>';

    beforeEach(module('templates'));
    beforeEach(module('sfFileUrlSelector'));

    beforeEach(module(function ($provide) {
        $provide.value('sfFileUrlService', fakeFilesUrlService);
        provide = $provide;
    }));

    beforeEach(inject(function (_$rootScope_, _$q_) {
        rootScope = _$rootScope_;
        $q = _$q_;
    }));

    var scope;
    beforeEach(function () {
        scope = rootScope.$new();
        scope.selectedUrl = null;
    });

    beforeEach(function () {
        commonMethods.mockServerContextToEnableTemplateCache();
    });

    afterEach(function () {
        //Tear down.
        var leftOver = $('.testDiv');
        leftOver.empty();
        leftOver.remove();
    });

    var isFolderCssClass = 'is-folder';
    var isFileCssClass = 'is-file';

    var sampleServiceItems = {
        root: [
            { label: 'Folder1', path: '', url: '~/Folder1', isFolder: true, extension: null, hasChildren:true },
            { label: 'Folder2', path: '', url: '~/Folder2', isFolder: true, extension: null, hasChildren: true },
            { label: 'Folder3', path: '', url: '~/Folder3', isFolder: true, extension: null, hasChildren: true },
            { label: 'Item1.css', path: '', url: '~/Item1.css', isFolder: false, extension: 'css' },
            { label: 'Item2.css', path: '', url: '~/Item2.css', isFolder: false, extension: 'css' }
        ],
        Folder1: [
            { label: 'ChildItem11.css', path: 'Folder1/', url: '~/Folder1/ChildItem11.css', isFolder: false, extension: 'css' },
            { label: 'ChildItem12.css', path: 'Folder1/', url: '~/Folder1/ChildItem12.css', isFolder: false, extension: 'css' }
        ],
        Folder2: [
            { label: 'ChildItem21.css', path: 'Folder2/', url: '~/Folder2/ChildItem21.css', isFolder: false, extension: 'css' },
            { label: 'ChildItem22.css', path: 'Folder2/', url: '~/Folder2/ChildItem22.css', isFolder: false, extension: 'css' }
        ],
        Folder3: [
            { label: 'ChildItem31.css', path: 'Folder3/', url: '~/Folder3/ChildItem31.css', isFolder: false, extension: 'css' },
            { label: 'ChildItem32.css', path: 'Folder3/', url: '~/Folder3/ChildItem32.css', isFolder: false, extension: 'css' }
        ]
    };

    var fakeFilesUrlService = {
        get: function (extension, path, skip, take) {
            path = path || 'root';
            var defer = $q.defer();
            defer.resolve(sampleServiceItems[path]);
            return defer.promise;
        },
    };

    describe('rendering', function () {
        it('[dzhenko] / initially all root items should be rendered', function () {
            var el = commonMethods.compileDirective(directiveMarkup, scope);
            expect(el.find('span label:contains("Folder1")').length).toEqual(1);
            expect(el.find('span label:contains("Folder2")').length).toEqual(1);
            expect(el.find('span label:contains("Folder3")').length).toEqual(1);
            expect(el.find('span label:contains("Item1.css")').length).toEqual(1);
            expect(el.find('span label:contains("Item2.css")').length).toEqual(1);
        });

        it('[dzhenko] / initially all non-root items should not be rendered', function () {
            var el = commonMethods.compileDirective(directiveMarkup, scope);
            expect(el.find('span label:contains("ChildItem11.css")').length).toEqual(0);
            expect(el.find('span label:contains("ChildItem12.css")').length).toEqual(0);
            expect(el.find('span label:contains("ChildItem21.css")').length).toEqual(0);
            expect(el.find('span label:contains("ChildItem22.css")').length).toEqual(0);
            expect(el.find('span label:contains("ChildItem31.css")').length).toEqual(0);
            expect(el.find('span label:contains("ChildItem32.css")').length).toEqual(0);
        });

        it('[dzhenko] / should properly set css classes', function () {
            var el = commonMethods.compileDirective(directiveMarkup, scope);
            expect(el.find('span label:contains("Folder1")').parent().hasClass(isFolderCssClass)).toBe(true);
            expect(el.find('span label:contains("Folder2")').parent().hasClass(isFolderCssClass)).toBe(true);
            expect(el.find('span label:contains("Folder3")').parent().hasClass(isFolderCssClass)).toBe(true);
            expect(el.find('span label:contains("Item1.css")').parent().hasClass(isFileCssClass)).toBe(true);
            expect(el.find('span label:contains("Item2.css")').parent().hasClass(isFileCssClass)).toBe(true);
        });
    });
         
    describe('selecting items', function () {
        it('[dzhenko] / clicking on an item should set its url to the passed model', function () {
            var el = commonMethods.compileDirective(directiveMarkup, scope);
            var s = scope.$$childHead;

            expect(s.selectedFile).toEqual([]);

            el.find('span label:contains("Item1.css")').click();
            scope.$digest();

            expect(s.selectedFile).toEqual(['~/Item1.css']);
        });

        it('[dzhenko] / clicking on an folder should set empty string to the passed model', function () {
            var el = commonMethods.compileDirective(directiveMarkup, scope);
            var s = scope.$$childHead;

            expect(s.selectedFile).toEqual([]);

            el.find('span label:contains("Folder1")').click();
            scope.$digest();

            expect(s.selectedFile).toEqual(['~/Folder1']);
        });
    });

    describe('expanding folders', function () {
        it('[dzhenko] / clicking on a folder should expand only its content properly', function () {
            var el = commonMethods.compileDirective(directiveMarkup, scope);
            var s = scope.$$childHead;

            expect(el.find('span label:contains("ChildItem11.css")').length).toEqual(0);
            expect(el.find('span label:contains("ChildItem12.css")').length).toEqual(0);

            el.find('span label:contains("Folder1")').parent().prev().click();
            scope.$digest();

            // In jasmine only last item is rendered. Tested in karma and manually - ok
            expect(el.find('span label:contains("ChildItem11.css")').length).toEqual(1);
            expect(el.find('span label:contains("ChildItem12.css")').length).toEqual(1);

            expect(el.find('span label:contains("ChildItem21.css")').length).toEqual(0);
            expect(el.find('span label:contains("ChildItem22.css")').length).toEqual(0);
            expect(el.find('span label:contains("ChildItem31.css")').length).toEqual(0);
            expect(el.find('span label:contains("ChildItem32.css")').length).toEqual(0);
        });
    });
});