/* global angular */
(function () {
    "use strict";
    var module = angular.module('manage-repo', [
        'local-services'
    ]);

    module.controller('managerRepoCtrl', ['$scope','$routeParams','localServices',
        function ($scope,$routeParams,localServices) {
        //Get repo
            localServices.getUserRepo($routeParams.repoName).then(function (response) {
            $scope.repo = response.data.repo;
        });
    }]);
})();