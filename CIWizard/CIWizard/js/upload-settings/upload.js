/* global angular */
(function () {
    "use strict";
    var module = angular.module('upload-settings',[
        'local-services'
    ]);

    module.directive('uploadSettings', ['localServices','fileUpload', function (localServices,fileUpload) {
        return {
            restrict: 'E',
            scope: {
                ownerName: '=',
                repoName: '=',
                isDisabled: '='
            },
            templateUrl: 'js/upload-settings/upload.html',
            controller: function ($scope, $element, $attrs) {
                $scope.getFiles = function () {
                    localServices.getProjectFiles($scope.ownerName,$scope.repoName).then(function (response) {
                        for(var i = 0; i < response.data.fileNames.length;i++) {
                            var file = response.data.fileNames[i];
                            if(file === "appsettings.txt")
                                $scope.hasAppSettings = true;
                        }
                        $scope.files = response.data.fileNames;
                        console.log($scope.files);
                    });
                };

                $scope.deleteFile = function (fileName) {
                    localServices.deleteProjectFile($scope.ownerName,$scope.repoName,fileName).then(function (response) {
                        $scope.getFiles();
                    }, function (response) {
                        //Handle error
                    });
                };

                $scope.uploadFile = function () {
                    var file = $scope.appSettingsFile;
                    console.log('file is ' + JSON.stringify(file));
                    var uploadUrl = '/user/projects/' + $scope.ownerName + '/' + $scope.repoName + '/settings';
                    fileUpload.uploadFileToUrl(file, uploadUrl);
                };

                $scope.getFiles();
            }
        }
    }])
})();