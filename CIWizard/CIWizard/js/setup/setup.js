/* global angular */
(function () {
    "use strict";
    var module = angular.module('setup', [

    ]);

    module.directive('appSetup', ['$http',function($http) {
        return {
            restrict: 'E',
            scope: {
            },
            templateUrl: 'js/setup/setup.html',
            controller: function ($scope, $element, $attrs) {
                // Collect local user + password to store in _Root project in TeamCity securely, NOT persisted in app.
                // This will make it easy to add locally deployed apps whilst still given the option to deploy elsewhere

                $scope.msDeployUserAlreadyConfigured = false;

                // Get _Root project params to check if `ss.msdeploy.username` has a value.
                //$http.get('')
            }
        }
    }]);
})();