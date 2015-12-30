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

                    authentication.getUserDetails().then(function (response) {
                        $scope.isLoading = true;
                        var orgs = [];
                        localServices.getUserRepos().then(function (response) {
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
                            $scope.selectedOrgs = angular.copy(orgs);
                        });
                    });

                    $scope.filterBySelectedOrg = function (orgName) {
                      for(var i = 0; i < $scope.allOrgs.length; i++) {
                          var org = $scope.allOrgs[i];
                          if(org.orgName === orgName) {

                          }
                      }
                    };

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