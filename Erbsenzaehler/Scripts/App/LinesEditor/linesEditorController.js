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

        $scope.switchIgnore = function (line) {
            line.Ignore = !line.Ignore;
            resource.update(line);
        };

        $scope.changeDate = function (line, event) {
            line.Date = $(event.target).html();
            resource.update(line);
        };

        $scope.changeCategory = function (line, event) {
            line.Category = $(event.target).html();
            resource.update(line);
        };

        $scope.changeAmount = function (line, event) {
            line.Amount = $(event.target).html();
            resource.update(line);
        };

        $scope.loadLines();
    }
]);