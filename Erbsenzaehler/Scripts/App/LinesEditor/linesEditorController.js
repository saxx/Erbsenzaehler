erbsenzaehlerControllers.controller('linesEditorController', [
    '$scope', 'linesEditorResource', function ($scope, resource) {
        $scope.loadLines = function () {
            var selectedDate = getQuerystring("month");
            if ($scope.viewModel && $scope.viewModel.SelectedDate)
                selectedDate = $scope.viewModel.SelectedDate;

            $scope.loading = true;
            $scope.viewModel = resource.query({ month: selectedDate }, function () {
                $scope.loading = false;
            });
        };

        $scope.save = function (line) {
            resource.update(line);
        };

        $scope.switchIgnore = function (line) {
            line.Ignore = !line.Ignore;
            resource.update(line);
        };

        $scope.loadLines();
    }
]);