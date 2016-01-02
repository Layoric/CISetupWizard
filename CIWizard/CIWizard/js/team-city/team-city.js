/* global angular */
(function () {
    "use strict";
    var module = angular.module('team-city', [
        'local-services'
    ]);

    module.directive('teamcityConfigs', ['$timeout','localServices',function ($timeout, localServices) {
        return {
            restrict: 'E',
            scope: {
                teamCityConfigs: '='
            },
            templateUrl: '/js/team-city/team-city-configs.html',
            controller: function ($scope, $element, $attrs) {
                //Get data from services
                localServices.getTeamCityProjects().then(function (response) {
                    $scope.teamCityConfigs = response.data.projects;
                });
            },
            link: function(scope) {

            }
        };
    }]);
})();