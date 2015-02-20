describe('Search box', function () {
    var rootScope;
    var q;

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));
    beforeEach(module('sfSearchBox'));

    beforeEach(module(function ($provide) {
        $provide.value('serverContext', {});
    }));

    beforeEach(inject(function (_$rootScope_, _$q_) {
        rootScope = _$rootScope_;
        q = _$q_;
    }));

    beforeEach(function () {
        commonMethods.mockServerContextToEnableTemplateCache();
    });

    it('[NPetrova] / should show error message if no sfAction is passed', function () {
        var scope = rootScope.$new();
        var template = '<sf-search-box> </sf-search-box>';

        commonMethods.compileDirective(template, scope);
        var childScope = scope.$$childHead;

        expect(childScope.showError).toBe(true);
    });

    it('[NPetrova] / should init sfPlaceholder', function () {
        var scope = rootScope.$new();
        var template = '<sf-search-box sf-placeholder="Narrow by typing something"> </sf-search-box>';

        commonMethods.compileDirective(template, scope);
        var childScope = scope.$$childHead;

        expect(childScope.sfPlaceholder).toBeDefined();
        expect(childScope.sfPlaceholder).toBe("Narrow by typing something");
    });

    it('[NPetrova] / should init sfMinTextLength with default value', function () {
        var scope = rootScope.$new();
        var template = '<sf-search-box> </sf-search-box>';

        commonMethods.compileDirective(template, scope);
        var childScope = scope.$$childHead;

        expect(childScope.sfMinTextLength).toBeDefined();
        expect(childScope.sfMinTextLength).toBe(0);
    });

    it('[NPetrova] / should init sfMinTextLength with the passed value', function () {
        var scope = rootScope.$new();
        var template = '<sf-search-box sf-min-text-length="1"> </sf-search-box>';

        commonMethods.compileDirective(template, scope);
        var childScope = scope.$$childHead;

        expect(childScope.sfMinTextLength).toBeDefined();
        expect(childScope.sfMinTextLength).toBe('1');
    });

    it('[NPetrova] / should call sfAction when sfModel is defined and its length is more than 3 symbols', function () {
        var scope = rootScope.$new();
        scope.action = function (query) {
            expect(query).toBeDefined();
            expect(query).toBe('text');
        };
        var template = '<sf-search-box sf-action="action(query)"> </sf-search-box>';

        commonMethods.compileDirective(template, scope);

        var childScope = scope.$$childHead;

        childScope.sfModel = "text";
        childScope.sfSearchCallback();
    });

    it('[NPetrova] / should call sfAction when sfModel is not defined', function () {
        var scope = rootScope.$new();
        scope.action = function (query) {
            expect(query).not.toBeDefined();
        };
        var template = '<sf-search-box sf-action="action(query)"> </sf-search-box>';

        commonMethods.compileDirective(template, scope);

        var childScope = scope.$$childHead;

        childScope.sfSearchCallback();
    });

    it('[NPetrova] / should hide suggestions if suggestions are disabled', function () {
        var scope = rootScope.$new();
        var template = '<sf-search-box sf-enable-suggestions="false"> </sf-search-box>';

        commonMethods.compileDirective(template, scope);

        var childScope = scope.$$childHead;

        childScope.sfModel = "text";
        childScope.sfSearchCallback().then(function () {
            expect(childScope.showSuggestions).toBe(false);
        });
    });

    it('[NPetrova] / should show suggestions if suggestions are enabled', function () {
        var scope = rootScope.$new();
        scope.getSuggestions = function (query) {
            expect(query).toBeDefined();
            expect(query).toBe('text');

            var result = q.defer();
            result.resolve(['suggestion1', 'suggestion2']);

            return result.promise;
        };

        var template = '<sf-search-box sf-enable-suggestions="true" sf-get-suggestions="getSuggestions(query)"> </sf-search-box>';

        commonMethods.compileDirective(template, scope);

        var childScope = scope.$$childHead;

        childScope.sfModel = "text";
        childScope.sfSearchCallback().then(function () {
            expect(childScope.showSuggestions).toBe(true);
            expect(childScope.suggestions).toBeDefined();
            expect(childScope.suggestions.length).toBe(2);
            expect(childScope.suggestions[0]).toBe('suggestion1');
            expect(childScope.suggestions[1]).toBe('suggestion2');

        });
    });

    it('[pivanova] / should set sfClearSearchString', function () {
        var scope = rootScope.$new();

        var template = '<sf-search-box sf-clear-search-string="false"></sf-search-box>';

        commonMethods.compileDirective(template, scope);

        var childScope = scope.$$childHead;
        expect(childScope.sfClearSearchString).toBe(false);
    });

    it('[pivanova] / should clear sfModel if sfClearSearchString is changed', function () {
        var scope = rootScope.$new();
        scope.clearSearch = false;
        var template = '<sf-search-box sf-clear-search-string="clearSearch"></sf-search-box>';

        commonMethods.compileDirective(template, scope);

        var childScope = scope.$$childHead;
        childScope.sfModel = "model";

        scope.clearSearch = true;
        scope.$apply();

        expect(childScope.sfModel).toBe("");
    });
});
