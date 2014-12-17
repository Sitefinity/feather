describe('items tree', function  () {
	describe('sfTreeHelper service', function  () {
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

		var sfTreeHelper;

		function constructPredecessorsCollection (levels) {
			var predecessors = [];
			for (var i = 0; i < levels.length; i++) {
				Array.prototype.push.apply(predecessors, levels[i]);
			};
			return predecessors;
		}

		function constructPredecessorsTree (levels) {
			var firstLevel = levels[0];
			var currentLevel = levels[0];
			for (var i = 0; i < levels.length - 1; i++) {
				//we assign the children to the first element of the level
				currentLevel[0].items = levels[i + 1];
				currentLevel = levels[i + 1];
			};
			return firstLevel;
		}

		function constructParentsIdsPath (levels) {
			return [levels[0][0], levels[1][0]].map(function (item) {
				return item.Id;	
			});
		}

		function equalArrayOfItems (actual, expected) {
			var valid = true;
            for (var i = 0; i < expected.length; i++) {
                if (expected[i].Id !== actual[i].Id) {
                    valid = false;
                    break;
                }
            }
            return valid;
		}

		function equalArrayOfValues (actual, expected) {
			var valid = true;
            for (var i = 0; i < expected.length; i++) {
                if (expected[i] !== actual[i]) {
                    valid = false;
                    break;
                }
            }
            return valid;
		}

		function equalTree (actual, expected) {
			var actualCurrentLevel = actual.items;
			var expectedCurrentLevel = expected;
			var currentParentIndex = 0;
			var valid = true;

			while(actualCurrentLevel || expectedCurrentLevel) {				
				valid = equalArrayOfItems(actualCurrentLevel, expectedCurrentLevel);
				if (!valid) break;

				var currentParentId = actual.parentsIds[currentParentIndex];
				var actualNextLevelParent = actualCurrentLevel.filter(function  (item) {
					return item.Id === currentParentId; 
				})[0];

				if(!actualNextLevelParent) break;

				var actualCurrentLevel = actualNextLevelParent.items;

				var expectedNextLevelParent = expectedCurrentLevel.filter(function  (item) {
					return item.Id === currentParentId; 
				})[0];

				if(!expectedNextLevelParent) break;

				var expectedCurrentLevel = expectedNextLevelParent.items;

				currentParentIndex++;
			}
			return valid;
		}

		beforeEach(function () {
	        this.addMatchers({
	            // Used to compare arrays of data items
	            toEqualArrayOfItems: function (expected) {
	                return equalArrayOfItems(this.actual, expected);
	            },

	            // Used to compare arrays of primitive values
	            toEqualArrayOfValues: function (expected) {
	                return equalArrayOfValues(this.actual, expected);
	            },

	            // Used to compare two trees with given parents path from the root to the bottom
	            toEqualTree: function (expected) {
	            	return equalTree(this.actual, expected);
	            }
	        });
	    });

		//Load the module under test.
    	beforeEach(module('sfSelectors'));

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
	});
});