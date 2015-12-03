/* global angular */
(function () {
    "use strict";
    var module = angular.module('home', [
        'authentication'
    ]);
    module.controller('homeCtrl', ['$scope', 'authentication', '$timeout','$location',
        function ($scope, authentication, $timeout,$location) {
            authentication.isAuthenticated().then(function (response) {
                $timeout(function () {
                    $scope.isAuthenticated = true;
                });
            }, function (response) {
                //Failed
                $timeout(function () {
                    $scope.isAuthenticated = false;
                });
            });

            $scope.onRepoSelect = function (repo) {
                $location.path('manage/' + repo.owner.login + '/' + repo.name);
            }
        }
    ]);
})();

Array.prototype.move = function(from, to) {
    this.splice(to, 0, this.splice(from, 1)[0]);
};