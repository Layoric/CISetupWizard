/* global angular */
(function () {
    "use strict";
    var module = angular.module('authentication', [

    ]);

    module.service('authentication', ['$http','$q', function ($http,$q) {
        var userDetails = null;
        return {
            isAuthenticated: function () {
                return $http.get('/session-info');
            },
            getUserDetails: function () {
                var deferred = $q.defer();
                if(userDetails) {
                    deferred.resolve(userDetails);
                }
                $http.get('/session-info').then(function (response) {
                    userDetails = response.data;
                    deferred.resolve(userDetails);
                });
                return deferred.promise;
            },
            getUserRepos: function () {
                return $http.get('/user/repos');
            }
        }
    }]);
})();