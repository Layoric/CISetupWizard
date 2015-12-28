/* global angular */
(function () {
    "use strict";
    var module = angular.module('manage-repo', [
        'local-services',
        'ciwizard.files'
    ]);

    module.controller('managerRepoCtrl', ['$scope', '$routeParams', 'localServices', 'fileUpload',
        function ($scope, $routeParams, localServices,fileUpload) {
            $scope.loadingUserRepo = true;
            $scope.loadingSolutionDetails = true;
            $scope.ready = false;
            $scope.ownerName = $routeParams.ownerName;
            $scope.repoName = $routeParams.repoName;
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
                $scope.loadingSolutionDetails = false;
                $scope.ready = !$scope.loadingUserRepo && !$scope.loadingSolutionDetails;
            });

            localServices.getTeamCityProject($routeParams.ownerName,$routeParams.repoName).then(function (response) {
                $scope.projectExists = true;
                $scope.currentProject = response.data.project;
                localServices.getTeamCityBuild($routeParams.ownerName,$routeParams.repoName).then(function (response) {
                    $scope.buildConfigExists = true;
                    $scope.buildStatus = response.data.status;
                    var dt = new Date(response.data.lastUpdate);
                    $scope.buildLastUpdate = dt.toLocaleDateString() + " " + dt.toLocaleTimeString();
                }, function (response) {
                    // Do nothing, user hasn't created/managed build for this project yet.
                });
            }, function (response) {

            });

            $scope.uploadFile = function () {
                var file = $scope.appSettingsFile;
                console.log('file is ' + JSON.stringify(file));
                var uploadUrl = '/user/projects/' + $scope.ownerName + '/' + $scope.repoName + '/settings';
                fileUpload.uploadFileToUrl(file, uploadUrl);
            };


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
                    hostName: $scope.repoConfig.hostName
                };
                localServices.createTeamCityBuild(request).then(function (response) {
                    $scope.success = true;
                    $scope.creating = false;
                }, function (response) {
                    $scope.creating = false;
                });
            };

            $scope.deleteBuild = function () {
                localServices.deleteTeamCityBuild($routeParams.ownerName,$routeParams.repoName).then(function (response) {
                    $scope.projectExists = false;
                })
            };

            $scope.$watch('success', function (newVal) {
                if(newVal === true) {

                }
            });

            localServices.getProjectFiles($routeParams.ownerName,$routeParams.repoName).then(function (response) {
                console.log(response.data);
                for(var i = 0; i < response.data.fileNames.length;i++) {
                    var file = response.data.fileNames[i];
                    if(file === "appsettings.txt")
                        $scope.hasAppSettings = true;
                }
                $scope.files = response.data.fileNames;
            });
        }]);
})();