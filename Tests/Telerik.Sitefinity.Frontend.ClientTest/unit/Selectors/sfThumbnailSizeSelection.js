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

    var resizeToAreaCustomSize = {
        "MaxWidth": "200",
        "MaxHeight": "300",
        "Width": null,
        "Height": null,
        "ScaleUp": false,
        "Quality": "Medium",
        "Method": "ResizeFitToAreaArguments"
    };

    var cropToAreaCustomSize = {
        "MaxWidth": null,
        "MaxHeight": null,
        "Width": "200",
        "Height": "300",
        "ScaleUp": false,
        "Quality": "Medium",
        "Method": "CropCropArguments"
    };

    var thumbnail = {
        name: 'thumb160'
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

    describe('sfThumbnailSizeSelectionCtrl tests', function () {
        var $rootScope, createController;

        beforeEach(inject(function ($injector) {
            // Get hold of a scope (i.e. the root scope)
            $rootScope = $injector.get('$rootScope');

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
            expect(options[0].thumbnail).toBe(null);
            expect(options[0].customSize).toBe(null);
            expect(options[0].openDialog).toBeFalsy();

            expect(options[1].type).toBe('Thumbnail');
            expect(options[1].title).toBe(thumbnailDataItems[0].Title);
            expect(options[1].thumbnail.name).toBe(thumbnailDataItems[0].Id);
            expect(options[1].customSize).toBe(null);
            expect(options[1].openDialog).toBeFalsy();

            expect(options[2].type).toBe('Thumbnail');
            expect(options[2].title).toBe(thumbnailDataItems[1].Title);
            expect(options[2].thumbnail.name).toBe(thumbnailDataItems[1].Id);
            expect(options[2].customSize).toBe(null);
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
            expect(options[index].thumbnail).toBe(null);
            expect(options[index].customSize).toBe(customSize);
            expect(options[index].openDialog).toBeFalsy();

            index = options.length - 1;
            expect(options[index].type).toBe('Custom');
            expect(options[index].title).toBe('Edit custom size...');
            expect(options[index].thumbnail).toBe(null);
            expect(options[index].customSize).toBe(customSize);
            expect(options[index].openDialog).toBe(true);
        };

        var verifyCorrectDefaultCustomSizeOptionIsPopulated = function (options) {
            var index = options.length - 1;
            expect(options[index].type).toBe('Custom');
            expect(options[index].title).toBe('Custom size...');
            expect(options[index].thumbnail).toBe(null);
            expect(options[index].customSize).not.toBeDefined();
            expect(options[index].openDialog).toBe(true);
        };

        var testWithCustomSizeOption = function (customSize) {
            createController();
            $rootScope.model = {
                item: dataItem,
                customSize: customSize,
                thumbnail: null,
                displayMode: 'Custom'
            };
            $rootScope.$digest();

            expect($rootScope.sizeOptions).toBeDefined();
            expect($rootScope.sizeOptions.length).toBe(THUMBNAILS_ITEMS_COUNT + 3);
            vefiryCorrectThumbnailOptionsArePopulated($rootScope.sizeOptions, customSize);

            expect($rootScope.sizeSelection).toBe($rootScope.sizeOptions[$rootScope.sizeOptions.length - 2]);
            expect($rootScope.model.displayMode).toBe('Custom');
            expect($rootScope.model.thumbnail).toBe(null);
            expect($rootScope.model.customSize).toBe(customSize);
        };

        it('[NPetrova] / Should populate correct size options and should set original size option as default.', function () {
            createController();
            $rootScope.$digest();

            $rootScope.model = { item: dataItem };
            $rootScope.$digest();

            // the correct size options are populated
            expect($rootScope.sizeOptions).toBeDefined();
            expect($rootScope.sizeOptions.length).toBe(THUMBNAILS_ITEMS_COUNT + 2);
            vefiryCorrectThumbnailOptionsArePopulated($rootScope.sizeOptions);

            // the original size option is set as default
            expect($rootScope.model.displayMode).toBe('Original');
            expect($rootScope.model.thumbnail).toBe(null);
            expect($rootScope.model.customSize).toBe(null);
        });

        it('[NPetrova] / Should initialize correct option when custom resize to area option is set', function () {
            testWithCustomSizeOption(resizeToAreaCustomSize);
        });

        it('[NPetrova] / Should initialize correct option when custom crop to area option is set', function () {
            testWithCustomSizeOption(cropToAreaCustomSize);
        });

        it('[NPetrova] / Should initialize correct option when thumbnail size is set', function () {
            createController();
            $rootScope.$digest();

            $rootScope.model = {
                item: dataItem,
                thumbnail: thumbnail,
                displayMode: 'Thumbnail'
            };
            $rootScope.$digest();

            expect($rootScope.sizeOptions).toBeDefined();
            expect($rootScope.sizeOptions.length).toBe(THUMBNAILS_ITEMS_COUNT + 2);
            vefiryCorrectThumbnailOptionsArePopulated($rootScope.sizeOptions);

            expect($rootScope.sizeSelection).toBe($rootScope.sizeOptions[$rootScope.sizeOptions.length - 2]);
            expect($rootScope.model.displayMode).toBe('Thumbnail');
            expect($rootScope.model.thumbnail.name).toBe(thumbnail.name);
            expect($rootScope.model.customSize).toBe(null);
        });
    });

    describe('sfCustomThumbnailSizeCtrl tests', function () {
        var $rootScope, createController;

        beforeEach(inject(function ($injector) {
            // Get hold of a scope (i.e. the root scope)
            $rootScope = $injector.get('$rootScope');

            // The $controller service is used to create instances of controllers
            var $controller = $injector.get('$controller');

            createController = function (modalInstance, model) {
                return $controller('sfCustomThumbnailSizeCtrl', {
                    '$scope': $rootScope, '$modalInstance': modalInstance, 'model': model
                });
            };
        }));

        it('[NPetrova] / Should initialize default model if no model is passed.', function () {
            //mock the call to the modal service.
            modalInstance = {
                close: function (model) {
                    expect(model.MaxWidth).toBe(200);
                    expect(model.MaxHeight).toBe(300);
                    expect(model.Width).toBe(null);
                    expect(model.Height).toBe(null);
                    expect(model.ScaleUp).toBe(true);
                    expect(model.Quality).toBe('Medium');
                    expect(model.Method).toBe('CropCropArguments');
                }
            };

            createController(modalInstance);
            $rootScope.$digest();

            expect($rootScope.model.MaxWidth).toBe(null);
            expect($rootScope.model.MaxHeight).toBe(null);
            expect($rootScope.model.Width).toBe(null);
            expect($rootScope.model.Height).toBe(null);
            expect($rootScope.model.ScaleUp).toBeFalsy();
            expect($rootScope.model.Quality).toBe('High');
            expect($rootScope.model.Method).toBe('ResizeFitToAreaArguments');

            $rootScope.model.Method = $rootScope.methodOptions[1].value;
            $rootScope.model.MaxWidth = 200;
            $rootScope.model.MaxHeight = 300;
            $rootScope.model.ScaleUp = true;
            $rootScope.model.Quality = $rootScope.quality[1];

            $rootScope.done();
        });

        it('[NPetrova] / Should populate correct model.', function () {
            //mock the call to the modal service.
            modalInstance = {
                close: function (model) {
                    expect(model).toBe(resizeToAreaCustomSize);
                }
            };

            createController(modalInstance, resizeToAreaCustomSize);
            $rootScope.$digest();

            $rootScope.done();
        });

        it('[NPetrova] / Should validate custom size options.', function () {
            createController();
            expect($rootScope.areCustomSizeOptionsValid()).toBeFalsy();

            $rootScope.model.MaxHeight = 'invalid';
            expect($rootScope.areCustomSizeOptionsValid()).toBeFalsy();

            $rootScope.model.MaxHeight = 'invalid';
            expect($rootScope.areCustomSizeOptionsValid()).toBeFalsy();

            $rootScope.model.MaxHeight = '234';
            expect($rootScope.areCustomSizeOptionsValid()).toBeFalsy();

            $rootScope.model.MaxWidth = 'invalid';
            expect($rootScope.areCustomSizeOptionsValid()).toBeFalsy();

            $rootScope.model.MaxWidth = '235';
            expect($rootScope.areCustomSizeOptionsValid()).toBe(true);

            $rootScope.model.Method = $rootScope.methodOptions[1].value;
            expect($rootScope.areCustomSizeOptionsValid()).toBeFalsy();

            $rootScope.model.Width = 9999999999;
            expect($rootScope.areCustomSizeOptionsValid()).toBeFalsy();

            $rootScope.model.Width = 9999;
            $rootScope.model.Height = 'invalid';
            expect($rootScope.areCustomSizeOptionsValid()).toBeFalsy();

            $rootScope.model.Height = 1;
            expect($rootScope.areCustomSizeOptionsValid()).toBe(true);

            $rootScope.model.Method = $rootScope.methodOptions[0].value;
            expect($rootScope.areCustomSizeOptionsValid()).toBe(true);
        });
    });
});