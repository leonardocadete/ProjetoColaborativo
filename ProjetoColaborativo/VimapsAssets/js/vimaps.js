
(function () {
    'use strict';

    console.log("starting collab...");

    var urlbase = "https://i-ello.com";
    //var urlbase = "https://colaborativo.azurewebsites.net";

    var script = document.createElement('script');
    script.src = urlbase + "/VimapsAssets/js/exportcanvas.js";
    document.getElementsByTagName('head')[0].appendChild(script);

    var script2 = document.createElement('script');
    script2.src = urlbase + "/VimapsAssets/js/html2canvas.js";
    document.getElementsByTagName('head')[0].appendChild(script2);

            // canvg tava faltando
            var script3 = document.createElement('script');
            // script3.src = "https://canvg.github.io/canvg/canvg.js";
            script3.src = urlbase + "/VimapsAssets/js/canvg.js";
            document.getElementsByTagName('head')[0].appendChild(script3);

    var showiframe = false;

    var headElement = document.getElementsByTagName("head")[0];
    var bodyElement = document.getElementsByTagName("body")[0];

    // creating css link rel
    var collabcss = urlbase + "/vimapsassets/css/vimaps.css?2" + Math.floor(new Date().getTime() / 3600000);

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
    var openiconsvg = "m9.52328,9.4059c0,1.24908 -1.01261,2.26164 -2.26164,2.26164c-1.24908,0 -2.26164,-1.01256 -2.26164,-2.26164c0,-1.24908 1.01256,-2.26164 2.26164,-2.26164c1.24903,0 2.26164,1.01256 2.26164,2.26164zm11.95439,0c0,1.24908 1.01261,2.26164 2.26169,2.26164c1.24903,0 2.26164,-1.01256 2.26164,-2.26164c0,-1.24908 -1.01261,-2.26164 -2.26164,-2.26164c-1.24913,0 -2.26169,1.01256 -2.26169,2.26164zm-12.60058,10.33893l0,-5.34878c0,-0.97156 -0.78773,-1.75923 -1.75923,-1.75923l-0.03554,0c-0.97156,0 -1.75923,0.78768 -1.75923,1.75923l0,5.67187c0,1.07072 0.86783,1.9386 1.93855,1.9386l2.90783,0c0.35702,0 0.64618,0.28917 0.64618,0.64618l0,4.52328c0,0.53538 0.43389,0.96923 0.96928,0.96923l0.32309,0c0.53538,0 0.96928,-0.43389 0.96928,-0.96923l0,-5.49265c0,-1.07072 -0.86783,-1.93855 -1.93855,-1.93855l-2.26164,0l0,0.00005zm15.04153,-7.10802l-0.03554,0c-0.97151,0 -1.75919,0.78768 -1.75919,1.75923l0,5.34878l-2.26169,0c-1.07072,0 -1.93855,0.86783 -1.93855,1.93855l0,5.34232c0,0.61839 0.50112,1.11951 1.11951,1.11951l0.02262,0c0.61839,0 1.11951,-0.50112 1.11951,-1.11951l0,-4.37305c0,-0.35702 0.28917,-0.64618 0.64623,-0.64618l2.90783,0c1.07072,0 1.93855,-0.86788 1.93855,-1.9386l0,-5.67183c-0.00005,-0.97156 -0.78777,-1.75923 -1.75928,-1.75923zm-3.08714,4.84638l0,-0.96928c0,-0.17832 -0.14477,-0.32309 -0.32309,-0.32309l-10.01584,0c-0.17837,0 -0.32309,0.14477 -0.32309,0.32309l0,0.96928c0,0.17832 0.14473,0.32309 0.32309,0.32309l10.01584,0c0.17832,0 0.32309,-0.14477 0.32309,-0.32309zm-6.20303,-16.48319c-2.35534,0 -4.26448,1.51112 -4.26448,3.37503c0,1.11049 0.68077,2.09264 1.72659,2.70784c-0.10244,0.5609 -0.4281,1.22157 -0.88332,1.67685c1.1121,-0.08305 2.09074,-0.40192 2.73174,-1.05779c0.22517,0.02908 0.45456,0.04813 0.6898,0.04813c2.35534,0 4.26448,-1.51112 4.26448,-3.37503c0,-1.86391 -1.9098,-3.37503 -4.26481,-3.37503zm-1.99932,4.04126c-0.36799,0 -0.66619,-0.29819 -0.66619,-0.66619s0.29819,-0.66623 0.66619,-0.66623s0.66619,0.29824 0.66619,0.66623s-0.29819,0.66619 -0.66619,0.66619zm1.99932,0c-0.36804,0 -0.66623,-0.29819 -0.66623,-0.66619s0.29819,-0.66623 0.66623,-0.66623c0.36799,0 0.66619,0.29824 0.66619,0.66623s-0.29819,0.66619 -0.66619,0.66619zm1.99894,0c-0.36799,0 -0.66619,-0.29819 -0.66619,-0.66619s0.29819,-0.66623 0.66619,-0.66623s0.66619,0.29824 0.66619,0.66623s-0.29815,0.66619 -0.66619,0.66619z";

    var buttonopen = document.createElement("button");
    buttonopen.id = "babrir";
    buttonopen.name = "babrir";
    buttonopen.title = "Abrir sistema colaborativo";
    buttonopen.className = "cesium-button cesium-toolbar-button cesium-home-button";

    var buttonopensvg = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
    buttonopensvg.setAttributeNS(null, "viewBox", "0 0 " + 30 + " " + 30);
    buttonopensvg.setAttributeNS(null, "width", 30);
    buttonopensvg.setAttributeNS(null, "height", 30);
    buttonopensvg.style.display = "block";
    var buttonopenicon = document.createElementNS("http://www.w3.org/2000/svg", 'path');
    buttonopenicon.setAttribute("d", openiconsvg);
    buttonopensvg.appendChild(buttonopenicon);
    buttonopen.appendChild(buttonopensvg);

    buttonopen.onclick = function () {
        document.getElementById("iframeCollab").style["display"] = "block";
        document.getElementById("overlay").style["display"] = "block";
        document.getElementById("babrir").style["display"] = "none";
        document.getElementById("benviar").style["display"] = "none";
    };
    divfooter.appendChild(buttonopen);

    // creating button
    var sendiconsvg1 = "m29.76233,9.30334l-6.48037,-6.48078c-0.21404,-0.21381 -0.46752,-0.32062 -0.75982,-0.32062c-0.13448,0 -0.27552,0.02814 -0.42159,0.08423c-0.43896,0.19134 -0.65845,0.52326 -0.65845,0.9958l0,3.24057l-2.7004,0c-1.19252,0 -2.29821,0.06189 -3.31623,0.18561c-1.01814,0.12366 -1.91546,0.29515 -2.69177,0.51475c-0.77637,0.21919 -1.47679,0.49216 -2.10125,0.81846c-0.62434,0.32618 -1.15593,0.67222 -1.59484,1.03794c-0.43896,0.36567 -0.8242,0.78205 -1.15611,1.24897c-0.33209,0.46686 -0.59632,0.92528 -0.7934,1.37535c-0.19672,0.4502 -0.3542,0.9452 -0.47242,1.48518c-0.11834,0.5401 -0.19702,1.04634 -0.23639,1.51876c-0.03937,0.47254 -0.05905,0.98463 -0.05905,1.53567c0,0.63031 0.0986,1.31648 0.29544,2.05934c0.1969,0.74239 0.41349,1.38363 0.64976,1.92379c0.23621,0.53969 0.5148,1.09966 0.83549,1.67895c0.32062,0.57941 0.54276,0.96453 0.66654,1.15593c0.12372,0.19093 0.24206,0.36572 0.35449,0.52314c0.11249,0.14624 0.25873,0.2193 0.43867,0.2193c0.04522,0 0.11243,-0.01099 0.20258,-0.03358c0.25873,-0.12384 0.37116,-0.31477 0.33741,-0.57368c-0.50623,-3.78044 -0.08985,-6.44159 1.24891,-7.98287c1.29408,-1.47389 3.75803,-2.21096 7.39199,-2.21096l2.70058,0l0,3.24027c0,0.47295 0.2193,0.80469 0.65827,0.99609c0.14607,0.05622 0.28687,0.08418 0.42159,0.08418c0.30383,0 0.55713,-0.10682 0.75982,-0.32045l6.4806,-6.48084c0.21357,-0.21393 0.32062,-0.46686 0.32062,-0.75941c-0.00006,-0.29231 -0.10711,-0.54566 -0.32068,-0.75911z";
    var sendiconsvg2 = "m25.40803,18.70348c-0.21428,-0.09038 -0.41089,-0.0451 -0.5907,0.13495c-0.29249,0.27002 -0.59656,0.47839 -0.9115,0.62446c-0.20293,0.11243 -0.30389,0.27546 -0.30389,0.48927l0,3.61197c0,0.74256 -0.26399,1.37778 -0.79298,1.90695c-0.52875,0.52881 -1.16474,0.79322 -1.90718,0.79322l-14.04147,0c-0.74268,0 -1.37837,-0.26441 -1.90718,-0.79322c-0.52887,-0.52917 -0.79316,-1.16432 -0.79316,-1.90695l0,-14.04171c0,-0.74262 0.26429,-1.37807 0.79316,-1.90689c0.52875,-0.52887 1.16444,-0.79316 1.90718,-0.79316l1.89022,0c0.06745,0 0.15759,-0.02264 0.27002,-0.06756c0.63013,-0.38263 1.37807,-0.72028 2.24459,-1.01276c0.29254,-0.05616 0.43879,-0.23609 0.43879,-0.54005c0,-0.14624 -0.05338,-0.27256 -0.16031,-0.37968c-0.10693,-0.10664 -0.23326,-0.16007 -0.37968,-0.16007l-4.3034,0c-1.33888,0 -2.48376,0.47538 -3.43451,1.42607c-0.95064,0.95064 -1.42601,2.09551 -1.42601,3.4344l0,14.04124c0,1.33894 0.47538,2.48376 1.42607,3.43451c0.95075,0.95075 2.09563,1.42613 3.43451,1.42613l14.04147,0c1.3387,0 2.4837,-0.47538 3.43445,-1.42613c0.95081,-0.95075 1.42619,-2.09551 1.42619,-3.43451l0,-4.37108c-0.00006,-0.22498 -0.11822,-0.38801 -0.35467,-0.48939z";
    var button = document.createElement("button");
    button.id = "benviar";
    button.name = "benviar";
    button.title = "Enviar esta tela para o sistema colaborativo";
    button.className = "cesium-button cesium-toolbar-button cesium-home-button";

    var buttonsendsvg = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
    buttonsendsvg.setAttributeNS(null, "viewBox", "0 0 " + 30 + " " + 30);
    buttonsendsvg.setAttributeNS(null, "width", 30);
    buttonsendsvg.setAttributeNS(null, "height", 30);
    buttonsendsvg.style.display = "block";
    var buttonsendicon1 = document.createElementNS("http://www.w3.org/2000/svg", 'path');
    buttonsendicon1.setAttribute("d", sendiconsvg1);
    buttonsendsvg.appendChild(buttonsendicon1);
    var buttonsendicon2 = document.createElementNS("http://www.w3.org/2000/svg", 'path');
    buttonsendicon2.setAttribute("d", sendiconsvg2);
    buttonsendsvg.appendChild(buttonsendicon2);
    button.appendChild(buttonsendsvg);

    button.onclick = function () {

        salvarImagemColaborativo(function (data) {
            document.getElementById("imgdata").value = data;
            document.getElementById("url").value = window.location.href;
            document.getElementById("formCollab").submit();

            showiframe = true;
            document.getElementById("overlay").style["display"] = "block";
            document.getElementById("benviar").style["display"] = "none";
            document.getElementById("babrir").style["display"] = "none";
        });

    };

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
    iframe.src = "https://i-ello.com/";
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