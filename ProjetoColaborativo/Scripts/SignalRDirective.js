(function () {
    "use strict";

    var app = angular.module('ColaborativoApp', []);

    app.directive("atualizaElementos", function() {
        return{
            restrict: "A",
            controller: atualizaElementosCtrl,
            controllerAs: "ctrl"
        };
    });

    var atualizaElementosCtrl = function () {
        alert("olá");
    };
})();