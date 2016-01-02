/* global angular */
(function () {
    "use strict";
    var module = angular.module('github', [
        'authentication',
        'local-services'
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

    module.directive('githubSelectRepo', ['githubService', 'authentication','localServices',
        function (githubService, authentication, localServices) {
            return {
                restrict: 'E',
                scope: {
                    onSelect: '&',
                    excludedRepositories: '='
                },
                templateUrl: 'js/github/show-repos.html',
                controller: function ($scope, $element, $attrs) {
                    $scope.selectRepo = function (repo) {
                        if($scope.onSelect) {
                            $scope.onSelect({repo:repo});
                        }
                    };

                    $scope.$watch('search.orgName', function (newVal) {
                       //On change, files orgs
                        if(newVal && newVal != '') {
                            $scope.selectedOrgs = $scope.allOrgs.filter(function (val) {
                                return val.orgName === newVal;
                            })
                        } else {
                            $scope.selectedOrgs = angular.copy($scope.allOrgs);
                        }
                    });

                    $scope.$watch('allRepos', function () {
                        if($scope.allRepos && $scope.allRepos.length > 0) {
                            filterRepositories();
                        }
                    });

                    $scope.$watch('excludedRepositories', function () {
                        if($scope.excludedRepositories && $scope.excludedRepositories.length > 0) {
                            filterRepositories();
                        }
                    });

                    function filterRepositories() {
                        var orgs = [];
                        if(!$scope.allRepos || $scope.allRepos.length == 0) {
                            return;
                        }


                        for(var repoIndex = 0; repoIndex < $scope.allRepos.length; repoIndex++) {
                            var repo = $scope.allRepos[repoIndex];
                            var skipRepo = false;
                            if($scope.excludedRepositories && $scope.excludedRepositories.length > 0) {
                                for(var excludeIndex = 0; excludeIndex < $scope.excludedRepositories.length; excludeIndex++) {
                                    var excludedRepo = $scope.excludedRepositories[excludeIndex];
                                    if(excludedRepo.orgName == repo.owner.login &&
                                        excludedRepo.name == repo.name)  {
                                        skipRepo = true;
                                        break;
                                    }
                                }

                                if(skipRepo)
                                    continue;
                            }


                            var org = getObjFromArrayWithPropValue(orgs,'orgName',repo.owner.login);
                            if(!org) {
                                org = { orgName: repo.owner.login};
                                orgs.push(org)
                            }
                            org.repos = org.repos || [];
                            org.repos.push(repo);
                        }
                        //Put user 'org' first
                        var userOrg = getObjFromArrayWithPropValue(orgs, 'orgName', $scope.userOwerName);
                        orgs.move(orgs.indexOf(userOrg),0);
                        $scope.isLoading = false;
                        $scope.allOrgs = orgs;
                        $scope.selectedOrgs = angular.copy(orgs);
                    }

                    authentication.getUserDetails().then(function (userResponse) {
                        $scope.isLoading = true;
                        $scope.userOwerName = userResponse.userName;
                        localServices.getUserRepos().then(function (response) {
                            $scope.allRepos = response.data.repos;
                        });
                    });

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
            getFiles: function (userName, repoName) {
                return $http.get(ghBase + 'repos/' + repoName + '/git/trees/master?recursive=1');
            }
        }
    }]);
})();