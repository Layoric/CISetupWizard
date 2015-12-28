/* global angular */
(function () {
    "use strict";
    var app = angular.module('navigation.controllers', []);

    app.controller('navigationCtrl', [
        '$scope', '$location', '$http','authentication', '$timeout',
        function ($scope, $location, $http,authentication, $timeout) {
            /**
             * @return {boolean}
             */
            $scope.IsRouteActive = function (routePath) {
                console.log($location.path());
                console.log(routePath === $location.path());
                return routePath === $location.path();
            };

            authentication.isAuthenticated().then(function(response) {
                $timeout(function () {
                    $scope.isAuthenticated = true;
                });
            }, function(response) {
                //Failed
                $timeout(function() {
                    $scope.isAuthenticated = false;
                });
            });
        }
    ]);
})();