var erbsenzaehlerApp = angular.module('erbsenzaehlerApp', ['erbsenzaehlerControllers', 'erbsenzaehlerServices', 'erbsenzaehlerAnimations']);

var erbsenzaehlerAnimations = angular.module('erbsenzaehlerAnimations', ['ngAnimate']);
var erbsenzaehlerServices = angular.module('erbsenzaehlerServices', ['ngResource']);
var erbsenzaehlerControllers = angular.module('erbsenzaehlerControllers', []);

$(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

erbsenzaehlerApp.directive('title', function () {
    return {
        restrict: 'A',
        link: function (scope, element) {
            $(element).hover(function () {
                // on mouseenter
                $(element).tooltip('show');
            }, function () {
                // on mouseleave
                $(element).tooltip('hide');
            });
        }
    };
});

erbsenzaehlerApp.directive('nobreaks', function () {
    return {
        restrict: 'A',
        link: function (scope, elm) {
            elm.on('keydown', function (event) {
                if (event.which !== 13) {
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
        link: function (scope, elm) {
            elm.on('keydown', function (event) {
                if (event.which !== 32) {
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
        link: function (scope, elm) {
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

erbsenzaehlerApp.directive('contenteditable', function () {
    return {
        restrict: 'A',
        require: '?ngModel',
        link: function (scope, element, attrs, ngModel) {
            if (!ngModel) {
                return;
            }

            function read() {
                var html = element.text();

                if (html === '') {
                    html = null;
                }

                ngModel.$setViewValue(html);
            }

            ngModel.$render = function () {
                element.html(ngModel.$viewValue || "");
            };

            element.bind('DOMCharacterDataModified keyup', function () {
                scope.$apply(read);
            });
        }
    };
});