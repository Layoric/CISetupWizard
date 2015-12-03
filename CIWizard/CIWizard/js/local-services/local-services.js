/* global angular */
(function () {
    "use strict";
    var module = angular.module('local-services', [

    ]);

    module.service('localServices', ['$http',function ($http) {
        return {
            getUserRepos: function () {
                return $http.get('/user/repos');
            },
            getUserRepo: function (ownerName,repoName) {
                return $http.get('/user/repos/' + ownerName + '/' + repoName);
            },
            getSolutionDetails: function (ownerName, repoName) {
                return $http.get('/user/repos/' + ownerName + '/' + repoName + '/solution');
            },
            getTeamCityConfigs: function () {
                return $http.get('/user/configs');
            },
            createTeamCityBuild: function (createTeamCityReq) {
                return $http.post('/user/build', createTeamCityReq);
            }
        }
    }])
})();