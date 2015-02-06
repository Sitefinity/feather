var feather = angular.module('feather', ['angularMoment', 'ngRoute', 'ui.bootstrap', 'duScroll']).value('duScrollOffset', 140);

feather.directive("sticky", function ($window) {
    return function(rootScope, element, attrs) {
        angular.element($window).bind("scroll", function() {
			if(this.pageYOffset > 200) {
				rootScope.sticky = true;
			} else {
				rootScope.sticky = false;
			}
			rootScope.$apply();
        });
    };
});


feather.filter('length', function () {
	return function (input) {
			string = input.substr(0,6);
			return string;
	};
});  

feather.config(['$routeProvider',
	function($routeProvider) {
		$routeProvider.
			when('/home', {
				templateUrl: 'views/home.html',
				controller: 'HomeCtrl'
			}).
			when('/faq', {
				templateUrl: 'views/faq.html',
				controller: 'FaqCtrl'
			}).
			when('/progress', {
				templateUrl: 'views/progress.html',
				controller: 'ProgressCtrl'
			}).
			otherwise({
				redirectTo: '/home'
			});
}]);

feather.controller('HomeCtrl', function ($rootScope) {
	$rootScope.current = 'home';
});


feather.controller('FaqCtrl', function ($rootScope) {
	$rootScope.current = 'faq';
});

feather.controller('ProgressCtrl', function ($scope, $rootScope, $http, $timeout, $window) {
	$rootScope.current = 'progress';
	$scope.animate = true;
	$scope.totalCount = 0;
	$scope.totalCountCompleted = 0;
	$rootScope.loading = true;

	$http.get('json/progress.json').
	success(function(data, status, headers, config) {
		$scope.progress = data; 

		angular.forEach(data.sections, function(section, key) {
			$scope.totalCount =  $scope.totalCount + section.widgets.length;

			angular.forEach(section.widgets, function(widget, key) {

				if(widget.progress == 100) {
					$scope.totalCountCompleted++;
				}

				angular.forEach(widget.releases, function(release, key) {
					var strp = release.date;
					if(strp) {
						var str = strp.replace(/-/g, '/');
	 					release.date = str;
 					}
				});
			});
		});

		
		$rootScope.loading = false;

		$timeout(function() {
			$scope.animate = false;
		}, 500);
	}).
	error(function(data, status, headers, config) {
		$rootScope.loading = false;
	});
});

