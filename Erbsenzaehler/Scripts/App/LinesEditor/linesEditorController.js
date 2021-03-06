﻿erbsenzaehlerControllers.controller('linesEditorController', [
    '$scope', 'linesEditorResource',
    function ($scope, linesEditorResource) {

        $scope.loadLines = function () {
            $scope.loading = true;
            $scope.viewModel = linesEditorResource.query({ month: getQuerystring("month") }, function () {
                $scope.loading = false;

                if (window.reloadCallback) {
                    window.reloadCallback();
                }

            });
        };

        $scope.addLine = function () {
            linesEditorResource.create({ month: getQuerystring("month") }, function () {
                $scope.loadLines();
            });
        };

        $scope.saveLine = function (line) {
            linesEditorResource.update(line, function () {
                if (window.reloadCallback) {
                    window.reloadCallback();
                }
            });
        };

        $scope.deleteLine = function (line) {
            if (confirm('Are you sure, you want to delete this account statement? This cannot be undone!')) {
                linesEditorResource.delete({ id: line.Id }, function () {
                    var index = $scope.viewModel.Lines.indexOf(line);
                    $scope.viewModel.Lines.splice(index, 1);

                    if (window.reloadCallback) {
                        window.reloadCallback();
                    }
                });
            }
        };

        $scope.switchIgnore = function (line) {
            line.Ignore = !line.Ignore;
            $scope.saveLine(line);
        };

        $scope.loadLines();
    }
]);