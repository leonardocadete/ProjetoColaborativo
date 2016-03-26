// from http://177.131.33.18:88/javascripts/globais.js


function copiarCanvas(canvas, callback) {

    var sourceCtx, destinationCtx, imageData;
    var sourceCanvas = $(canvas)[0];
    var destinationCanvas = document.createElement("canvas");

    sourceCtx = sourceCanvas.getContext('webgl', { preserveDrawingBuffer: true });

    destinationCanvas.id = "tempCanvas";
    destinationCanvas.width = sourceCanvas.width;
    destinationCanvas.height = sourceCanvas.height;
    destinationCtx = destinationCanvas.getContext('2d', { preserveDrawingBuffer: true });

    var image, destinationCtx;

    image = new Image();
    image.src = sourceCanvas.toDataURL('image/jpeg');
    image.onload = function () {
        destinationCtx.drawImage(image, 0, 0);
        return callback(destinationCanvas);
    }
}


function copiarElementosCesium() {

    var data = $("body");
    $(data).css("background", "transparent");
    var svgElements = $(".app-container svg");//.find('svg');		

    svgElements.each(function () {

        var canvas, xml;

        canvas = document.createElement("canvas");
        var ctx = canvas.getContext('2d');

        $(this).css("fill", "#FFF");

        canvas.className = "screenShotTempCanvas";
        //convert SVG into a XML string
        xml = (new XMLSerializer()).serializeToString(this);

        // Removing the name space as IE throws an error
        xml = xml.replace(/xmlns=\"http:\/\/www\.w3\.org\/2000\/svg\"/, '');

        //draw the SVG onto a canvas
        canvg(canvas, xml, { useCORS: true });

        if ($(this).parent().hasClass("compass-outer-ring") || $(this).parent().hasClass("compass-gyro")) {

            $(canvas).width('95px');
            $(canvas).height('95px');

        }

        if ($(this).parent().hasClass("navigation-control-icon-reset")) {
            $(canvas).css({ "margin-bottom": "5px", "width": "10px", "height": "10px" });
        }

        //hide the SVG element
        $(canvas).insertAfter(this);
        this.className = "tempHide";
    });

    return data;
}

function copiarElementosRV() {
    var data = $("#rvcontainer");
    return data;
}

salvarImagemColaborativo = function (callback) {

    var destinationCanvas, data;
    var isRV = $("#rvcontainer").hasClass("show");

    //rv
    if (isRV) {
        destinationCanvas = copiarCanvas("#rvcena", function (canvasDestino) {
            data = copiarElementosRV();
            salvar(data, canvasDestino);
        });

    } else {
        destinationCanvas = copiarCanvas(".cesium-widget canvas", function (canvasDestino) {
            data = copiarElementosCesium();
            salvar(data, canvasDestino);
        });
    }

    function salvar(dados, destino) {

        html2canvas(dados, { canvas: destino, allowTaint: true, useCORS: true }).then(function (canvas) {

            var data = canvas.toDataURL();
            callback(data);

        });
    }
}