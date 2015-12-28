/* global angular */
(function () {
    "use strict";
    var module = angular.module('local-services', [

    ]);

    module.service('localServices', [
        '$http', function($http) {
            return {
                getUserRepos: function() {
                    return $http.get('/user/repos');
                },
                getUserRepo: function(ownerName, repoName) {
                    return $http.get('/user/repos/' + ownerName + '/' + repoName);
                },
                getSolutionDetails: function(ownerName, repoName) {
                    return $http.get('/user/repos/' + ownerName + '/' + repoName + '/solution');
                },
                getTeamCityConfigs: function() {
                    return $http.get('/user/configs');
                },
                getTeamCityBuilds: function() {
                    return $http.get('/user/builds')
                },
                getTeamCityBuild: function(owner, repoName) {
                    return $http.get('/user/builds/' + owner + '/' + repoName);
                },
                getTeamCityProject: function(owner, repoName) {
                    return $http.get('/user/projects/' + owner + '/' + repoName);
                },
                getTeamCityProjects: function() {
                    return $http.get('/user/projects');
                },
                createTeamCityBuild: function(createTeamCityReq) {
                    return $http.post('/user/build', createTeamCityReq);
                },
                deleteTeamCityBuild: function(owner, repoName) {
                    return $http.delete('/user/projects/' + owner + '/' + repoName);
                },
                getProjectFiles: function (owner,repoName) {
                    return $http.get('/user/projects/' + owner + '/' + repoName + '/files');
                }
            }
        }
    ]);
})();