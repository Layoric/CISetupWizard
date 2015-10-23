/* global angular */
(function () {
    "use strict";
    var app = angular.module('navigation.controllers', []);

    app.controller('navigationCtrl', [
        '$scope', '$location', '$http',
        function ($scope, $location, $http) {
            /**
             * @return {boolean}
             */
            $scope.IsRouteActive = function (routePath) {
                return routePath === $location.path();
            };
        }
    ]);
})();