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
            },
            templateUrl: '/js/team-city/team-city-configs.html',
            controller: function ($scope, $element, $attrs) {
                $scope.teamCityConfigs = [];
                var date = new Date();
                //DUMMY DATA
                var displayDate = date.getFullYear() + '/' +  (date.getMonth() + 1) + '/' + date.getDate() + ' ' + date.getHours() + ':' + date.getMinutes();
                $timeout(function () {
                    $scope.teamCityConfigs.push({
                        name: 'ServiceStackAngularUploadExample',
                        status: 'Deployed',
                        lastUpdated: displayDate
                    });
                });
                //Get data from services
                //localServices.getTeamCityConfigs().then(function (response) {
                //    $scope.teamCityConfigs = response.data.configs;
                //});
            },
            link: function(scope) {

            }
        };
    }]);
})();