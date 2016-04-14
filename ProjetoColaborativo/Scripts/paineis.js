// canvas stuff

var canvas1 = new fabric.Canvas('draw-canvas');

window.addEventListener('resize', resizeCanvas, false);

var customProperties = 'id iddono collabtype mediasent'.split(' ');

function getThumbnail(original, scale) {
    var canvas = document.createElement("canvas");

    canvas.width = original.width * scale;
    canvas.height = original.height * scale;

    canvas.getContext("2d").drawImage(original, 0, 0, canvas.width, canvas.height);

    return canvas;
}

function atualizarMiniatura(salvar) {
    $("li.selecionado img").attr("src", canvas1.toDataURL({
        format: 'jpeg',
        quality: 0.1
    }));

    if (salvar)
        SaveThumbnail();
}

function hexToRgb(hex) {
    var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return result ? {
        r: parseInt(result[1], 16),
        g: parseInt(result[2], 16),
        b: parseInt(result[3], 16)
    } : null;
}

var escrevendo = false;
canvas1.on('text:editing:entered', function (e) {
    escrevendo = true;
});

canvas1.on('text:editing:exited', function (e) {
    escrevendo = false;
    canvas1.off('mouse:up');

    if (e.target.text == "") {
        e.target.remove();
        return;
    }

    SaveObject(e.target, false);
});

canvas1.on('object:modified', function (e) {
    SaveObject(e.target, false);
});

canvas1.on('object:removed', function (e) {

    if(e.target.collabtype == 'sound')
        $("#audio-record-toolbar").fadeOut();

    SaveObject(e.target, true);
});

canvas1.on('path:created', function (e) {
    var id = uuid.v4();
    e.path.id = id;
    e.path.iddono = dono;
    SaveObject(e.path, false);
});

var audio, video;
canvas1.on('object:selected', function (e) {

    var o = canvas1.getActiveObject();

    if (o && o.collabtype) {

        switch (o.collabtype) {
            case 'sound':
            case 'video':
                showMediaPlayer(o);
        }
    }

});

canvas1.on('selection:cleared', function () {
    if (audio) {
        audio.pause();
    }

    $("#media-player-toolbar").fadeOut();
});

function getDadosUsuario(id, callback) {

    $.ajax({
        type: "GET",
        url: window.location.pathname.replace("MostrarSessao", "GetDadosUsuario"),
        data: { idusuario: id },
        success: function (data) {
            callback(data);
        }
    });

}

var c, f, b;
canvas1.on('mouse:over', function (e) {
    c = e.target.stroke;
    e.target.stroke = c.replace("0.5", "1");

    if (e.target.fill) {
        f = e.target.fill;
        e.target.fill = f.replace("0.5", "1");
    }

    if (e.target.textBackgroundColor) {
        b = e.target.textBackgroundColor;
        e.target.textBackgroundColor = b.replace("0.5", "1");
    }
    
    getDadosUsuario(e.target.iddono, function (data) {
        $("#elemento-tooltip-txt").html(data.Nome);
        $("#elemento-tooltip").css("top", e.target.top);
        $("#elemento-tooltip").css("left", e.target.left);
        $("#elemento-tooltip").show();
    });

    canvas1.renderAll();
});

canvas1.on('mouse:out', function (e) {
    if (e.target.fill) {
        e.target.fill = f;
    }

    if (e.target.textBackgroundColor) {
        e.target.textBackgroundColor = b;
    }
    e.target.stroke = c;

    $("#elemento-tooltip").hide();

    canvas1.renderAll();
});

function SaveObject(target, remover) {
    
    if (target.naosalvar)
        return;
    
    if (audiostream)
        audiostream.stop();

    atualizarMiniatura(true);

    $.ajax({
        type: "POST",
        url: "",
        data: "{ 'guid' : '" + target.id + "', 'remover' : '" + remover + "', 'json' : '" + JSON.stringify(target.toJSON(customProperties)) + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) { console.log(data) },
        failure: function (errMsg) {
            alert(errMsg);
        }
    });
}

function SaveThumbnail() {

    var image = canvas1.toDataURL({
        format: 'jpeg',
        quality: 0.1
    });

    $.ajax({
        type: "POST",
        url: window.location.pathname.replace("MostrarSessao", "SalvarMiniatura"),
        data: { imgdata: image },
        success: function (data) {
            console.log(data);
        },
        failure: function (errMsg) {
            alert(errMsg);
        }
    });

}

function resizeCanvas() {
    canvas1.setHeight(window.innerHeight);
    canvas1.setWidth(window.innerWidth);
    canvas1.calcOffset();
    canvas1.renderAll();
    $(".canvas-container").css("height", ""); // to show side panels
}
resizeCanvas();

function setCanvasBackground(url) {
    canvas1.setBackgroundImage(url, canvas1.renderAll.bind(canvas1), {
        // width: canvas1.width,
        // height: canvas1.height,
        // originX: 'left',
        // originY: 'top'
    });
}



/**
 DRAWING 
 */

var draw = false;
var drawingobject;

/**
 * SQUARE
 */
$("input[type='button'].icon-rect").click(function () {

    canvas1.off('mouse:move');
    canvas1.off('mouse:down');
    canvas1.off('mouse:up');

    draw = true;
    var drawingobject;

    canvas1.on('mouse:down', function (option) {

        if (!draw) return false;

        if (typeof option.target != "undefined") {
            return false;
        } else {
            var startY = option.e.offsetY,
                startX = option.e.offsetX,
                id = uuid.v4();

            drawingobject = new fabric.Rect({
                top: startY,
                left: startX,
                originX: 'left',
                originY: 'top',
                width: 0,
                height: 0,
                fill: "rgba(" + hexToRgb(cordono).r + ", " + hexToRgb(cordono).g + ", " + hexToRgb(cordono).b + ", 0.5)",
                stroke: '',
                strokewidth: 0,
                id: id,
                iddono: dono
            });

            canvas1.add(drawingobject);

            canvas1.on('mouse:move', function (option) {
                var pointer = canvas1.getPointer(option.e);

                if (startX > pointer.x) {
                    drawingobject.set({ left: Math.abs(pointer.x) });
                }
                if (startY > pointer.y) {
                    drawingobject.set({ top: Math.abs(pointer.y) });
                }

                drawingobject.set({ width: Math.abs(startX - pointer.x) });
                drawingobject.set({ height: Math.abs(startY - pointer.y) });
                drawingobject.setCoords();
                canvas1.renderAll();
            });
        }
    });

    canvas1.on('mouse:up', function () {
        draw = false;
        canvas1.off('mouse:move');
        canvas1.off('mouse:down');
        canvas1.off('mouse:up');
        SaveObject(drawingobject, false);
    });

});


/**
 * ELIPSE
 */
$("input[type='button'].icon-elipse").click(function () {

    canvas1.off('mouse:move');
    canvas1.off('mouse:down');
    canvas1.off('mouse:up');

    draw = true;
    var drawingobject;

    canvas1.on('mouse:down', function (option) {

        if (!draw) return false;

        if (typeof option.target != "undefined") {
            return;
        } else {
            var startY = option.e.offsetY,
                startX = option.e.offsetX,
                id = uuid.v4();

            var pointer = canvas1.getPointer(option.e);

            drawingobject = new fabric.Ellipse({
                top: startY,
                left: startX,
                originX: 'left',
                originY: 'top',
                rx: pointer.x - startX,
                ry: pointer.y - startY,
                angle: 0,
                fill: "rgba(" + hexToRgb(cordono).r + ", " + hexToRgb(cordono).g + ", " + hexToRgb(cordono).b + ", 0.5)",
                stroke: '',
                strokewidth: 0,
                id: id,
                iddono: dono
            });

            canvas1.add(drawingobject);

            canvas1.on('mouse:move', function (o) {
                var pointer = canvas1.getPointer(o.e);
                var rx = Math.abs(startX - pointer.x) / 2;
                var ry = Math.abs(startY - pointer.y) / 2;
                if (rx > drawingobject.strokeWidth) {
                    rx -= drawingobject.strokeWidth / 2;
                }
                if (ry > drawingobject.strokeWidth) {
                    ry -= drawingobject.strokeWidth / 2;
                }
                drawingobject.set({ rx: rx, ry: ry });

                if (startX > pointer.x) {
                    drawingobject.set({ originX: 'right' });
                } else {
                    drawingobject.set({ originX: 'left' });
                }
                if (startY > pointer.y) {
                    drawingobject.set({ originY: 'bottom' });
                } else {
                    drawingobject.set({ originY: 'top' });
                }
                drawingobject.setCoords();
                canvas1.renderAll();
            });
        }
    });

    canvas1.on('mouse:up', function () {
        draw = false;
        canvas1.off('mouse:move');
        canvas1.off('mouse:down');
        canvas1.off('mouse:up');
        SaveObject(drawingobject, false);
    });

});


/**
 * PENCIL
 */
$("input[type='button'].icon-pencil").click(function () {

    canvas1.off('mouse:move');
    canvas1.off('mouse:down');
    canvas1.off('mouse:up');

    canvas1.isDrawingMode = true;

    canvas1.freeDrawingBrush.color = "rgba(" + hexToRgb(cordono).r + ", " + hexToRgb(cordono).g + ", " + hexToRgb(cordono).b + ", 0.5)";
    canvas1.freeDrawingBrush.width = 12;

    canvas1.on('mouse:up', function () {
        canvas1.isDrawingMode = false;
    });
});

/**
 * PIN
 */
var pinpath = "m138.34106,102.24695c0,4.62839 -3.74586,8.38078 -8.36738,8.38078c-4.615,0 -8.36738,-3.75239 -8.36738,-8.38078c0,-4.615 3.74586,-8.36051 8.36738,-8.36051c4.62152,0 8.36738,3.74552 8.36738,8.36051zm15.96285,-0.00309c0,3.01071 -0.5706,5.87369 -1.56753,8.53194c-3.18865,9.47149 -24.30001,58.1622 -24.30001,58.1622s-19.87636,-53.08756 -21.60157,-59.17596c-0.76848,-2.35937 -1.1948,-4.87677 -1.1948,-7.51165c0,-13.4221 10.89474,-24.3268 24.33367,-24.3268c13.43859,-0.00344 24.33333,10.90127 24.33024,24.32028zm-8.8071,0.09722c0,-8.57213 -6.94414,-15.52657 -15.52313,-15.52657c-8.57213,0 -15.5197,6.95101 -15.5197,15.52313s6.94757,15.52313 15.5197,15.52313c8.57866,0 15.52313,-6.94757 15.52313,-15.5197z";
$("input[type='button'].icon-pin").click(function () {

    canvas1.off('mouse:move');
    canvas1.off('mouse:down');
    canvas1.off('mouse:up');

    draw = true;
    var drawingobject;

    canvas1.on('mouse:down', function (option) {

        if (!draw) return false;

        if (typeof option.target != "undefined") {
            return;
        } else {
            var startY = option.e.offsetY,
                startX = option.e.offsetX,
                id = uuid.v4();

            drawingobject = new fabric.Path(pinpath, {
                originX: 'center',
                originY: 'bottom',
                top: startY,
                left: startX,
                width: 141,
                height: 164,
                fill: "rgba(" + hexToRgb(cordono).r + ", " + hexToRgb(cordono).g + ", " + hexToRgb(cordono).b + ", 0.5)",
                stroke: '',
                strokewidth: 0,
                id: id,
                iddono: dono
            });

            canvas1.add(drawingobject);
        }
    });

    canvas1.on('mouse:up', function () {
        draw = false;
        canvas1.off('mouse:move');
        canvas1.off('mouse:down');
        canvas1.off('mouse:up');
        SaveObject(drawingobject, false);
    });

});


/**
 * SPEAKER
 */
var speakerpath = "m112.337,85.047l-3.333,4.992c6.093,4.95 9.996,12.498 9.996,20.961c0,8.466 -3.903,16.014 -9.996,20.964l3.333,4.989c7.698,-6.036 12.663,-15.405 12.663,-25.953c0,-10.545 -4.962,-19.914 -12.663,-25.953zm-6.687,10.014l-3.366,5.04c2.904,2.739 4.74,6.594 4.74,10.902s-1.836,8.166 -4.74,10.905l3.366,5.04c4.491,-3.858 7.35,-9.564 7.35,-15.948c0,-6.381 -2.856,-12.087 -7.35,-15.939zm-16.65,-17.061c-3.531,0 -4.599,2.052 -4.599,2.052s-9.183,10.089 -15.507,14.541c-1.164,0.741 -2.442,1.407 -4.704,1.407l-5.19,0c-3.312,0 -6,2.688 -6,6l0,18c0,3.312 2.688,6 6,6l5.19,0c2.262,0 3.54,0.666 4.701,1.407c6.324,4.452 15.507,14.544 15.507,14.544s1.071,2.049 4.602,2.049c3.312,0 6,-2.685 6,-6l0,-54c0,-3.315 -2.688,-6 -6,-6z";
$("input[type='button'].icon-speaker").click(function () {

    resetAudioRecordTools();

    canvas1.off('mouse:move');
    canvas1.off('mouse:down');
    canvas1.off('mouse:up');

    draw = true;
    var drawingobject;

    canvas1.on('mouse:down', function (option) {

        if (!draw) return false;

        if (typeof option.target != "undefined") {
            return;
        } else {

            var startY = option.e.offsetY,
                startX = option.e.offsetX,
                id = uuid.v4();

            $("#audio-record-toolbar input[type='button'].icon-ok").attr("data-objectid", id);

            drawingobject = new fabric.Path(speakerpath, {
                originX: 'center',
                originY: 'bottom',
                top: startY,
                left: startX,
                width: 141,
                height: 164,
                fill: "rgba(" + hexToRgb(cordono).r + ", " + hexToRgb(cordono).g + ", " + hexToRgb(cordono).b + ", 0.5)",
                stroke: '',
                strokewidth: 0,
                id: id,
                iddono: dono,
                collabtype: 'sound',
                naosalvar: true
            });

            canvas1.add(drawingobject);
            initAudio(function () {
                $("#audio-record-toolbar").css("top", startY);
                $("#audio-record-toolbar").css("left", startX);
                $("#audio-record-toolbar").fadeIn();
            });
        }
    });

    canvas1.on('mouse:up', function () {
        draw = false;
        canvas1.off('mouse:move');
        canvas1.off('mouse:down');
        canvas1.off('mouse:up');
        SaveObject(drawingobject, false);
    });


    // record controls
    $("#audio-record-toolbar input[type='button'].icon-record").off("click");
    $("#audio-record-toolbar input[type='button'].icon-record").click(function () {
        toggleRecording(this);
        if ($(this).hasClass("recording")) {
            $("#audio-record-toolbar input[type='button'].icon-play").attr("disabled", "disabled");
            $("#audio-record-toolbar input[type='button'].icon-ok").attr("disabled", "disabled");
        } else {
            $("#audio-record-toolbar input[type='button'].icon-ok").removeAttr("disabled");
            $("#audio-record-toolbar input[type='button'].icon-play").removeAttr("disabled");
        }
    });

    $("#audio-record-toolbar input[type='button'].icon-delete").off("click");
    $("#audio-record-toolbar input[type='button'].icon-delete").click(function () {
        canvas1.remove(drawingobject);
        if (audiostream)
            audiostream.stop();
        $("#audio-record-toolbar").fadeOut();
    });

});

function resetAudioRecordTools() {
    $("#audio-record-toolbar input[type='button'].icon-record").removeClass("recording");
    $("#audio-record-toolbar input[type='button'].icon-record").removeAttr("disabled");
    $("#audio-record-toolbar input[type='button'].icon-play").attr("disabled", "disabled");
    $("#audio-record-toolbar input[type='button'].icon-ok").attr("disabled", "disabled");
}

/**
 * VIDEO
 */
var deg;
var youtubepath = "m79.73329,4.69492c-3.19192,-3.56183 -6.7671,-3.58296 -8.40627,-3.78473c-11.74272,-0.91019 -29.3554,-0.91019 -29.3554,-0.91019l-0.03946,0c0,0 -17.61296,0 -29.3529,0.91019c-1.63918,0.20205 -5.21157,0.22289 -8.40627,3.78473c-2.51268,2.71723 -3.33366,8.88902 -3.33366,8.88902s-0.83932,7.24179 -0.83932,14.48385l0,6.78794c0,7.24707 0.83932,14.48635 0.83932,14.48635s0.81848,6.16901 3.33366,8.88096c3.1947,3.56183 7.38325,3.45428 9.25088,3.82669c6.71207,0.68758 28.52914,0.89991 28.52914,0.89991s17.6313,-0.03141 29.37374,-0.93104c1.63918,-0.20983 5.21435,-0.23067 8.40627,-3.79279c2.51268,-2.71195 3.33366,-8.88096 3.33366,-8.88096s0.83932,-7.24179 0.83932,-14.48635l0,-6.78822c0,-7.24429 -0.83932,-14.48635 -0.83932,-14.48635s-0.81792,-6.17151 -3.33338,-8.88902zm-48.25875,42.51746l0,-31.47482l26.22907,15.73755l-26.22907,15.73727z";
$("input[type='button'].icon-youtube").click(function () {

    resetVideoRecordTools();

    canvas1.off('mouse:move');
    canvas1.off('mouse:down');
    canvas1.off('mouse:up');

    draw = true;
    var drawingobject;

    canvas1.on('mouse:down', function (option) {

        if (!draw) return false;

        if (typeof option.target != "undefined") {
            return;
        } else {

            var startY = option.e.offsetY,
                startX = option.e.offsetX,
                id = uuid.v4();

            $("#video-record-toolbar input[type='button'].icon-ok").attr("data-objectid", id);

            drawingobject = new fabric.Path(youtubepath, {
                originX: 'center',
                originY: 'center',
                top: startY,
                left: startX,
                width: 141,
                height: 164,
                fill: "rgba(" + hexToRgb(cordono).r + ", " + hexToRgb(cordono).g + ", " + hexToRgb(cordono).b + ", 0.5)",
                stroke: '',
                strokewidth: 0,
                id: id,
                iddono: dono,
                collabtype: 'video',
                naosalvar: true
            });
            deg = drawingobject;
            canvas1.add(drawingobject);

            captureAudioPlusVideo(commonConfig);
            $("#video-record-toolbar").css("top", startY);
            $("#video-record-toolbar").css("left", startX);
            $("#video-record-toolbar").fadeIn();
        }
    });

    canvas1.on('mouse:up', function () {
        draw = false;
        canvas1.off('mouse:move');
        canvas1.off('mouse:down');
        canvas1.off('mouse:up');
        SaveObject(drawingobject, false);
    });
    
    // record controls
    $("#video-record-toolbar input[type='button'].icon-record").off("click");
    $("#video-record-toolbar input[type='button'].icon-record").click(function () {

        startRecording(this);

        if ($(this).hasClass("recording")) {
            $("#video-record-toolbar input[type='button'].icon-play").attr("disabled", "disabled");
            $("#video-record-toolbar input[type='button'].icon-ok").attr("disabled", "disabled");
        } else {
            $("#video-record-toolbar input[type='button'].icon-ok").removeAttr("disabled");
            $("#video-record-toolbar input[type='button'].icon-play").removeAttr("disabled");
        }

    });

    $("#video-record-toolbar input[type='button'].icon-delete").off("click");
    $("#video-record-toolbar input[type='button'].icon-delete").click(function () {
        if (stream) {
            stream.stop();
            stream = null;
        }
        canvas1.remove(drawingobject);
        $("#video-record-toolbar").fadeOut();
    });

    $("#video-record-toolbar input[type='button'].icon-play").off("click");
    $("#video-record-toolbar input[type='button'].icon-play").click(function () {
        recordingPlayer.play();
    });

    $("#video-record-toolbar input[type='button'].icon-ok").off("click");
    $("#video-record-toolbar input[type='button'].icon-ok").click(function () {

        var id = $("#video-record-toolbar input[type='button'].icon-ok").attr("data-objectid");

        var fd = new FormData();
        fd.append('file', recordRTC.blob);
        fd.append('objectid', id);
        $.ajax({
            type: 'POST',
            url: '/Vimaps/SendVideo',
            data: fd,
            processData: false,
            contentType: false
        }).done(function (data) {
            canvas1.forEachObject(function (d) {
                if (d.id == id) {
                    d.mediasent = true;
                    d.naosalvar = false;
                    SaveObject(d, false);
                }
            });
        });

        $("#video-record-toolbar input[type='button'].icon-play").attr("disabled", "disabled");
        $("#video-record-toolbar input[type='button'].icon-ok").attr("disabled", "disabled");
        $("#video-record-toolbar").fadeOut();
    });
    
});

function resetVideoRecordTools() {
    $("#video-record-toolbar input[type='button'].icon-record").removeAttr("disabled");
    $("#video-record-toolbar input[type='button'].icon-play").attr("disabled", "disabled");
    $("#video-record-toolbar input[type='button'].icon-ok").attr("disabled", "disabled");
}

function showMediaPlayer(o) {

    switch (o.collabtype) {
        case 'sound':
            if (o.mediasent) {
                $("#media-player-toolbar video").css("height", 40);
                $("#media-player-toolbar").css("top", o.top);
                $("#media-player-toolbar").css("left", o.left);
                $("#media-player-toolbar video").attr("src", '/UserData/Audio/' + o.id + '.wav');
                $("#media-player-toolbar").show();
            }
            break;
        case 'video':
            if (o.mediasent) {
                $("#media-player-toolbar video").css("height", 300);
                $("#media-player-toolbar").css("top", o.top);
                $("#media-player-toolbar").css("left", o.left);
                $("#media-player-toolbar video").attr("src", '/UserData/Video/' + o.id + '.webm');
                $("#media-player-toolbar").show();
            }
            break;
    }

    $("#media-player-toolbar input[type='button'].icon-close").on("click", function () {
        $("#media-player-toolbar").fadeOut();
    });

}


/**
 * TEXT
 */
$("input[type='button'].icon-text").click(function () {

    canvas1.off('mouse:move');
    canvas1.off('mouse:down');
    canvas1.off('mouse:up');

    canvas1.on('mouse:up', function (o) {
        var id = uuid.v4();
        var startY = o.e.offsetY,
            startX = o.e.offsetX;

        escrevendo = true;
        var iText7 = new fabric.IText('', {
            left: startX,
            top: startY,
            padding: 7,
            fill: "#fff",
            fontFamily: 'Helvetica',
            textBackgroundColor: "rgba(" + hexToRgb(cordono).r + ", " + hexToRgb(cordono).g + ", " + hexToRgb(cordono).b + ", 0.5)",
            stroke: "#000",
            strokeWidth: 1,
            id: id,
            iddono: dono
        });
        canvas1.add(iText7).setActiveObject(iText7);
        iText7.enterEditing();
    });

});

// delete
$('html').keyup(function (e) {
    if (e.keyCode == 46) {
        if (canvas1.getActiveObject().iddono == dono)
            canvas1.getActiveObject().remove();
    }
});