using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;

namespace Affdex
{
    public class MonoPInvokeCallbackAttribute : System.Attribute
    {
        private Type type;
        public MonoPInvokeCallbackAttribute( Type t ) { type = t; }
    }

    internal class iOSNativePlatform : NativePlatform
    {
        const string dll = "__Internal";

        private static FaceResults mFaceFound;
        private static FaceResults mFaceLost;
        private static ImageResults mImageResults;

        delegate void ImageResultsCallback(IntPtr i);
        delegate void FaceResultsCallback(Int32 i);

        [MonoPInvokeCallbackAttribute (typeof(ImageResultsCallback))]
        static void ImageCallbackProc (IntPtr i) {
//            Debug.Log ("ImageCallbackProc: Image at " + i);
            mImageResults (i);
        }

        [MonoPInvokeCallbackAttribute (typeof(FaceResultsCallback))]
        static void FaceFoundCallbackProc (Int32 i) {
            Debug.Log ("FaceFoundCallbackProc: found face " + i);
//          detector.AddEvent(new NativeEvent(NativeEventType.FaceFound, i));
            mFaceFound (i);
        }

        [MonoPInvokeCallbackAttribute (typeof(FaceResultsCallback))]
        static void FaceLostCallbackProc (Int32 i) {
            Debug.Log ("FaceLostCallbackProc: lost face " + i);
//          detector.AddEvent(new NativeEvent(NativeEventType.FaceLost, i));
            mFaceLost (i);
        }

        // declarations for listeners
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ImageResults(IntPtr i);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void FaceResults(Int32 i);

        // declarations for the exported C functions in the C/C++ SDK for Affdex
        [DllImport(dll)]
        private static extern int affdexRegisterListeners (IntPtr handle,
                   [MarshalAs (UnmanagedType.FunctionPtr)] ImageResults imageCallback,
                   [MarshalAs (UnmanagedType.FunctionPtr)] FaceResults foundCallback, 
                   [MarshalAs (UnmanagedType.FunctionPtr)] FaceResults lostCallback);

        [DllImport(dll)]
        private static extern int affdexProcessFrame (IntPtr handle, IntPtr rgba, Int32 w, Int32 h, Int32 orientation, float timestamp);

        [DllImport(dll)]
        private static extern int affdexStart(IntPtr handle);

        [DllImport(dll)]
        private static extern void affdexRelease(IntPtr handle);

        [DllImport(dll)]
        private static extern int affdexStop(IntPtr handle);

        [DllImport(dll)]
        private static extern void affdexSetExpressionState(IntPtr handle, int expression, int state);

        [DllImport(dll)]
        private static extern void affdexSetEmotionState(IntPtr handle, int emotion, int state);
                
        [DllImport(dll)]
        private static extern IntPtr affdexInitialize(int discrete, string affdexDataPath);

        private IntPtr nativeHandle;

        public override IEnumerator Initialize(Detector detector, int discrete)
        {
            iOSNativePlatform.detector = detector;
            String adP = Application.streamingAssetsPath;
            String affdexDataPath = Path.Combine(adP, "affdex-data-ios");
            int status = 0;

            try 
            {
                nativeHandle = affdexInitialize(discrete, affdexDataPath);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            Debug.Log("Initialized detector: " + status);

            mFaceFound = new FaceResults(this.onFaceFound);
            mFaceLost = new FaceResults(this.onFaceLost);
            mImageResults = new ImageResults(this.onImageResults);

            Debug.Log("mFaceFound = " + mFaceFound + ", mFaceLost = " + mFaceLost + ", mImageResults = " + mImageResults);  // SCP - TEMPORARY

            status = affdexRegisterListeners(nativeHandle, ImageCallbackProc, FaceFoundCallbackProc, FaceLostCallbackProc);
            Debug.Log("Registered listeners: " + status);
            yield break;
        }

        public override void ProcessFrame(byte[] rgba, int w, int h, Frame.Orientation orientation, float timestamp)
        {
            try
            {
                IntPtr addr = Marshal.AllocHGlobal(rgba.Length);
                
                Marshal.Copy(rgba, 0, addr, rgba.Length);
                
                affdexProcessFrame(nativeHandle, addr, w, h, (int)orientation, Time.realtimeSinceStartup);
                
                Marshal.FreeHGlobal(addr);
                addr = IntPtr.Zero;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + " " + e.StackTrace);
            }
        }

        public override void SetExpressionState(int expression, bool state)
        {
            int intState = (state) ? 1 : 0;
            affdexSetExpressionState(nativeHandle, expression, intState);
            // Debug.Log("Expression " + expression + " set to " + state);
        }

        public override void SetEmotionState(int emotion, bool state)
        {
            int intState = (state) ? 1 : 0;
            affdexSetEmotionState(nativeHandle, emotion, intState);
            //  Debug.Log("Emotion " + emotion + " set to " + state);
        }
        
        public override int StartDetector()
        {
            return affdexStart(nativeHandle);
        }

        public override void StopDetector()
        {
            affdexStop(nativeHandle);
        }

        public override void Release()
        {
            affdexRelease(nativeHandle);
        }
    }
}
