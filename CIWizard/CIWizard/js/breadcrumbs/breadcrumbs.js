/* global angular */
(function () {
    var module = angular.module('breadcrumbs', [

    ]);
    module.directive('ciBreadcrumbs', ['authentication','$location',
        function (authentication,$location) {
            return {
                restrict: 'E',
                scope: {
                    crumbs: '='
                },
                templateUrl: 'js/breadcrumbs/breadcrumbs.html',
                controller: function ($scope, $element, $attrs) {
                    /**
                     * @return {boolean}
                     */
                    $scope.IsActivePath = function (routePath) {
                        return $location.path().indexOf(routePath) === 0;
                    };
                }
            }
        }
    ]);

    module.controller('breadcrumbsCtrl', ['$scope','$route', function($scope,$route) {
        $scope.validRoutes = [
            {path:'/manage', name: 'Manage'},
            {path:'/create', name: 'Create'}
        ];

        function constructBreadcrumbs(routes) {
            $scope.breadcrumbs = [];
            for(var i = 0; i < routes.length;i++) {
                var route = routes[i];
                $scope.breadcrumbs.push(route);
            }
        }

        $scope.$on('$routeChangeSuccess', function(event,newRoute) {
            var foundCrumb = false;
            console.log(newRoute);
            var routes = [];
            for(var i = 0; i < $scope.validRoutes.length; i++) {
                var route = $scope.validRoutes[i];
                if(newRoute.$$route.originalPath.indexOf(route.path) > -1) {
                    foundCrumb = true;
                    routes.push(route);
                }
            }

            constructBreadcrumbs(routes);
        });
    }]);
})();