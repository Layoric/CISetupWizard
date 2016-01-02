/* global angular */
(function () {
    "use strict";
    var module = angular.module('home', [
        'authentication'
    ]);
    module.controller('homeCtrl', ['$scope', 'authentication', '$timeout','$location',
        function ($scope, authentication, $timeout,$location) {
            $scope.teamCityConfigs = [];
            $scope.excludedRepositories = [];
            $scope.$watch('teamCityConfigs', function (newVal) {
                if(newVal) {
                    console.log(newVal.length);
                    for(var i = 0; i < newVal.length; i++) {
                        var teamCityRepo = newVal[i];
                        console.log(teamCityRepo);
                        if(teamCityRepo == null)
                            continue;
                        $scope.excludedRepositories.push({
                            name: teamCityRepo.name,
                            ownerName: teamCityRepo.owner.login
                        });
                    }
                }
            });

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
                $location.path('create/' + repo.owner.login + '/' + repo.name);
            }
        }
    ]);
})();

Array.prototype.move = function(from, to) {
    this.splice(to, 0, this.splice(from, 1)[0]);
};