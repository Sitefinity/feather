describe('items tree tests', function  () {
    var predecessorsLevels = [
            //First level
            [{ Id: '1.1', ParentId: '0'}, { Id: '1.2', ParentId: '0'}, { Id: '1.3', ParentId: '0'}],

            //Second level
            [{ Id: '2.1', ParentId: '1.2'}, { Id: '2.2', ParentId: '1.2'}, { Id: '2.3', ParentId: '1.2'}],

            //Third level
            [{ Id: '3.1', ParentId: '2.2'}, { Id: '3.2', ParentId: '2.2'}, { Id: '3.3', ParentId: '2.2'}]];

    // The items that contains children have to be always in the beginning of their level.
    var sortedLevels = [
            //First level
            [{ Id: '1.2', ParentId: '0'}, { Id: '1.1', ParentId: '0'}, { Id: '1.3', ParentId: '0'}],

            //Second level
            [{ Id: '2.2', ParentId: '1.2'}, { Id: '2.1', ParentId: '1.2'}, { Id: '2.3', ParentId: '1.2'}],

            //Third level
            [{ Id: '3.2', ParentId: '2.2'}, { Id: '3.1', ParentId: '2.2'}, { Id: '3.3', ParentId: '2.2'}]];

    function constructPredecessorsTree (levels) {
        var firstLevel = levels[0];
        var currentLevel = levels[0];
        for (var i = 0; i < levels.length - 1; i++) {
            //we assign the children to the first element of the level
            currentLevel[0].items = levels[i + 1];
            currentLevel = levels[i + 1];
        }

        return firstLevel;
    }

    function constructPredecessorsCollection (levels) {
        var predecessors = [];
        for (var i = 0; i < levels.length; i++) {
            Array.prototype.push.apply(predecessors, levels[i]);
        }

        return predecessors;
    }

    //Load the module under test.
    beforeEach(module('sfSelectors'));

    //Load the module that contains the cached templates.
    beforeEach(module('templates'));

    var $q;
    beforeEach(inject(function (_$q_) {
        $q = _$q_;
    }));

    describe('sfTreeHelper factory', function  () {
        var sfTreeHelper;

        function constructParentsIdsPath (levels) {
            return [levels[0][0], levels[1][0]].map(function (item) {
                return item.Id;
            });
        }

        function equalTree (actual, expected) {
            var actualCurrentLevel = actual.items;
            var expectedCurrentLevel = expected;
            var currentParentIndex = 0;
            var valid = true;

            var isParentIdFunc = function (parentId) {
                return function (item) {
                    return item.Id === parentId;
                };
            };

            while(actualCurrentLevel || expectedCurrentLevel) {
                try {
                    // throws error if not true
                    expect(actualCurrentLevel).toEqualArrayOfObjects(expectedCurrentLevel, ['Id']);
                }
                catch (err) {
                    valid = false;
                    break;
                }

                var currentParentId = actual.parentsIds[currentParentIndex];
                
                var actualNextLevelParent = actualCurrentLevel.filter(isParentIdFunc(currentParentId))[0];

                if(!actualNextLevelParent) break;

                actualCurrentLevel = actualNextLevelParent.items;

                var expectedNextLevelParent = expectedCurrentLevel.filter(isParentIdFunc(currentParentId))[0];

                if(!expectedNextLevelParent) break;

                expectedCurrentLevel = expectedNextLevelParent.items;

                currentParentIndex++;
            }
            return valid;
        }

        beforeEach(function () {
            this.addMatchers({
                // Used to compare two trees with given parents path from the root to the bottom
                toEqualTree: function (expected) {
                    return equalTree(this.actual, expected);
                }
            });
        });

        beforeEach(inject(function (_sfTreeHelper_) {
            sfTreeHelper = _sfTreeHelper_;
        }));

        it('[GeorgiMateev] / should construct tree to an item in third level.', function  () {
            //input
            var selectedItemId = '3.2';
            var predecessors = constructPredecessorsCollection(predecessorsLevels);

            //expected output
            var tree = constructPredecessorsTree(sortedLevels);
            var parentsIds = constructParentsIdsPath(sortedLevels);

            //compute result
            var result = sfTreeHelper.constructPredecessorsTree(predecessors, selectedItemId);

            //assertions
            expect(result.parentsIds).toEqualArrayOfValues(parentsIds);
            expect(result).toEqualTree(tree);
        });

        it('[GeorgiMateev] / should return only root level if the selected item is in the root level.', function () {
            var selectedItemId = '1.2';

            //construct tree only from the first level
            var result = sfTreeHelper.constructPredecessorsTree(predecessorsLevels[0], selectedItemId);

            //assertions
            expect(result.parentsIds).toEqualArrayOfValues([]);
            expect(result).toEqualTree(sortedLevels[0]);
        });
    });

    describe('sfHybridHierarchicalDataSource factory', function () {
        var sfHybridHierarchicalDataSource;

        beforeEach(inject(function (_sfHybridHierarchicalDataSource_) {
            sfHybridHierarchicalDataSource = _sfHybridHierarchicalDataSource_;
        }));

        var getChildrenSpy = jasmine.createSpy('getChildren').andCallFake(function (id) {
            var children = $q.defer();
            children.resolve(sortedLevels[2]);
            return children.promise;
        });

        var model = {
            id: 'Id',
            hasChildren: 'HasChildren'
        };

        it('[GeorgiMateev] / it should fetch data from hierarchical collection when node is expanded.', function () {
            var tree = constructPredecessorsTree(sortedLevels);

            var dataSource = sfHybridHierarchicalDataSource
                .getDataSource(model, tree, getChildrenSpy);

            dataSource.bind('change', function (e) {
                var children = this.data();

                expect(children).toEqualArrayOfObjects(sortedLevels[2], ['Id']);
                expect(getChildrenSpy.callCount).toBe(0);
            });

            dataSource.read({Id: '2.2'});
        });

        it('[GeorgiMateev] / it should call the web service if the node is not in the provided collection.', function () {
            var tree = constructPredecessorsTree(sortedLevels);

            var dataSource = sfHybridHierarchicalDataSource
                .getDataSource(model, tree, getChildrenSpy);

            dataSource.bind('change', function (e) {
                var children = this.data();

                expect(children).toEqualArrayOfObjects(sortedLevels[2], ['Id']);
                expect(getChildrenSpy.callCount).toBe(0);
            });

            // The id is not in the tree.
            dataSource.read({Id: '4.2'});
            expect(getChildrenSpy).toHaveBeenCalledWith('4.2');
        });
    });

    describe('sf-items-tree directive', function () {
        var scope;
        var $timeout;

        beforeEach(inject(function ($rootScope, _$timeout_) {
            scope = $rootScope.$new();
            $timeout = _$timeout_;
        }));

        beforeEach(function () {
            commonMethods.mockServerContextToEnableTemplateCache();
        });

        it('[GeorgiMateev] / it should call the predecessors end point when the directive is loaded.', function () {
        var template = '<sf-items-tree '+
                         'sf-expand-selection="expandSelection" '+
                         'sf-selected-ids-promise="sfSelectedIdsPromise" '+
                         'sf-get-predecessors="getPredecessors(itemId)" '+
                         'class="k-treeview--list-group"></sf-items-tree>';

            scope.getPredecessors = jasmine.createSpy('getPredecessors')
                .andCallFake(function (itemId) {
                    var defered = $q.defer();
                    var predecessors = constructPredecessorsCollection(predecessorsLevels);
                    defered.resolve({ Items: predecessors });

                    return defered.promise;
                });

            scope.expandSelection = true;

            var defered = $q.defer();
            scope.sfSelectedIdsPromise = defered.promise;

            commonMethods.compileDirective(template, scope);

            defered.resolve(['3.2']);

            // Looks like kendo is using a timeout in its widgets
            // so this method is executing all callbacks of the $timeout mock.
            $timeout.flush(1000);

            expect(scope.getPredecessors).toHaveBeenCalledWith('3.2');
        });

        it('[GeorgiMateev] / it should show items when expanding is disabled.', function () {
        var template = '<sf-items-tree '+
                         'sf-selected-ids-promise="sfSelectedIdsPromise" '+
                         'sf-get-predecessors="getPredecessors(itemId)" '+
                         'sf-items-promise=sfItemsPromise' +
                         'class="k-treeview--list-group"></sf-items-tree>';

            scope.getPredecessors = jasmine.createSpy('getPredecessors');

            scope.expandSelection = false;

            // Should not expand even if an item is selected.
            var deferedSelection = $q.defer();
            scope.sfSelectedIdsPromise = deferedSelection.promise;

            var deferedItems = $q.defer();
            scope.sfItemsPromise = deferedItems.promise;

            commonMethods.compileDirective(template, scope);

            deferedSelection.resolve(['3.2']);
            deferedItems.resolve(predecessorsLevels[0]);

            // Looks like kendo is using a timeout in its widgets
            // so this method is executing all callbacks of the $timeout mock.
            $timeout.flush(1000);

            expect(scope.getPredecessors).not.toHaveBeenCalled();
        });

    });
});
