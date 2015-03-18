erbsenzaehlerControllers.controller('linesEditorController', function ($scope, linesEditorResource) {
    $scope.loadLines = function () {
        var selectedDate = getQuerystring("month");
        if ($scope.viewModel && $scope.viewModel.SelectedDate)
            selectedDate = $scope.viewModel.SelectedDate;

        $scope.loading = true;
        $scope.viewModel = linesEditorResource.query({ month: selectedDate }, function () {
            $scope.loading = false;
        });
    };



    $scope.addLine = function () {
        alert("add");

        if (window.reloadCallback) {
            window.reloadCallback();
        }
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
            linesEditorResource.delete({ id: line.Id, month: getQuerystring("month") }, function () {
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
        $scope.save(line);
    };

    $scope.loadLines();
}
);