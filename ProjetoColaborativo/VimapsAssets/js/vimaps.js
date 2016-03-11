
(function () {
    'use strict';

    console.log("starting collab...");

    var urlbase = document.location.protocol + "//177.131.33.18:8080";
    var showiframe = false;

    var headElement = document.getElementsByTagName("head")[0];
    var bodyElement = document.getElementsByTagName("body")[0];

    // creating css link rel
    var collabcss = urlbase + "/vimapsassets/css/vimaps.css?" + Math.floor(new Date().getTime() / 3600000);

    var script = document.createElement('link');
    script.type = 'text/css';
    script.rel = 'stylesheet';
    script.href = collabcss;
    headElement.appendChild(script);

    // creating fixed div
    var divfooter = document.createElement("div");
    divfooter.id = "footer";
    bodyElement.appendChild(divfooter);
    
    // creating open button
    var buttonopen = document.createElement("button");
    buttonopen.id = "babrir";
    buttonopen.name = "babrir";
    var tco = document.createTextNode("Abrir Colaborativo");
    buttonopen.appendChild(tco);
    buttonopen.onclick = function () {
        document.getElementById("iframeCollab").style["display"] = "block";
        document.getElementById("overlay").style["display"] = "block";
        document.getElementById("babrir").style["display"] = "none";
        document.getElementById("benviar").style["display"] = "none";
    };
    divfooter.appendChild(buttonopen);

    // creating button
    var button = document.createElement("button");
    button.id = "benviar";
    button.name = "benviar";
    button.onclick = function () {
        
        var canvas = document.getElementsByTagName("canvas");
        for (var i = 0; i < canvas.length; i++) { // searching rv canvas
            if (canvas[i].parentNode.className == "show") {
                document.getElementById("imgdata").value = canvas[i].toDataURL();
                document.getElementById("url").value = window.location.href;
                document.getElementById("formCollab").submit();
                document.getElementById("overlay").style["display"] = "block";
                document.getElementById("babrir").style["display"] = "none";
                document.getElementById("benviar").style["display"] = "none";
                showiframe = true;
                return;
            }
        }

        document.getElementById("imgdata").value = canvas[0].toDataURL();
        document.getElementById("url").value = window.location.href;
        document.getElementById("formCollab").submit();
        showiframe = true;
        document.getElementById("overlay").style["display"] = "block";
        document.getElementById("benviar").style["display"] = "none";
        document.getElementById("babrir").style["display"] = "none";
    };

    var t = document.createTextNode("Enviar para sessão colaborativa");
    button.appendChild(t);
    divfooter.appendChild(button);

    // create form
    var form = document.createElement("form");
    var imgdata = document.createElement("input");
    imgdata.type = "hidden";
    imgdata.id = "imgdata";
    imgdata.name = "imgdata";
    var url = document.createElement("input");
    url.type = "hidden";
    url.id = "url";
    url.name = "url";
    form.id = "formCollab";
    form.method = "POST";
    form.action = urlbase + "/Vimaps/SendImage";
    form.target = "iframeCollab";
    form.appendChild(imgdata);
    form.appendChild(url);
    bodyElement.appendChild(form);

    // create iframe
    var iframe = document.createElement("iframe");
    iframe.id = "iframeCollab";
    iframe.name = "iframeCollab";
    iframe.src = "http://177.131.33.18:8080/";
    iframe.frameBorder = "0";

    iframe.onload = function () {
        if (showiframe)
            document.getElementById("iframeCollab").style["display"] = "block";
    }

    bodyElement.appendChild(iframe);

    // css overlay
    var overlay = document.createElement("div");
    overlay.id = "overlay";
    overlay.name = "overlay";
    overlay.onclick = function () {
        closeCollab();
    };
    bodyElement.appendChild(overlay);

    function closeCollab() {
        document.getElementById("iframeCollab").style["display"] = "none";
        document.getElementById("overlay").style["display"] = "none";
        document.getElementById("benviar").style["display"] = "block";
        document.getElementById("babrir").style["display"] = "block";
    }

    // message listener
    window.addEventListener("message", function (event) {
        console.log("Recebido: " + event.data);
        var acao = event.data.split("<|>")[0];
        var par = event.data.split("<|>")[1];

        if (acao == "url") {
            closeCollab();
            window.location.href = par;
        } else if (acao == "fechar") {
            closeCollab();
        }
    });

})();


