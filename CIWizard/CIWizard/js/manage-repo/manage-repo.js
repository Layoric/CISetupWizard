/* global angular */
(function () {
    "use strict";
    var module = angular.module('manage-repo', [
        'local-services',
        'ciwizard.files'
    ]);

    module.controller('managerRepoCtrl', ['$scope', '$routeParams', 'localServices','$location','$timeout',
        function ($scope, $routeParams, localServices,$location,$timeout) {
            $scope.loadingUserRepo = true;
            $scope.loadingSolutionDetails = true;
            $scope.ready = false;
            $scope.ownerName = $routeParams.ownerName;
            $scope.repoName = $routeParams.repoName;
            //Get repo
            localServices.getUserRepo($routeParams.ownerName, $routeParams.repoName).then(function (response) {
                $scope.repo = response.data.repo;
                $scope.loadingUserRepo = false;
                $scope.ready = true
            }, function (response) {
                $scope.loadingUserRepo = false;
                $scope.ready = true
            });

            localServices.getTeamCityProject($routeParams.ownerName,$routeParams.repoName).then(function (response) {
                $scope.projectExists = true;
                $scope.currentProject = response.data.project;
            }, function (response) {

            });

            $scope.deleteBuild = function () {
                localServices.deleteTeamCityBuild($routeParams.ownerName,$routeParams.repoName).then(function (response) {
                    $scope.projectExists = false;
                    $timeout(function () {
                        $location.path('/');
                    },1000);
                })
            };

            localServices.getTeamCityUrl().then(function (response) {
                $scope.tcProjectUrl = response.data.url + 'admin/editProject.html?projectId=SS_' + $routeParams.ownerName + '_' + $routeParams.repoName;
            })
        }]);
})();