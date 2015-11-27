/* global angular */
(function () {
    "use strict";
    var module = angular.module('home', [
        'authentication'
    ]);
    module.controller('homeCtrl', ['$scope', 'authentication', '$timeout',
        function ($scope, authentication, $timeout) {
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
        }
    ]);

    module.directive('githubLoginSplash', [
        function () {
            return {
                restrict: 'E',
                scope: {},
                template: '<div class="github-login-container">' +
                '<a href="/auth/github" class="github-login" title="Login using GitHub!">' +
                '<img src="/img/Octocat.jpg" width="400"/>' +
                '</a>' +
                '<span>Login using GitHub</span>' +
                '</div>',
                controller: function ($scope, $element, $attrs) {

                }
            }
        }
    ]);

    module.directive('githubSelectRepo', ['githubService', 'authentication',
        function (githubService, authentication) {
            return {
                restrict: 'E',
                scope: {},
                templateUrl: 'js/home/show-repos.html',
                controller: function ($scope, $element, $attrs) {
                    $scope.selectRepo = function (repo) {
                        githubService.getFiles(repo.owner.login, repo.name).then(function (response) {
                            var files = response.data.tree.filter(function (tree) {
                                return tree.type === 'blob' && endsWith(tree.path, '.sln');
                            });
                            //Assume one .sln file, grab the first
                            $scope.slnPath = files[0].path;
                        });
                    };

                    authentication.getUserDetails().then(function (response) {
                        $scope.isLoading = true;
                        var orgs = [];
                        authentication.getUserRepos().then(function (response) {
                            angular.forEach(response.data.repos, function (repo) {
                                var org = getObjFromArrayWithPropValue(orgs,'orgName',repo.owner.login);
                                if(!org) {
                                    org = { orgName: repo.owner.login};
                                    orgs.push(org)
                                }
                                org.repos = org.repos || [];
                                org.repos.push(repo);
                            });
                            //Put user 'org' first
                            var userOrg = getObjFromArrayWithPropValue(orgs, 'orgName', response.userName);
                            orgs.move(orgs.indexOf(userOrg),0);
                            $scope.isLoading = false;
                            $scope.allOrgs = orgs;
                        });
                    });

                    function endsWith(str, suffix) {
                        return str.indexOf(suffix, str.length - suffix.length) !== -1;
                    }

                    function getObjFromArrayWithPropValue(array, propName, propVal) {
                        for(var i = 0; i < array.length; i++) {
                            var obj = array[i];
                            if(obj && obj.hasOwnProperty(propName) && obj[propName] === propVal) {
                                return obj;
                            }
                        }
                        return null;
                    }
                }
            }
        }
    ]);

    module.service('githubService', ['$http', function ($http) {
        var ghBase = "https://api.github.com/";
        return {
            getRepos: function () {
                return $http.get(ghBase + 'user/repos?per_page=101');
            },
            getFiles: function (userName, repoName) {
                return $http.get(ghBase + 'repos/' + repoName + '/git/trees/master?recursive=1');
            }
        }
    }]);
})();

Array.prototype.move = function(from, to) {
    this.splice(to, 0, this.splice(from, 1)[0]);
};