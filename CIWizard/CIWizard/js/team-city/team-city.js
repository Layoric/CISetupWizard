/* global angular */
(function () {
    "use strict";
    var module = angular.module('team-city', [
        'local-services'
    ]);

    module.directive('teamCityCurrentConfigs', ['localServices',function (localServices) {
        return {
            restrict: 'A',
            scope: {

            },
            templateUrl: '',
            controller: function ($scope, $element, $attrs) {
                localServices.getTeamCityConfigs().then(function (response) {
                    $scope.teamCityConfigs = response.data.configs;
                });

            }
        }
    }]);
})();