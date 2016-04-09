
    var params = {},
    r = /([^&=]+)=?([^&]*)/g;

    function d(s) {
        return decodeURIComponent(s.replace(/\+/g, ' '));
    }

    var match, search = window.location.search;
    while (match = r.exec(search.substring(1))) {
        params[d(match[1])] = d(match[2]);

        if (d(match[2]) === 'true' || d(match[2]) === 'false') {
            params[d(match[1])] = d(match[2]) === 'true' ? true : false;
        }
    }
    window.params = params;

    var recordingPlayer = document.querySelector('#videoPreview');
    var recordRTC;
    var stream;
    var recordingEndedCallback;
    var mimeType = 'video/webm';

    var commonConfig = {
        onMediaCaptured: function (sstream) {
            stream = sstream;
            mediaCapturedCallback();
            // button.stream = stream;
            // if (button.mediaCapturedCallback) {
            //     button.mediaCapturedCallback();
            // }
            // 
            // button.innerHTML = 'Stop Recording';
            // button.disabled = false;
        },
        onMediaStopped: function () {
            // button.innerHTML = 'Start Recording';
            // 
            // if (!button.disableStateWaiting) {
            //     button.disabled = false;
            // }
        },
        onMediaCapturingFailed: function (error) {
            console.log("onMediaCapturingFailed: " + error);
            // if (error.name === 'PermissionDeniedError' && !!navigator.mozGetUserMedia) {
            //     intallFirefoxScreenCapturingExtension();
            // }
            //
            // commonConfig.onMediaStopped();
        }
    };

    function captureAudioPlusVideo(config) {
        captureUserMedia({ video: true, audio: true }, function (audioVideoStream) {
            recordingPlayer.srcObject = audioVideoStream;
            recordingPlayer.play();

            config.onMediaCaptured(audioVideoStream);

            audioVideoStream.onended = function () {
                config.onMediaStopped();
            };
        }, function (error) {
            config.onMediaCapturingFailed(error);
        });
    }

    function captureUserMedia(mediaConstraints, successCallback, errorCallback) {
        navigator.mediaDevices.getUserMedia(mediaConstraints).then(successCallback)/*.catch(errorCallback)*/;
    }

    function mediaCapturedCallback() {

        if (typeof MediaRecorder === 'undefined') { // opera or chrome etc.
            recordRTC = [];

            if (!params.bufferSize) {
                // it fixes audio issues whilst recording 720p
                params.bufferSize = 16384;
            }

            var options = {
                type: 'audio',
                bufferSize: typeof params.bufferSize == 'undefined' ? 0 : parseInt(params.bufferSize),
                sampleRate: typeof params.sampleRate == 'undefined' ? 44100 : parseInt(params.sampleRate),
                leftChannel: params.leftChannel || false,
                disableLogs: params.disableLogs || false,
                recorderType: webrtcDetectedBrowser === 'edge' ? StereoAudioRecorder : null
            };

            if (typeof params.sampleRate == 'undefined') {
                delete options.sampleRate;
            }

        }

    }

    function startRecording(e) {

        if (e.classList.contains("recording")) {
            // stop recording
            e.classList.remove("recording");

            recordRTC.stopRecording(function (url) {
                stream.stop();
                stream = null;
                recordingEndedCallback(url);
                recordingPlayer.src = (window.URL || window.webkitURL).createObjectURL(recordRTC.blob);
                recordingPlayer.play();
            });

        } else {
            // start recording
            e.classList.add("recording");

            if (typeof MediaRecorder === 'undefined') { // opera or chrome etc.
                var audioRecorder = RecordRTC(stream, options);

                var videoRecorder = RecordRTC(stream, {
                    type: 'video',
                    disableLogs: params.disableLogs || false,
                    canvas: {
                        width: params.canvas_width || 320,
                        height: params.canvas_height || 240
                    },
                    frameInterval: typeof params.frameInterval !== 'undefined' ? parseInt(params.frameInterval) : 20 // minimum time between pushing frames to Whammy (in milliseconds)
                });

                // to sync audio/video playbacks in browser!
                videoRecorder.initRecorder(function () {
                    audioRecorder.initRecorder(function () {
                        audioRecorder.startRecording();
                        videoRecorder.startRecording();
                    });
                });

                recordRTC.push(audioRecorder, videoRecorder);

                recordingEndedCallback = function () {
                    var audio = new Audio();
                    audio.src = audioRecorder.toURL();
                    audio.controls = true;
                    audio.autoplay = true;

                    audio.onloadedmetadata = function () {
                        recordingPlayer.src = videoRecorder.toURL();
                        recordingPlayer.play();
                    };

                    recordingPlayer.parentNode.appendChild(document.createElement('hr'));
                    recordingPlayer.parentNode.appendChild(audio);

                    if (audio.paused) audio.play();

                    stream.stop();
                    stream = null;
                };
                return;
            }


            recordRTC = RecordRTC(stream, {
                type: 'video',
                mimeType: mimeType,
                disableLogs: params.disableLogs || false,
                // bitsPerSecond: 25 * 8 * 1025 // 25 kbits/s
                getNativeBlob: false // enable it for longer recordings
            });

            recordingEndedCallback = function (url) {
                recordingPlayer.muted = false;
                recordingPlayer.removeAttribute('muted');
                recordingPlayer.src = url;
                recordingPlayer.play();

                recordingPlayer.onended = function () {
                    recordingPlayer.pause();
                    recordingPlayer.src = URL.createObjectURL(recordRTC.blob);
                };

                if (stream) {
                    stream.stop();
                    stream = null;
                }
            };

            recordRTC.startRecording();
        }

        
    }
    
