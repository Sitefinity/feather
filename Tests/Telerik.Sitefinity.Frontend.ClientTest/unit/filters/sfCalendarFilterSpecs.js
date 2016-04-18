describe('Calendar filter', function () {
    var rootScope,
        provide,
        $q,
        mediaService,
        tagsId = 'CB0F3A19-A211-48a7-88EC-77495C0F5374',
        directiveMarkup = '<sf-calendar-filter sf-query-data="sfQueryData" sf-provider="sfProvider" sf-query-field-name="Calendars" sf-group-logical-operator="AND" sf-item-logical-operator="OR"></sf-calendar-filter>';

    var calendarService = {
        getSpecificItems: jasmine.createSpy('sfCalendarService.getSpecificItems').and.callFake(function (ids, provider) {
            if ($q) {
                serviceResult = $q.defer();
            }

            var dataItem = {
                Id: '6266ff24-2be2-611d-b54a-ff0000bc5146',
                Title: { Value: 'Filtered' }
            };

            var dataItem2 = {
                Id: '4c003fb0-2a77-61ec-be54-ff11117864f4',
                Title: { Value: 'Dummy' },
                Filter: true
            };
            var items = [dataItem, dataItem2].filter(function (item) {
                return ids.indexOf(item.Id) >= 0;
            });

            serviceResult.resolve({
                Items: items,
                TotalCount: items.length
            });

            return serviceResult.promise;
        }),
    };

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));
    beforeEach(module('sfSelectors'));

    beforeEach(module(function ($provide) {
        $provide.value('sfCalendarService', calendarService);

        provide = $provide;
    }));

    beforeEach(inject(function (_$rootScope_, _$q_) {
        rootScope = _$rootScope_;
        $q = _$q_;
    }));

    beforeEach(function () {
        commonMethods.mockServerContextToEnableTemplateCache();
    });

    afterEach(function () {
        //Tear down.
        var leftOver = $('.testDiv');
        leftOver.empty();
        leftOver.remove();
    });

    it('[EGaneva] / should render calendar filter with unchecked checkbox then check it.', function () {
        var scope = rootScope.$new();
        scope.sfQueryData = { QueryItems: [] };

        var element = commonMethods.compileDirective(directiveMarkup, scope);
        scope.$digest();

        expect($(element).find('input#calendarInput').length).toEqual(1);
        expect($(element).find('input#calendarInput:checked').length).toEqual(0);

        $(element).find('input#calendarInput').click();

        expect($(element).find('input#calendarInput:checked').length).toEqual(1);
        expect($(element).find('.openSelectorBtn').is(':visible'));
    });

    it('[EGaneva] / should render calendar filter with selected items.', function () {
        var scope = rootScope.$new();
        scope.sfQueryData = {
            QueryItems: [{
                    IsGroup: true,
                    Name: "Calendars"
                }, {
                    Value: "6266ff24-2be2-611d-b54a-ff0000bc5146",
                    Condition: {
                        FieldName: "Parent.Id.ToString()",
                        FieldType: "System.String"
                    }
                }]
        };

        var element = commonMethods.compileDirective(directiveMarkup, scope);
        scope.$digest();

        expect($(element).find('input#calendarInput:checked').length).toEqual(1);
        expect($(element).find('.openSelectorBtn').is(':visible'));
        expect($(element).find('span:contains("Filtered")').length).toEqual(1);
    });
});