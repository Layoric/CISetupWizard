/* global angular */
(function () {
    "use strict";
    var module = angular.module('create-project', [
        'local-services',
        'ciwizard.files'
    ]);

    module.controller('createProjectCtrl', ['$scope', '$routeParams', 'localServices','$timeout','$location',
        function ($scope, $routeParams, localServices,$timeout,$location) {
            $scope.loadingUserRepo = true;
            $scope.loadingSolutionDetails = true;
            $scope.ready = false;
            $scope.ownerName = $routeParams.ownerName;
            $scope.repoName = $routeParams.repoName;
            $scope.repoConfig = $scope.repoConfig || {};

            $scope.$watch('repoConfig.localOnlySite', function (newVal) {
                if(newVal)
                    $scope.repoConfig.hostName = null;
                else
                    $scope.repoConfig.port = null;
            });
            //Get repo
            localServices.getUserRepo($routeParams.ownerName, $routeParams.repoName).then(function (response) {
                $scope.repo = response.data.repo;
                $scope.loadingUserRepo = false;
                $scope.ready = !$scope.loadingUserRepo && !$scope.loadingSolutionDetails;
            }, function (response) {
                $scope.loadingUserRepo = false;
                $scope.ready = !$scope.loadingUserRepo && !$scope.loadingSolutionDetails;
            });

            localServices.getSolutionDetails($routeParams.ownerName, $routeParams.repoName).then(function (response) {
                $scope.repoConfig = response.data;
                //Default username
                $scope.repoConfig.msDeployUserName = "Administrator";
                $scope.loadingSolutionDetails = false;
                $scope.ready = !$scope.loadingUserRepo && !$scope.loadingSolutionDetails;
                $scope.isValidRepository = true;
            }, function (response) {
                //Error, offer to specify alternate branch?
                $scope.repoConfig = { templateType : 'Unknown'};
                $scope.loadingSolutionDetails = false;
                $scope.ready = !$scope.loadingUserRepo && !$scope.loadingSolutionDetails;
            });

            $scope.createBuild = function () {
                $scope.creating = true;
                var request = {
                    name: $routeParams.repoName,
                    ownerName: $routeParams.ownerName,
                    branch: $scope.repoConfig.branch,
                    repositoryUrl: $scope.repoConfig.repositoryUrl,
                    templateType: $scope.repoConfig.templateType,
                    workingDirectory: $scope.repoConfig.projectWorkingDirectory,
                    projectName: $scope.repoConfig.projectName,
                    privateRepository: $scope.repoConfig.privateRepository,
                    solutionPath: $scope.repoConfig.solutionPath,
                    hostName: $scope.repoConfig.hostName,
                    localOnlyApp: $scope.repoConfig.localOnlyApp,
                    port: $scope.repoConfig.port
                };
                localServices.createTeamCityBuild(request).then(function (response) {
                    $scope.success = true;
                    $scope.creating = false;
                    $timeout(function () {
                        //$location.path('/manage/' + $routeParams.ownerName + '/' + $routeParams.repoName);
                    },2000);
                }, function (response) {
                    $scope.creating = false;
                });
            };

        }]);
})();