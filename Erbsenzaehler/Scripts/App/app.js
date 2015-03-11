var erbsenzaehlerApp = angular.module('erbsenzaehlerApp', ['erbsenzaehlerControllers', 'erbsenzaehlerServices', 'erbsenzaehlerAnimations']);

var erbsenzaehlerAnimations = angular.module('erbsenzaehlerAnimations', ['ngAnimate']);
var erbsenzaehlerServices = angular.module('erbsenzaehlerServices', ['ngResource']);
var erbsenzaehlerControllers = angular.module('erbsenzaehlerControllers', []);

erbsenzaehlerApp.directive('nobreaks', function () {
    return {
        restrict: 'A',
        link: function (scope, elm, attrs, ctrl) {
            elm.on('keydown', function (event) {
                if (event.which != 13) {
                    return true;
                } else {
                    event.preventDefault();
                    return false;
                }
            });
        }
    }
});

erbsenzaehlerApp.directive('nospaces', function () {
    return {
        restrict: 'A',
        link: function (scope, elm, attrs, ctrl) {
            elm.on('keydown', function (event) {
                if (event.which != 32) {
                    return true;
                } else {
                    event.preventDefault();
                    return false;
                }
            });
        }
    }
});

erbsenzaehlerApp.directive('nochars', function () {
    return {
        restrict: 'A',
        link: function (scope, elm, attrs, ctrl) {
            elm.on('keydown', function (event) {
                if (event.which < 64 || event.which > 122) {
                    return true;
                } else {
                    event.preventDefault();
                    return false;
                }
            });
        }
    }
});

erbsenzaehlerApp.directive("contenteditable", function () {
    return {
        require: "?ngModel",
        link: function (scope, element, attrs, ngModel) {

            if (!ngModel) {
                return;
            }

            function read() {
                ngModel.$setViewValue(element.html());
            }

            ngModel.$render = function () {
                element.html(ngModel.$viewValue || "");
            };

            element.bind("blur keyup change", function () {
                scope.$apply(read);
            });
        }
    };
});