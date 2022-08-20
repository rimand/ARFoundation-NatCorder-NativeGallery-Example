/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatSuite.Examples {

    using System.Collections;
    using UnityEngine;
    using Recorders;
    using Recorders.Clocks;
    using Recorders.Inputs;

    public class CameraRecReplayCam : MonoBehaviour
    {

        [Header(@"Recording")]
        private int videoWidth = 720;
        private int videoHeight = 1280;
        public bool recordMicrophone;

        private MP4Recorder recorder;
        private CameraInput cameraInput;
        private AudioInput audioInput;
        private AudioSource microphoneSource;

        private JPGRecorder recorderJpg;

        private void Start()
        {
            StartCoroutine("AddMicrophoneSource");
        }

        private IEnumerator AddMicrophoneSource()
        {

            // Start microphone
            microphoneSource = gameObject.AddComponent<AudioSource>();
            microphoneSource.mute = false;
            microphoneSource.loop = true;
            microphoneSource.bypassEffects = false;
            microphoneSource.bypassListenerEffects = false;
            microphoneSource.clip = Microphone.Start(null, true, 10, AudioSettings.outputSampleRate);
            yield return new WaitUntil(() => Microphone.GetPosition(null) > 0);
            microphoneSource.Play();
            Debug.Log("================== Finnich Add microphone ==============");
        }

        private void OnDestroy()
        {
            // Stop microphone
            microphoneSource.Stop();
            Microphone.End(null);
        }

        public void StartRecording()
        {
            // Start recording
            var frameRate = 30;
            var sampleRate = recordMicrophone ? AudioSettings.outputSampleRate : 0;
            var channelCount = recordMicrophone ? (int)AudioSettings.speakerMode : 0;
            var clock = new RealtimeClock();
            recorder = new MP4Recorder(videoWidth, videoHeight, frameRate, sampleRate, channelCount, audioBitRate: 96_000);
            // Create recording inputs
            cameraInput = new CameraInput(recorder, clock, Camera.main);
            audioInput = recordMicrophone ? new AudioInput(recorder, clock, microphoneSource, true) : null;
            // Unmute microphone
            microphoneSource.mute = audioInput == null;
            //microphoneSource.mute = false;
        }

        public async void StopRecording()
        {
            // Mute microphone
            microphoneSource.mute = true;
            // Stop recording
            audioInput?.Dispose();
            cameraInput.Dispose();
            var path = await recorder.FinishWriting();
            // Playback recording
            Debug.Log($"Saved recording to: {path}");
            NativeGallery.Permission permission = NativeGallery.SaveVideoToGallery(path, "Miracle Cannabis", "testVideo.mp4", (success, path) => Debug.Log("Media save result: " + success + " " + path));
            Debug.Log("Permission result: " + permission);
#if !UNITY_STANDALONE
            //Handheld.PlayFullScreenMovie($"file://{path}");
#endif
        }

        public void Screenshot()
        {
            Debug.Log("+++++++++Screenshot+++++++");
            //TakeScreenshotAndSave();
            StartCoroutine(TakeScreenshotAndSave());
        }

        private IEnumerator TakeScreenshotAndSave()
        {
            yield return new WaitForEndOfFrame();

            recorderJpg = new JPGRecorder(videoWidth, videoHeight);

            RenderTexture.active = Camera.main.targetTexture;
            Camera.main.Render();
            Texture2D texture = new Texture2D(videoWidth, videoHeight, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, videoWidth, videoHeight), 0, 0);
            texture.Apply();
            RenderTexture.active = null;

            NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(texture, "Miracle Cannabis", "Image.png", (success, path) => Debug.Log("Media save result: " + success + " " + path));
            Debug.Log("Permission result: " + permission);

            //recorderJpg.CommitFrame(texture.GetPixels32());
            Debug.Log("+++++++++TakeScreenshotAndSave+++++++");

            //stopTake();

            Destroy(texture);
        }

        private async void stopTake()
        {
            var path = await recorderJpg.FinishWriting();
            NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(path, "Miracle Cannabis", "Image.jpg", (success, path) => Debug.Log("Media save result: " + success + " " + path));
            Debug.Log("Permission result: " + permission);
        }



    }
}