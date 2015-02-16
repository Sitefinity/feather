/* Tests for sf-media-service.js */
describe('sfMediaFilter', function () {
    var mediaFilter;

    beforeEach(module('sfServices'));

    beforeEach(inject(function ($injector) {
        mediaFilter = $injector.get('sfMediaFilter');
    }));

    var assertFilterSetCallsFunction = function (object, callback, val1, val2) {
        var filter = mediaFilter.newFilter();
        var called = false;

        filter.attachEvent(function () {
            called = true;
        });

        filter.set[object][callback](val1, val2);

        expect(called).toBe(true);

        return filter;
    };

    var assertAllButOnePropertyAreNull = function (filter, propName) {
        if (propName !== 'basic') {
            expect(filter.basic).toBe(null);
        }

        if (propName !== 'parent') {
            expect(filter.parent).toBe(null);
        }

        if (propName !== 'query') {
            expect(filter.query).toBe(null);
        }

        if (propName !== 'date') {
            expect(filter.date).toBe(null);
        }

        if (propName !== 'taxon') {
            expect(filter.taxon.id).toBe(null);
            expect(filter.taxon.field).toBe(null);
        }
    };

    it('[dzhenko] / a new filter should have all its properties null', function () {
        assertAllButOnePropertyAreNull(mediaFilter.newFilter(), null);
    });

    // basic
    it('[dzhenko] / attaching a function should not call it when changing basic none from initial state and set properties correctly', function () {
        var filter = mediaFilter.newFilter();
        var called = false;

        filter.attachEvent(function () {
            called = true;
        });

        filter.set.basic.none();

        expect(called).toBe(false);
        assertAllButOnePropertyAreNull(filter, null);
    });

    it('[dzhenko] / attaching a function should call it when changing basic none not from initial state and set properties correctly', function () {
        var filter = mediaFilter.newFilter();
        var called = false;

        filter.set.basic.allLibraries();

        filter.attachEvent(function () {
            called = true;
        });

        filter.set.basic.none();

        expect(called).toBe(true);
        assertAllButOnePropertyAreNull(filter, null);
    });

    it('[dzhenko] / attaching a function should call it when changing basic to all libraries and set properties correctly', function () {
        var filter = assertFilterSetCallsFunction('basic', 'allLibraries');
        assertAllButOnePropertyAreNull(filter, 'basic');
        expect(filter.basic).toEqual(filter.constants.basic.allLibraries);
    });

    it('[dzhenko] / attaching a function should call it when changing basic to own items and set properties correctly', function () {
        var filter = assertFilterSetCallsFunction('basic', 'ownItems');
        assertAllButOnePropertyAreNull(filter, 'basic');
        expect(filter.basic).toEqual(filter.constants.basic.ownItems);
    });

    it('[dzhenko] / attaching a function should call it when changing basic to recent items and set properties correctly', function () {
        var filter = assertFilterSetCallsFunction('basic', 'recentItems');
        assertAllButOnePropertyAreNull(filter, 'basic');
        expect(filter.basic).toEqual(filter.constants.basic.recentItems);
    });

    // date
    it('[dzhenko] / attaching a function should call it when changing date to all time value and set properties correctly', function () {
        var filter = assertFilterSetCallsFunction('date', 'all');
        assertAllButOnePropertyAreNull(filter, 'date');
        expect(filter.date).toEqual(filter.constants.anyTimeValue);
    });

    it('[dzhenko] / attaching a function should call it when changing date to some value and set properties correctly', function () {
        var date = new Date();
        var filter = assertFilterSetCallsFunction('date', 'to', date);
        assertAllButOnePropertyAreNull(filter, 'date');
        expect(filter.date).toEqual(date);
    });

    // parent
    it('[dzhenko] / attaching a function should call it when changing parent to some value and set properties correctly', function () {
        var parent = 'someParent';
        var filter = assertFilterSetCallsFunction('parent', 'to', parent);
        assertAllButOnePropertyAreNull(filter, 'parent');
        expect(filter.parent).toEqual(parent);
    });

    // taxon
    it('[dzhenko] / attaching a function should call it when changing taxon to some value and set properties correctly', function () {
        var taxonId = 'taxonId';
        var taxonField = 'taxonField';
        var filter = assertFilterSetCallsFunction('taxon', 'to', taxonId, taxonField);
        assertAllButOnePropertyAreNull(filter, 'taxon');
        expect(filter.taxon.id).toEqual(taxonId);
        expect(filter.taxon.field).toEqual(taxonField);
    });

    // query
    it('[dzhenko] / attaching a function should call it when changing query to some value and set properties correctly', function () {
        var query = 'query';
        var filter = assertFilterSetCallsFunction('query', 'to', query);
        assertAllButOnePropertyAreNull(filter, 'query');
        expect(filter.query).toEqual(query);
    });

    // query
    it('[dzhenko] / setting query when basic was all libraries should make basic to null', function () {
        var filter = mediaFilter.newFilter();
        filter.set.basic.allLibraries();

        expect(filter.basic).toEqual(filter.constants.basic.allLibraries);

        filter.set.query.to('some query');
        expect(filter.basic).toEqual(null);
    });
});