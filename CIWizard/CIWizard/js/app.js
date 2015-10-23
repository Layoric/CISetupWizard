/* global angular */
(function () {
    "use strict";
    // Declare app level module which depends on filters, and services
    var module = angular.module('helloApp', [
        'ngRoute',
        'helloApp.controllers',
        'navigation.controllers'
    ]);

    module.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
        $routeProvider.when('/', { templateUrl: '/partials/hello/hello.html', controller: 'helloCtrl' });
        $routeProvider.when('/view1', { templateUrl: '/partials/partial1.html' });
        $routeProvider.when('/view2', { templateUrl: '/partials/partial2.html' });
        $routeProvider.when('/404', { templateUrl: '/partials/404.html' });
        $routeProvider.otherwise({ redirectTo: '/404' });

        $locationProvider.html5Mode(true);
    }]);

    module.service('githubService', ['$http', function ($http) {
        var ghBase = "https://api.github.com/";
        return {
            getUserRepos: function(userName) {
                return $http.get(ghBase + 'users/' + userName + '/repos?per_page=100');
            },
            getFiles: function(userName, repoName) {
                return $http.get(ghBase + 'repos/' + userName + '/' + repoName + '/git/trees/master?recursive=1');
            }
        }
    }]);

    module.controller('githubController', [
        '$scope', 'githubService', function ($scope, githubService) {
            $scope.getUserRepos = function() {
                githubService.getUserRepos($scope.userName).then(function(response) {
                    $scope.userRepos = response.data;
                    console.log($scope.userRepos);
                });
            }

            $scope.selectRepo = function(repo) {
                githubService.getFiles(repo.owner.login, repo.name).then(function(response) {
                    var files = response.data.tree.filter(function(tree) {
                        return tree.type === 'blob' && endsWith(tree.path, '.sln');
                    });
                    //Assume one .sln file, grab the first
                    $scope.slnPath = files[0].path;
                });
            }
        }
    ]);

})();

function endsWith(str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
}