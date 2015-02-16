/* Tests for image thumbnail size selector */
describe("image thumbnail size selector", function () {
    var THUMBNAILS_ITEMS_COUNT = 2;

    //Will be returned from the service mock.
    var thumbnailDataItems = [{
        "Id": "thumbnail",
        "IsDefault": false,
        "LibrariesCount": 0,
        "Size": "Crop to 120x120",
        "Title": "Thumbnail: 120x120 px cropped"
    }, {
        "Id": "thumb160",
        "IsDefault": false,
        "LibrariesCount": 0,
        "Size": "Fit to area 160x160",
        "Title": "Thumbnail: 160x160 px"
    }];

    var dataItem = {
        "Height": 550,
        "Width": 400
    };

    var customSize = {
        "MaxWidth": "200",
        "MaxHeight": "300",
        "ScaleUp": false,
        "Quality": "Medium",
        "Method": "ResizeFitToAreaArguments"
    };

    var serviceResult;
    var $q;
    var provide;

    var imagesObj = {
        thumbnailProfiles: jasmine.createSpy('sfMediaService.images.thumbnailProfiles').andCallFake(function () {
            if ($q) {
                serviceResult = $q.defer();
            }

            serviceResult.resolve({
                Items: thumbnailDataItems,
                TotalCount: THUMBNAILS_ITEMS_COUNT
            });

            return serviceResult.promise;
        })
    };

    //Mock image item service. It returns promises.
    var mediaService = {
        images: imagesObj
    };

    var serverContext = {
        getEmbeddedResourceUrl: function (assembly, url) {
            return url;
        }
    };

    //Load the module responsible for the modal dialog
    beforeEach(module('modalDialog'));

    //Load the module under test.
    beforeEach(module('sfThumbnailSizeSelection'));

    //beforeEach(inject(function (serverContext) {
    //    serverContext.getEmbeddedResourceUrl = function (assemblyName, url) { {
    //        return 
    //    };
    //}));

    describe('sfThumbnailSizeSelectionCtrl tests', function () {
        var $rootScope, $scope, createController;

        beforeEach(inject(function ($injector) {
            //// Set up the mock http service responses
            //$httpBackend = $injector.get('$httpBackend');

            // Get hold of a scope (i.e. the root scope)
            $rootScope = $injector.get('$rootScope');
            $scope = $rootScope.$new();

            $q = $injector.get('$q');

            // The $controller service is used to create instances of controllers
            var $controller = $injector.get('$controller');

            createController = function () {
                return $controller('sfThumbnailSizeSelectionCtrl', {
                    '$scope': $rootScope, 'sfMediaService': mediaService, 'serverContext': serverContext
                });
            };
        }));

        var vefiryCorrectThumbnailOptionsArePopulated = function (options, customSize) {
            expect(options[0].type).toBe('Original');
            expect(options[0].title).toBe('Original size: 400x550 px');
            expect(options[0].thumbnail).not.toBe();
            expect(options[0].customSize).not.toBe();
            expect(options[0].openDialog).toBeFalsy();

            expect(options[1].type).toBe('Thumbnail');
            expect(options[1].title).toBe(thumbnailDataItems[0].Title);
            expect(options[1].thumbnail.name).toBe(thumbnailDataItems[0].Id);
            expect(options[1].customSize).not.toBe();
            expect(options[1].openDialog).toBeFalsy();

            expect(options[2].type).toBe('Thumbnail');
            expect(options[2].title).toBe(thumbnailDataItems[1].Title);
            expect(options[2].thumbnail.name).toBe(thumbnailDataItems[1].Id);
            expect(options[2].customSize).not.toBe();
            expect(options[2].openDialog).toBeFalsy();

            if (customSize) {
                verifyCorrectCustomSizeOptionsArePopulated(options, customSize);
            } else {
                verifyCorrectDefaultCustomSizeOptionIsPopulated(options);
            }
        };

        var verifyCorrectCustomSizeOptionsArePopulated = function (options, customSize) {
            var index = options.length - 2;
            expect(options[index].type).toBe('Custom');
            expect(options[index].title).toBe('Custom size: 200x300 px');
            expect(options[index].thumbnail).not.toBe();
            expect(options[index].customSize).not.toBe();
            expect(options[index].openDialog).toBeFalsy();

            index = options.length - 1;
            expect(options[index].type).toBe('Custom');
            expect(options[index].title).toBe('Edit custom size...');
            expect(options[index].thumbnail).not.toBe();
            expect(options[index].customSize.MaxWidth).toBe(customSize.MaxWidth);
            expect(options[index].customSize.MaxHeight).toBe(customSize.MaxHeight);
            expect(options[index].customSize.ScaleUp).toBe(customSize.ScaleUp);
            expect(options[index].customSize.Quality).toBe(customSize.Quality);
            expect(options[index].customSize.Method).toBe(customSize.Method);
            expect(options[index].openDialog).toBe(true);
        };

        var verifyCorrectDefaultCustomSizeOptionIsPopulated = function (options) {
            var index = options.length - 1;
            expect(options[index].type).toBe('Custom');
            expect(options[index].title).toBe('Custom size...');
            expect(options[index].thumbnail).not.toBe();
            expect(options[index].customSize).not.toBe();
            expect(options[index].openDialog).toBe(true);
        };

        it('[NPetrova] / Should populate correct size options and should set original size option as default.', function () {
            createController();
            $rootScope.model = {
                item: dataItem
            };

            $rootScope.$digest();

            // the correct size options are populated
            expect($rootScope.sizeOptions).toBeDefined();
            expect($rootScope.sizeOptions.length).toBe(THUMBNAILS_ITEMS_COUNT + 2);
            vefiryCorrectThumbnailOptionsArePopulated($rootScope.sizeOptions);

            // the original size option is set as default
            expect($rootScope.model.displayMode).toBe('Original');
            expect($rootScope.model.thumbnail).not.toBe();
            expect($rootScope.model.customSize).not.toBe();
        });

        it('[NPetrova] / Should initialize correct option when custom size is set', function () {
            createController();
            $rootScope.model = {
                item: dataItem,
                customSize: customSize
            };

            $rootScope.$digest();

            expect($rootScope.sizeOptions).toBeDefined();
            expect($rootScope.sizeOptions.length).toBe(THUMBNAILS_ITEMS_COUNT + 3);
            vefiryCorrectThumbnailOptionsArePopulated($rootScope.sizeOptions, customSize);
        });
    });
});
