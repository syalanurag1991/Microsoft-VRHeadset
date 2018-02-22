using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Affdex
{
    /// <summary>
    /// Core detector object.  Handles all communication with the native APIs.  
    /// </summary>
    public class Detector : MonoBehaviour
    {
        /// <summary>
        /// Boolean to determine if the detector should start on wake
        /// </summary>
        public bool startOnWake = true;

        /// <summary>
        /// True when the Detector is running
        /// </summary>
        public bool IsRunning { get { return state == State.STARTED; } }

        /// <summary>
        /// If frames are processed as discrete images
        /// </summary>
        public bool discrete;

        /// <summary>
        /// Emotions for detector to look for (Contains Unity LayerMask values (pow2)) - dont let people use this programatically
        /// </summary>
        public Emotions emotions;

        /// <summary>
        /// Listener to receive callback events for the detector.  All events received in Update() call.
        /// </summary>
        public ImageResultsListener Listener
        {
            get
            {
                if (listener == null)
                {
                    listener = (ImageResultsListener)GameObject.FindObjectOfType(typeof(ImageResultsListener));
                }

                return listener;
            }
            set
            {
                listener = value;
            }
        }
        
        /// <summary>
        /// Expressions for the detector to look for (Contains Unity LayerMask values (pow2)) - dont let people use this programatically
        /// </summary>
        public Expressions expressions;

        /// <summary>
        /// Initialization flag
        /// </summary>
        private bool initialized = false;

        /// <summary>
        /// Detector status
        /// </summary>
        private enum State
        {
            STOPPED,
            STARTING,
            STARTED
        }
        private volatile State state = State.STOPPED;

        /// <summary>
        /// Native Platform handle
        /// </summary>
        private NativePlatform nativePlatform;

        private ImageResultsListener listener;

        /// <summary>
        /// Event callbacks
        /// </summary>
        private List<NativeEvent> nativeEvents;

        /// <summary>
        /// Events available flag
        /// </summary>
        private volatile bool hasEvents = false;

        /// <summary>
        /// Synchronization lock
        /// </summary>
        private object eventLock = new object();

        /// <summary>
        /// Pointer to loaded library if it doesn't exist already
        /// </summary>
        private static IntPtr lib;

        /// <summary>
        /// Coroutines
        /// </summary>
        private Coroutine startDetectorCoroutine;
        private Coroutine initializeCoroutine;

        internal static bool LoadNativeDll (string filename)
        {

            if (lib != IntPtr.Zero) {
                return true;
            }
            lib = NativeMethods.LoadLibrary (filename);
            if (lib == IntPtr.Zero) {
                Debug.LogError ("Failed to load native library!");
                return false;
            }
            return true;
        }

        //start called via unity
        public void Start ()
        {
            if (!AffdexUnityUtils.ValidPlatform ())
                return;

            if (startOnWake) {
                try
                {
                    StartDetector();
                }
                catch (Exception e)
                {
                    Debug.Log("Detector.Start: caught " + e.Message + " " + e.StackTrace);
                }
            }
        }

        /// <summary>
        /// Unity onEnable callback for the detector component
        /// </summary>
        public void OnEnable ()
        {
            if (!AffdexUnityUtils.ValidPlatform ())
                return;

            StartCoroutine(ListenForEvents ());
        }

        /// <summary>
        /// Method to start the detector if it is not already running
        /// </summary>
        public void StartDetector()
        {
            if (state != State.STOPPED)
                return;

            state = State.STARTING;

            startDetectorCoroutine = StartCoroutine(StartDetectorAsync());
        }

        /// <summary>
        /// Single frame processing
        /// </summary>
        /// <param name="frame">Frame to process</param>
        public void ProcessFrame(Frame frame)
        {
            if (!IsRunning)
            {
                return;
            }

            int bytesPerPixel = 3;
            byte[] destBytes = new byte[frame.rgba.Length * bytesPerPixel];
	
            for (int y = 0; y < frame.h; y++)
            {
                for (int x = 0; x < frame.w; x++)
				{
					int frameByteIndex = (y * (frame.w)) + x;

					int idx = ((frame.h - y - 1) * (frame.w * bytesPerPixel)) + (x * bytesPerPixel);

					destBytes[idx] = frame.rgba[frameByteIndex].b;
					destBytes[idx + 1] = frame.rgba[frameByteIndex].g;
					destBytes[idx + 2] = frame.rgba[frameByteIndex].r;
                }
            }
            nativePlatform.ProcessFrame(destBytes, frame.w, frame.h, frame.orientation, frame.timestamp);
        }

        /// <summary>
        /// Method to stop the detector
        /// </summary>
        public void StopDetector ()
        {
            switch (state)
            {
                case State.STOPPED:
                    break;
                case State.STARTING:
                    StopCoroutine(startDetectorCoroutine);
                    StopCoroutine(initializeCoroutine);
                    state = State.STOPPED;
                    break;
                case State.STARTED:
                    nativePlatform.StopDetector();

                    lock (eventLock)
                    {
                        // purge pending events; we don't wand the client to continue to get callbacks after this method returns
                        nativeEvents.Clear();
                        hasEvents = false;

                        // note: changing state to STOPPED here prevents further events from being added in a
                        // background thread (see AddEvent method). We need to do this here inside the lock block
                        // to avoid a race condition with that method.
                        state = State.STOPPED;
                    }

                    break;
            }
        }

        [Obsolete("Stop is deprecated, please use StopDetector instead")]
        public void Stop()
        {
            StopDetector();
        }

        /// <summary>
        /// Method Unity calls when destroying objects.  This will release the detector and it's associated memory.
        /// </summary>
        public void OnDestroy ()
        {
            StopDetector();
            if (nativePlatform != null)
                nativePlatform.Release ();
        }

        /// <summary>
        /// Set State to Off/On for an expression
        /// </summary>
        /// <param name="expression">Expression to set the state of</param>
        /// <param name="expressionState">Off/On state</param>
        public void SetExpressionState (Expressions expression, bool expressionState)
        {
            if (IsRunning)
                nativePlatform.SetExpressionState ((int)expression, expressionState);
        }

        /// <summary>
        /// Set State to Off/On for an emotion
        /// </summary>
        /// <param name="emotion">Emotion to set the state of</param>
        /// <param name="emotionState">Off/On state</param>
        public void SetEmotionState (Emotions emotion, bool emotionState)
        {
            if (IsRunning)
                nativePlatform.SetEmotionState ((int)emotion, emotionState);
        }

        //todo: Add GetExpressionState and GetEmotionState

        /// <summary>
        /// Add Event to the queue for callback
        /// </summary>
        /// <param name="e"></param>
        internal void AddEvent(NativeEvent e)
        {
            lock (eventLock)
            {
                if (IsRunning)
                {
                    if (nativeEvents == null)
                        nativeEvents = new List<NativeEvent>();

                    nativeEvents.Add(e);
                    hasEvents = true;
                }
            }
        }

        private IEnumerator StartDetectorAsync()
        {
            if (!initialized)
            {
                initializeCoroutine = StartCoroutine(Initialize(this.discrete));
                yield return initializeCoroutine;
            }

            if (nativePlatform.StartDetector() == 1)
            {
                state = State.STARTED;
            }
        }

        /// <summary>
        /// Initialize the detector
        /// </summary>
        /// <param name="discrete">If the frames processed as discrete images</param>
        private IEnumerator Initialize(bool discrete = false)
        {
            if (initialized)
                yield break;

            // Libraries are expected in Assets/Affdex/Plugins
            String rootPath;
            if (RuntimePlatform.OSXPlayer == Application.platform || RuntimePlatform.WindowsPlayer == Application.platform)
                rootPath = Application.dataPath;
            else
                rootPath = Path.Combine(Application.dataPath, "Affdex");

            rootPath = Path.Combine(rootPath, "Plugins");

            Debug.Log("Detector.Initialize: Starting affdex SDK using (" + Application.platform + ") Platform");

            //use Application.platform to determine platform
            if (RuntimePlatform.WindowsEditor == Application.platform || RuntimePlatform.WindowsPlayer == Application.platform)
            {
                if (IntPtr.Size == 8 && RuntimePlatform.WindowsEditor == Application.platform)
                {
                    rootPath = Path.Combine(rootPath, "x86_64");
                }
                else if (RuntimePlatform.WindowsEditor == Application.platform)
                {
                    rootPath = Path.Combine(rootPath, "x86");
                }
                LoadNativeDll(Path.Combine(rootPath, "affdex-native.dll"));
                nativePlatform = gameObject.AddComponent<WindowsNativePlatform>();
            }
            else if (RuntimePlatform.OSXEditor == Application.platform || RuntimePlatform.OSXPlayer == Application.platform)
            {
                if (!LoadNativeDll(Path.Combine(rootPath, "affdex-native.bundle/Contents/MacOS/affdex-native")))
                    yield break;
                nativePlatform = gameObject.AddComponent<OSXNativePlatform>();
            }
			else if (RuntimePlatform.IPhonePlayer == Application.platform)
			{
				nativePlatform = gameObject.AddComponent<iOSNativePlatform>();
			}
            else if (RuntimePlatform.Android == Application.platform)
            {
                nativePlatform = gameObject.AddComponent<AndroidNativePlatform>();
            }

            //todo: Handle Initialize failure here!
            yield return StartCoroutine(nativePlatform.Initialize(this, discrete ? 1 : 0));

            //find all ON emotions and enable them!
            for (int i = 0; i < System.Enum.GetNames(typeof(Emotions)).Length; i++)
            {
                Emotions targetEmotion = (Emotions)i;
                if (emotions.On(targetEmotion))
                {
                    Debug.Log(targetEmotion + " is on");
                    nativePlatform.SetEmotionState(i, true);
                }
            }

            //find all ON expressions and enable them!
            for (int i = 0; i < System.Enum.GetNames(typeof(Expressions)).Length; i++)
            {
                Expressions targetExpression = (Expressions)i;
                if (expressions.On(targetExpression))
                {
                    Debug.Log(targetExpression + " is on");
                    nativePlatform.SetExpressionState(i, true);
                }
            }

            initialized = true;
        }

        /// <summary>
        /// Create an image from the BGR bytes and save 
        /// </summary>
        /// <param name="bytes">BGR bytes </param>
        /// <param name="w">image width</param>
        /// <param name="h">image height</param>
        private void SampleImage(byte[] bytes, int w, int h)
        {
            Texture2D t = new Texture2D(w, h, TextureFormat.RGB24, false);
            Color32[] colors = new Color32[w * h];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i].b = bytes[(i * 3)];
                colors[i].g = bytes[(i * 3) + 1];
                colors[i].r = bytes[(i * 3) + 2];
            }

            t.SetPixels32(colors);

            byte[] png = t.EncodeToPNG();
            string pngPath = Path.Combine(Application.persistentDataPath, "tmp.png");
            File.WriteAllBytes(pngPath, png);
        }

        /// <summary>
        /// Create an image from a Frame and save 
        /// </summary>
        /// <param name="frame">the Frame</param>
        private void SampleImage(Frame frame)
        {
            Texture2D t = new Texture2D(frame.w, frame.h, TextureFormat.RGB24, false);
            t.SetPixels32(frame.rgba);
            byte[] png = t.EncodeToPNG();
            string pngPath = Path.Combine(Application.persistentDataPath, "tmp.png");
            File.WriteAllBytes(pngPath, png);
        }

        /// <summary>
        /// Loops to listen for events received from native threads
        /// </summary>
        /// <returns></returns>
        private IEnumerator ListenForEvents()
        {
            nativeEvents = new List<NativeEvent>();
            while (enabled)
            {
                yield return new WaitForEndOfFrame();

                if (hasEvents)
                {

                    //dispatch all events stored up!
                    lock (eventLock)
                    {
                        while (nativeEvents.Count > 0)
                        {
                            NativeEvent e = nativeEvents[0];
                            nativeEvents.RemoveAt(0);

                            if (e.type == NativeEventType.ImageResults)
                            {
                                if (Listener != null)
                                    Listener.onImageResults((Dictionary<int, Face>)e.eventData);
                            }
                            else if (e.type == NativeEventType.FaceFound)
                            {
                                if (Listener != null)
                                    Listener.onFaceFound(Time.realtimeSinceStartup, (int)e.eventData);
                            }
                            else if (e.type == NativeEventType.FaceLost)
                            {
                                if (Listener != null)
                                    Listener.onFaceLost(Time.realtimeSinceStartup, (int)e.eventData);
                            }
                        }

                        hasEvents = false;
                    }
                }
            }
        }

    }

    /// <summary>
    /// Detector Input interface
    /// </summary>
    public interface IDetectorInput
    {
        /// <summary>
        /// Texture for the input interface
        /// </summary>
        Texture Texture { get; }
    }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    /// <summary>
    /// DLL loader helper
    /// </summary>
    internal static class NativeMethods
    {
        [DllImport ("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr LoadLibrary (
            string lpFileName
        );

        
        [DllImport ("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern int FreeLibrary (
            string lpFileName
        );
    }
    
#else
    /// <summary>
    /// DLL loader helper
    /// </summary>
    internal class NativeMethods
    {
        
        public static IntPtr LoadLibrary (string fileName)
        {
            IntPtr retVal = dlopen (fileName, RTLD_NOW);
            var errPtr = dlerror ();
            if (errPtr != IntPtr.Zero) {
                Debug.LogError (Marshal.PtrToStringAnsi (errPtr));
            }
            return retVal;
        }

        public static void FreeLibrary (IntPtr handle)
        {
            dlclose (handle);
        }

        const int RTLD_NOW = 2;

        [DllImport ("libdl.dylib")]
        private static extern IntPtr dlopen (String fileName, int flags);

        [DllImport ("libdl.dylib")]
        private static extern IntPtr dlsym (IntPtr handle, String symbol);

        [DllImport ("libdl.dylib")]
        private static extern int dlclose (IntPtr handle);

        [DllImport ("libdl.dylib")]
        private static extern IntPtr dlerror ();
    }
#endif
}
    
