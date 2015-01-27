describe('sfInfiniteScroll', function () {

    var scope,
        compile;

    beforeEach(module('sfInfiniteScroll'));

    beforeEach(inject(function ($rootScope, $compile) {
        scope = $rootScope.$new();
        compile = $compile;
    }));


    it('sets the overflow-y property to scroll on initialization', function () {

        var markup = '<div sf-infinite-scroll="loadMore()"></div>',
            element = compile(markup)(scope);

        scope.$digest();

        expect(element.css('overflow-y')).toEqual('scroll');
    });
    
    it('calls the needsData delegate if element is scrolled to the bottom', function () {

        var markup = '<div style="height:200px;" sf-infinite-scroll="loadMore()">' +
                     '  <ul>' +
                     '    <li ng-repeat="item in items">{{item}}</li>' +
                     '  </ul>' +
                     '</div>',
            wasLoadMoreCalled = false,
            element;

        scope.items = [];

        for (var i = 0; i < 100; i++) {
            scope.items.push(i);
        }

        scope.loadMore = function () {
            wasLoadMoreCalled = true;
        }

        element = compile(markup)(scope);
        scope.$digest();

        // append the element to the body in order for scroll
        // properties to start functioning
        element.appendTo('body');

        // scroll to the bottom
        $(element).scrollTop($(element).get(0).scrollHeight);
        // due to single thread nature of JS we need to trigger
        // the event manually
        $(element).trigger('scroll');

        // we verify that wasLoadMoreCalled is true, which is what our
        // fake method for loading more items on the scope does
        expect(wasLoadMoreCalled).toBe(true);
    });

    it('does not call needsData delegate if element is scrolled, but not all the way to the bottom', function () {

        var markup = '<div style="height:200px;" sf-infinite-scroll="loadMore()">' +
                     '  <ul>' +
                     '    <li ng-repeat="item in items">{{item}}</li>' +
                     '  </ul>' +
                     '</div>',
            wasLoadMoreCalled = false,
            element;

        scope.items = [];

        for (var i = 0; i < 100; i++) {
            scope.items.push(i);
        }

        scope.loadMore = function () {
            wasLoadMoreCalled = true;
        }

        element = compile(markup)(scope);
        scope.$digest();

        // append the element to the body in order for scroll
        // properties to start functioning
        element.appendTo('body');

        // scroll to the bottom
        $(element).scrollTop($(element).get(0).scrollHeight - 300);
        // due to single thread nature of JS we need to trigger
        // the event manually
        $(element).trigger('scroll');

        // we verify that wasLoadMoreCalled is true, which is what our
        // fake method for loading more items on the scope does
        expect(wasLoadMoreCalled).toBe(false);
    });

});