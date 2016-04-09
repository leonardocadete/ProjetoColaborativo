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

var audio;
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

canvas1.on('selection:cleared', function() {
    if (audio) {
        audio.pause();
    }
});

function SaveObject(target, remover) {
    
    console.log(target);

    if (target.naosalvar)
        return;

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

var pinpath = "M156.831,70.804c0,13.473-10.904,24.396-24.357,24.396c-13.434,0-24.357-10.923-24.357-24.396c0-13.434,10.904-24.337,24.357-24.337C145.927,46.467,156.831,57.37,156.831,70.804z M203.298,70.795c0,8.764-1.661,17.098-4.563,24.836c-9.282,27.571-70.736,169.307-70.736,169.307S70.14,110.403,65.118,92.68c-2.237-6.868-3.478-14.196-3.478-21.866C61.64,31.743,93.354,0,132.474,0C171.593-0.01,203.307,31.733,203.298,70.795zM177.661,71.078c0-24.953-20.214-45.197-45.187-45.197c-24.953,0-45.177,20.234-45.177,45.187s20.224,45.187,45.177,45.187C157.446,116.255,177.661,96.031,177.661,71.078z";
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

var speakerpath = "M19.779,3.349l-1.111,1.664C20.699,6.663,22,9.179,22,12    c0,2.822-1.301,5.338-3.332,6.988l1.111,1.663C22.345,18.639,24,15.516,24,12C24,8.485,22.346,5.362,19.779,3.349z M17.55,6.687    l-1.122,1.68c0.968,0.913,1.58,2.198,1.58,3.634s-0.612,2.722-1.58,3.635l1.122,1.68C19.047,16.03,20,14.128,20,12    C20,9.873,19.048,7.971,17.55,6.687z M12,1c-1.177,0-1.533,0.684-1.533,0.684S7.406,5.047,5.298,6.531C4.91,6.778,4.484,7,3.73,7    H2C0.896,7,0,7.896,0,9v6c0,1.104,0.896,2,2,2h1.73c0.754,0,1.18,0.222,1.567,0.469c2.108,1.484,5.169,4.848,5.169,4.848    S10.823,23,12,23c1.104,0,2-0.895,2-2V3C14,1.895,13.104,1,12,1z";
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

            $("#audio-record-toolbar").css("top", startY); 
            $("#audio-record-toolbar").css("left", startX); 
            $("#audio-record-toolbar").fadeIn();
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

    $("#audio-record-toolbar input[type='button'].icon-delete").click(function () {
        canvas1.remove(drawingobject);
        $("#audio-record-toolbar").fadeOut();
    });

});

function resetAudioRecordTools() {
    $("#audio-record-toolbar input[type='button'].icon-record").removeAttr("disabled");
    $("#audio-record-toolbar input[type='button'].icon-play").attr("disabled", "disabled");
    $("#audio-record-toolbar input[type='button'].icon-ok").attr("disabled", "disabled");
}

/**
 * VIDEO
 */

var youtubepath = "M391.939,159.642c-11.485-12.816-24.349-12.892-30.247-13.618  c-42.252-3.275-105.625-3.275-105.625-3.275h-0.142c0,0-63.374,0-105.616,3.275c-5.898,0.727-18.752,0.802-30.247,13.618  c-9.041,9.777-11.995,31.984-11.995,31.984s-3.02,26.057-3.02,52.115v24.424c0,26.076,3.02,52.124,3.02,52.124  s2.945,22.197,11.995,31.955c11.495,12.816,26.566,12.429,33.286,13.769C177.499,368.487,256,369.251,256,369.251  s63.44-0.113,105.691-3.35c5.898-0.755,18.762-0.83,30.247-13.647c9.041-9.758,11.995-31.955,11.995-31.955s3.02-26.057,3.02-52.124  V243.75c0-26.066-3.02-52.124-3.02-52.124S400.99,169.42,391.939,159.642z M218.297,312.626V199.375l94.376,56.626L218.297,312.626z  ";
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