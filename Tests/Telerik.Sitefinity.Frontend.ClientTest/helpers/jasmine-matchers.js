beforeEach(function () {
	this.addMatchers({
	 	// Used to compare arrays of primitive values. Strict equals operator is used.
        toEqualArrayOfValues: function (expected) {
            var valid = true;
            for (var i = 0; i < expected.length; i++) {
                if (expected[i] !== this.actual[i]) {
                    valid = false;
                    break;
                }
            }
            return valid;
        },

        // Used to compare arrays of objects. The items should have equal values on all provided property names.
        toEqualArrayOfObjects: function (expected, properties) {
            if (!(properties && properties.length > 0)) { 
                throw new Error('Please provide array with property names based on whom the objects will be compared.')
            };

            var valid = true;
            for (var i = 0; i < expected.length; i++) {
            	for(var j = 0; j < properties.length; j++) {
            		var property = properties[j];
            		
            		var actualPropValue = this.actual[i].item ?
	            		 this.actual[i].item[property] :
	            		 this.actual[i][property];

	            	var expectedPropValue = expected[i][property];

	            	if (actualPropValue !== expectedPropValue) {
	            		valid = false;
	            		break;
	            	};
            	}
            }
            return valid;
        },

        // Used to compare objects returned by $resource
        toEqualData: function (expected) {
            return angular.equals(this.actual, expected);
        }
    });
});