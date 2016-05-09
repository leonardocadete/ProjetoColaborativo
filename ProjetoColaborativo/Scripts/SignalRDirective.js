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

        $.getJSON( window.location.pathname.replace("MostrarSessaoIframe", "BuscarElementosDosOutrosParticipantesJson"), function( data ) {
            
            // sorting
            data.objects.forEach(function(d) {
                $("li[data-id='" + d.id + "']").attr("data-order", d.order);
            });

            var li = $("#sortable li");
            li.detach().sort(function(a, b) {
                return parseInt($(a).attr("data-order")) > parseInt($(b).attr("data-order"));
            });
            $("#sortable").append(li);
            
            canvas1.forEachObject(function(d) {
                if (d.iddono != dono) {
                    d.naosalvar = true;
                    d.remove();
                }
            });

            fabric.util.enlivenObjects(data.multimediaelements, function(objects) {
                var origRenderOnAddRemove = canvas1.renderOnAddRemove;
                canvas1.renderOnAddRemove = false;
                objects.forEach(function(o) {
                    o.naosalvar = true;
                    //o.selectable = false;
                    o.lockRotation = true;
                    o.lockScalingX = o.lockScalingY = true;
                    o.lockMovementX = o.lockMovementY = true;
                    canvas1.add(o);
                });
                canvas1.renderOnAddRemove = origRenderOnAddRemove;
                canvas1.renderAll();

                atualizarMiniatura(false);
            });

        });

    };
})();