/* global angular */
(function () {
    "use strict";
    // Declare app level module which depends on filters, and services
    var module = angular.module('helloApp', [
        'ngRoute',
        'navigation.controllers',
        'home',
        'authentication'
    ]);

    module.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
        $routeProvider.when('/', { templateUrl: '/js/home/home.html', controller: 'homeCtrl' });
        $routeProvider.when('/auth/:any', { controller: function () { location.href = location.href; }, template: '<div class="github-passthrough">Passing you to GitHub now..</div>' });
        $routeProvider.otherwise({ redirectTo: '/' });

        $locationProvider.html5Mode(true);
    }]);
})();

function endsWith(str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
}