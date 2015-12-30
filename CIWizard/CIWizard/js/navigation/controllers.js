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