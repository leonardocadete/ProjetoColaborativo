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
        var atualizaElementos = $.connection.atualizaElementos;

        atualizaElementos.client.atualizar = function() {
            console.log("chamou");
        };

        $.connection.hub.start();
    };
})();