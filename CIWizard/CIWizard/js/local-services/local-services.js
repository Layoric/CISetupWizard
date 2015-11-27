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
            getUserRepo: function (repoName) {
                return $http.get('/user/repos/' + repoName);
            },
            getTeamCityConfigs: function () {
                return $http.get('/user/configs');
            }
        }
    }])
})();