(function () {
    "use strict";

    var app = angular.module('ColaborativoApp', []);

    app.directive("atualizaElementos", function() {
        return{
            restrict: "A",
            controller: atualizaElementosCtrl
        };
    });

    var atualizaElementosCtrl = function () {
        var atualizaElementos = $.connection.atualizaElementos;

        atualizaElementos.client.atualizar = atualizacao;

        $.connection.hub.start();
    };

    var atualizacao = function() {
        console.log("chamou");
    };
})();