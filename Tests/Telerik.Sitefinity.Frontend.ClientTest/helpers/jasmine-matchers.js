beforeEach(function () {
	jasmine.addMatchers({
	 	// Used to compare arrays of primitive values. Strict equals operator is used.
        toEqualArrayOfValues: function () {
			return {
				compare: function (expected, actual) {
					var valid = true;
					for (var i = 0; i < expected.length; i++) {
						if (expected[i] !== actual[i]) {
							valid = false;
							break;
						}
					}

					return {
						pass: valid
					};
				}
			};
        },

        // Used to compare arrays of objects. The items should have equal values on all provided property names.
        toEqualArrayOfObjects: function () {
			return {
				compare: function (expected, actual, properties) {
					if (!(properties && properties.length > 0)) { 
						throw new Error('Please provide array with property names based on whom the objects will be compared.')
					};

					var valid = true;
					for (var i = 0; i < expected.length; i++) {
						for(var j = 0; j < properties.length; j++) {
							var property = properties[j];
							
							var actualPropValue = actual[i].item ?
								 actual[i].item[property] :
								 actual[i][property];

							var expectedPropValue = expected[i][property];

							if (actualPropValue !== expectedPropValue) {
								valid = false;
								break;
							};
						}
					}
					
					return {
						pass: valid
					};
				}
			};
        },

		// Used to compare objects returned by $resource
		toEqualData: function(util, customEqualityTesters) { 
			return { 
				compare: function(actual, expected) { 
					return { 
						pass: angular.equals(actual, expected)
					};
				} 
			};
		},
		
		toBeJsonEqual: function () {
			var replacer = function (k, v) {
				if (typeof v === 'function') {
					v = v.toString();
				} else if (window['File'] && v instanceof File) {
					v = '[File]';
				} else if (window['FileList'] && v instanceof FileList) {
					v = '[FileList]';
				}
				return v;
			};
			
			return {
				compare: function(expected, actual) {
					var one = JSON.stringify(actual, replacer).replace(/(\\t|\\n)/g,''),
						two = JSON.stringify(expected, replacer).replace(/(\\t|\\n)/g,'');

					return {
						pass: one === two
					};
				}
			};
		}
    });
});