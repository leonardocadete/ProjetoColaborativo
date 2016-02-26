
(function () {
    'use strict';
    
    console.log("starting collab...");

    var urlbase = document.location.protocol + "//177.131.33.18:8080";

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
                document.getElementById("benviar").style["display"] = "none";
                return;
            }
        }

        document.getElementById("imgdata").value = canvas[0].toDataURL();
        document.getElementById("formCollab").submit();
        document.getElementById("overlay").style["display"] = "block";
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
    iframe.src = "";
    iframe.frameBorder = "0";

    iframe.onload = function () {
        console.log(iframe.src);
        if(iframe.src.indexOf(urlbase) == 0)
            document.getElementById("iframeCollab").style["display"] = "block";
    }

    bodyElement.appendChild(iframe);

    // css overlay
    var overlay = document.createElement("div");
    overlay.id = "overlay";
    overlay.name = "overlay";
    overlay.onclick = function () {
        document.getElementById("iframeCollab").style["display"] = "none";
        document.getElementById("overlay").style["display"] = "none";
        document.getElementById("benviar").style["display"] = "block";
        //document.getElementById("divlogin").style["display"] = "none";
    };
    bodyElement.appendChild(overlay);

    // login
    //var login = document.createElement("div");
    //login.id = "divlogin";
    //login.name = "divlogin";
    //login.className = "login-box"; 
    //bodyElement.appendChild(login);
    //
    //// create login iframe
    //var loginiframe = document.createElement("iframe");
    //loginiframe.id = "loginiframeCollab";
    //loginiframe.name = "loginiframeCollab";
    //loginiframe.src = "http://177.131.33.18:8080";
    //loginiframe.frameBorder = "0";
    //login.appendChild(loginiframe);

})();

