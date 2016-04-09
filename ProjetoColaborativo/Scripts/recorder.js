/*License (MIT)

Copyright © 2013 Matt Diamond

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/

(function (window) {

  var WORKER_PATH = '/Scripts/recorderWorker.js';

  var Recorder = function(source, cfg){
    var config = cfg || {};
    var bufferLen = config.bufferLen || 4096;
    this.context = source.context;
    if(!this.context.createScriptProcessor){
       this.node = this.context.createJavaScriptNode(bufferLen, 2, 2);
    } else {
       this.node = this.context.createScriptProcessor(bufferLen, 2, 2);
    }
   
    var worker = new Worker(config.workerPath || WORKER_PATH);
    worker.postMessage({
      command: 'init',
      config: {
        sampleRate: this.context.sampleRate
      }
    });
    var recording = false, currCallback;

    this.node.onaudioprocess = function(e){
      if (!recording) return;
      worker.postMessage({
        command: 'record',
        buffer: [
          e.inputBuffer.getChannelData(0),
          e.inputBuffer.getChannelData(1)
        ]
      });
    }

    this.configure = function(cfg){
      for (var prop in cfg){
        if (cfg.hasOwnProperty(prop)){
          config[prop] = cfg[prop];
        }
      }
    }

    this.record = function(){
      recording = true;
    }

    this.stop = function(){
      recording = false;
    }

    this.clear = function () {
      worker.postMessage({ command: 'clear' });
    }

    this.getBuffers = function (cb) {
      currCallback = cb || config.callback;
      worker.postMessage({ command: 'getBuffers' });
    }

    this.exportWAV = function(cb, type){
      currCallback = cb || config.callback;
      type = type || config.type || 'audio/wav';
      if (!currCallback) throw new Error('Callback not set');
      worker.postMessage({
        command: 'exportWAV',
        type: type
      });
    }

    this.exportMonoWAV = function(cb, type){
      currCallback = cb || config.callback;
      type = type || config.type || 'audio/wav';
      if (!currCallback) throw new Error('Callback not set');
      worker.postMessage({
        command: 'exportMonoWAV',
        type: type
      });
    }

    worker.onmessage = function (e) {
      var blob = e.data;
      currCallback(blob);
    }

    source.connect(this.node);
    this.node.connect(this.context.destination);   // if the script node is not connected to an output the "onaudioprocess" event is not triggered in chrome.
  };

  Recorder.setupDownload = function (blob) {
      $("#audio-record-toolbar input[type='button'].icon-ok").off("click");
      $("#audio-record-toolbar input[type='button'].icon-play").off("click");

      $("#audio-record-toolbar input[type='button'].icon-play").click(function () {
          $("#audio-record-toolbar input[type='button'].icon-play").attr("disabled", "disabled");
          var url = (window.URL || window.webkitURL).createObjectURL(blob);
          var a = new Audio(url);
          a.onended = function () {
              $("#audio-record-toolbar input[type='button'].icon-play").removeAttr("disabled");
          }
          a.play();
      });

      $("#audio-record-toolbar input[type='button'].icon-ok").click(function () {

          var id = $("#audio-record-toolbar input[type='button'].icon-ok").attr("data-objectid");

          var fd = new FormData();
          fd.append('file', blob);
          fd.append('objectid', id);
          $.ajax({
              type: 'POST',
              url: '/Vimaps/SendAudio',
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

          $("#audio-record-toolbar input[type='button'].icon-play").attr("disabled", "disabled");
          $("#audio-record-toolbar input[type='button'].icon-ok").attr("disabled", "disabled");
          $("#audio-record-toolbar").fadeOut();
      });
      
      // var url = (window.URL || window.webkitURL).createObjectURL(blob);
      // var link = document.getElementById("save");
      // link.href = url;
      // link.download = filename || 'output.wav';
  }

  window.Recorder = Recorder;

})(window);