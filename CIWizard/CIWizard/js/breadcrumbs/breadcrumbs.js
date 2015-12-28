/* global angular */
(function () {
    var module = angular.module('breadcrumbs', [

    ]);
    module.directive('ciBreadcrumbs', ['authentication','$location',
        function (authentication,$location) {
            return {
                restrict: 'E',
                scope: {},
                templateUrl: 'js/breadcrumbs/breadcrumbs.html',
                controller: function ($scope, $element, $attrs) {
                    /**
                     * @return {boolean}
                     */
                    $scope.IsRouteActive = function (routePath) {
                        return routePath === $location.path();
                    };
                }
            }
        }
    ]);
})();