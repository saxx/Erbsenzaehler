erbsenzaehlerControllers.controller('linesEditorController', [
    '$scope', 'linesEditorResource', function ($scope, linesEditorResource) {
        $scope.loadLines = function () {
            var selectedDate = getQuerystring("month");
            if ($scope.viewModel && $scope.viewModel.SelectedDate)
                selectedDate = $scope.viewModel.SelectedDate;

            $scope.loading = true;
            $scope.viewModel = linesEditorResource.query({ month: selectedDate }, function () {
                $scope.loading = false;
            });
        };

        $scope.save = function (line) {
            linesEditorResource.update(line, function () {
                if (window.reloadCallback) {
                    window.reloadCallback();
                }
            });
        };

        $scope.delete = function (line) {
            if (confirm('Are you sure, you want to delete this account statement? This cannot be undone!')) {
                alert(line);

                line.$delete(function () {
                    var index = $scope.viewModel.indexOf(line);
                    $scope.viewModel.splice(index, 1);
                });
            }
        };

        $scope.switchIgnore = function (line) {
            line.Ignore = !line.Ignore;
            $scope.save(line);
        };

        $scope.loadLines();
    }
]);