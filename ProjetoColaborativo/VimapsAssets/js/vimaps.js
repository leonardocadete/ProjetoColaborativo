
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

    // creating close button
    var buttonclose = document.createElement("button");
    buttonclose.id = "bfechar";
    buttonclose.name = "bfechar";
    buttonclose.style["display"] = "none";
    var tc = document.createTextNode("Voltar para o Vimaps");
    buttonclose.appendChild(tc);
    buttonclose.onclick = function() {
        closeCollab();
    };
    divfooter.appendChild(buttonclose);

    // creating button
    var button = document.createElement("button");
    button.id = "benviar";
    button.name = "benviar";
    button.onclick = function () {

        // login test
        // document.getElementById("overlay").style["display"] = "block";
        // document.getElementById("divlogin").style["display"] = "block";
        // return;
        // login test

        var canvas = document.getElementsByTagName("canvas");
        for (var i = 0; i < canvas.length; i++) { // searching rv canvas
            if (canvas[i].parentNode.className == "show") {
                document.getElementById("imgdata").value = canvas[i].toDataURL();
                document.getElementById("formCollab").submit();
                document.getElementById("overlay").style["display"] = "block";
                document.getElementById("bfechar").style["display"] = "block";
                document.getElementById("benviar").style["display"] = "none";
                showiframe = true;
                return;
            }
        }

        document.getElementById("imgdata").value = canvas[0].toDataURL();
        document.getElementById("formCollab").submit();
        showiframe = true;
        document.getElementById("overlay").style["display"] = "block";
        document.getElementById("bfechar").style["display"] = "block";
        document.getElementById("benviar").style["display"] = "none";
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
    form.id = "formCollab";
    form.method = "POST";
    form.action = urlbase + "/Vimaps/SendImage";
    form.target = "iframeCollab";
    form.appendChild(imgdata);
    bodyElement.appendChild(form);

    // create iframe
    var iframe = document.createElement("iframe");
    iframe.id = "iframeCollab";
    iframe.name = "iframeCollab";
    iframe.src = "about:blank";
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
        document.getElementById("bfechar").style["display"] = "none";
        document.getElementById("benviar").style["display"] = "block";
    }

})();

