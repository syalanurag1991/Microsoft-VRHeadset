// Unity derives Camera Input Component UI from this file
using UnityEngine;
using System.Collections;
#if UNITY_XBOXONE
using Kinect;
#endif

namespace Affdex
{
    /// <summary>
    /// Provides WebCam access to the detector.  Sample rate set per second.  Use
    /// </summary>
    [RequireComponent(typeof(Detector))]
    public class CameraInput : MonoBehaviour, IDetectorInput
    {
        /// <summary>
        /// Number of frames per second to sample.  Use 0 and call ProcessFrame() manually to run manually.
        /// Enable/Disable to start/stop the sampling
        /// </summary>
        public float sampleRate = 20;

        /// <summary>
        /// Should the selected camera be front facing?
        /// </summary>
        public bool isFrontFacing = true;

        /// <summary>
        /// Desired width for capture
        /// </summary>
        public int targetWidth = 640;

        /// <summary>
        /// Desired height for capture
        /// </summary>
        public int targetHeight = 480;

#if UNITY_XBOXONE
        /// <summary>
        /// Kinect texture
        /// </summary>
        [HideInInspector]
        private Texture2D cameraTexture;

        /// <summary>
        /// The rotation of the camera
        /// </summary>
        public float videoRotationAngle = 0;
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_ANDROID
        /// <summary>
        /// List of WebCams accessible to Unity
        /// </summary>
        [HideInInspector]
        protected WebCamDevice[] devices;

        /// <summary>
        /// WebCam chosen to gather metrics from
        /// </summary>
        [HideInInspector]
        protected WebCamDevice device;

        /// <summary>
        /// Web Cam texture
        /// </summary>
        [HideInInspector]
        private WebCamTexture cameraTexture;

        public float videoRotationAngle
        {
            get
            {
                return cameraTexture.videoRotationAngle;
            }
        }
#endif
        /// <summary>
        /// The detector that is on this game object
        /// </summary>
        public Detector detector
        {
            get; private set;
        }

        /// <summary>
        /// The texture that is being modified for processing
        /// </summary>
        public Texture Texture
        {
            get
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_XBOXONE || UNITY_IOS || UNITY_ANDROID
                return cameraTexture;
#else
                return new Texture();
#endif
            }
        }

        void Start()
        {
            if (!AffdexUnityUtils.ValidPlatform())
                return;
            detector = GetComponent<Detector>();
#if !UNITY_XBOXONE && UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_ANDROID
            devices = WebCamTexture.devices;
            if (devices.Length > 0)
            {
                SelectCamera(isFrontFacing);

                if (device.name != "Null")
                {
                    cameraTexture = new WebCamTexture(device.name, targetWidth, targetHeight, (int)sampleRate);
                    cameraTexture.Play();
                }
            }
#endif
        }

        /// <summary>
        /// Set the target device (by name or orientation)
        /// </summary>
        /// <param name="isFrontFacing">Should the device be forward facing?</param>
        /// <param name="name">The name of the webcam to select.</param>
        public void SelectCamera(bool isFrontFacing, string name = "")
        {
#if !UNITY_XBOXONE && UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_ANDROID
            foreach (WebCamDevice d in devices)
            {
                if (d.name.Length > 1 && d.name == name)
                {
                        cameraTexture.Stop();
                        device = d;

                        cameraTexture = new WebCamTexture(device.name, targetWidth, targetHeight, (int)sampleRate);
                        cameraTexture.Play();
                }
                else if (d.isFrontFacing == isFrontFacing)
                {
                    device = d;
                }
            }
#endif
        }

        void OnEnable()
        {
            if (!AffdexUnityUtils.ValidPlatform())
                return;

            //get the selected camera!

            if (sampleRate > 0)
                StartCoroutine(SampleRoutine());
        }

        /// <summary>
        /// Coroutine to sample frames from the camera
        /// </summary>
        /// <returns></returns>
        private IEnumerator SampleRoutine()
        {
            while (enabled)
            {
                yield return new WaitForSeconds(1 / sampleRate);
                ProcessFrame();
            }
        }


        /// <summary>
        /// Sample an individual frame from the webcam and send to detector for processing.
        /// </summary>
        public void ProcessFrame()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_XBOXONE || UNITY_IOS || UNITY_ANDROID
            if (cameraTexture != null)
            {
                if (detector.IsRunning)
                {
    #if UNITY_XBOXONE
                    cameraTexture = CreateKinectImageTexture();
    #else
                    if (cameraTexture.isPlaying)
                    {
    #endif
                    Frame.Orientation orientation = Frame.Orientation.Upright;

    #if UNITY_IOS || UNITY_ANDROID
                        // account for camera rotation on mobile devices
                        switch(cameraTexture.videoRotationAngle)
                        {
                            case 90:
                                orientation = Frame.Orientation.CW_90;
                                break;
                            case 180:
                                orientation = Frame.Orientation.CW_180;
                                break;
                            case 270:
                                orientation = Frame.Orientation.CW_270;
                                break;
                        }
    #endif

                        Frame frame = new Frame(cameraTexture.GetPixels32(), cameraTexture.width, cameraTexture.height, orientation, Time.realtimeSinceStartup);
                        detector.ProcessFrame(frame);
    #if !UNITY_XBOXONE
                    }
    #endif
                }
            }
#endif
                }

        void OnDestroy()
        {
#if !UNITY_XBOXONE && UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_ANDROID
            if (cameraTexture != null)
            {
                cameraTexture.Stop();
            }
#endif
        }

#if UNITY_XBOXONE
        Texture2D CreateKinectImageTexture()
        {
            Texture2D texture;

            FrameDescription frameDesc = new FrameDescription();
            if (SensorManager.colorFrameReader.GetFrameDescription(out frameDesc))
            {
                texture = new Texture2D(frameDesc.Width, frameDesc.Height, TextureFormat.YUY2, false);
                texture.filterMode = FilterMode.Bilinear;

                // Call Apply() so it's actually uploaded to the GPU
                texture.Apply();
                return texture;
            }
            else
            {
                Debug.LogError("Failed to get color frame desc.");
                return null;
            }
        }
#endif
    }
}
