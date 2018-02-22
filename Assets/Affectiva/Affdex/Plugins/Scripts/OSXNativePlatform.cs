using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;

namespace Affdex
{
    internal class OSXNativePlatform : NativePlatform
    {
        // declarations for listeners
        [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
        delegate void ImageResults (IntPtr i);

        [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
        delegate void FaceResults (Int32 i);

        [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
        delegate void LogCallback (IntPtr l);

        // declarations for the exported C functions in the C/C++ SDK for Affdex
        [DllImport ("affdex-native")]
        private static extern int affdexRegisterListeners (IntPtr handle,
            [MarshalAs (UnmanagedType.FunctionPtr)] ImageResults imageCallback,
            [MarshalAs (UnmanagedType.FunctionPtr)] FaceResults foundCallback, 
            [MarshalAs (UnmanagedType.FunctionPtr)] FaceResults lostCallback);

        [DllImport ("affdex-native")]
        private static extern int affdexProcessFrame (IntPtr handle, IntPtr rgba, Int32 w, Int32 h, Int32 orientation, float timestamp);

        [DllImport ("affdex-native")]
        private static extern int affdexStart (IntPtr handle);

        [DllImport ("affdex-native")]
        private static extern void affdexRelease (IntPtr handle);

        [DllImport ("affdex-native")]
        private static extern int affdexStop (IntPtr handle);

        [DllImport ("affdex-native")]
        private static extern void affdexSetExpressionState (IntPtr handle, int expression, int state);

        [DllImport ("affdex-native")]
        private static extern void affdexSetEmotionState (IntPtr handle, int emotion, int state);

        [DllImport ("affdex-native")]
        private static extern IntPtr affdexInitialize (int discrete, string affdexDataPath);

        private IntPtr nativeHandle;

        public override IEnumerator Initialize(Detector detector, int discrete)
        {
            OSXNativePlatform.detector = detector;
            String adP = Application.streamingAssetsPath;
            String affdexDataPath = Path.Combine(adP, "affdex-data-osx"); 
            int status = 0;

            Debug.Log("Initializing detector");
            try 
            {
                nativeHandle = affdexInitialize(discrete, affdexDataPath);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            Debug.Log("Initialized detector: " + status);
            
            FaceResults faceFound = new FaceResults(this.onFaceFound);
            FaceResults faceLost = new FaceResults(this.onFaceLost);
            ImageResults imageResults = new ImageResults(this.onImageResults);
            
            h1 = GCHandle.Alloc(faceFound, GCHandleType.Pinned);
            h2 = GCHandle.Alloc(faceLost, GCHandleType.Pinned);
            h3 = GCHandle.Alloc(imageResults, GCHandleType.Pinned);
            
            status = affdexRegisterListeners(nativeHandle, imageResults, faceFound, faceLost);
            Debug.Log("Registered listeners: " + status);
            yield break;
        }

        public override void ProcessFrame(byte[] rgba, int w, int h, Frame.Orientation orientation, float timestamp)
        {
            try
            {
                IntPtr addr = Marshal.AllocHGlobal(rgba.Length);
                Marshal.Copy(rgba, 0, addr, rgba.Length);
                // native call
                affdexProcessFrame(nativeHandle, addr, w, h, (int)orientation, Time.realtimeSinceStartup);
                Marshal.FreeHGlobal(addr);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + " " + e.StackTrace);
            }
        }

        public override void SetExpressionState(int expression, bool state)
        {
            int intState = (state) ? 1 : 0;
            // native call
            affdexSetExpressionState(nativeHandle, expression, intState);
            // Debug.Log("Expression " + expression + " set to " + state);
        }

        public override void SetEmotionState(int emotion, bool state)
        {
            int intState = (state) ? 1 : 0;
            // native call
            affdexSetEmotionState(nativeHandle, emotion, intState);
            //  Debug.Log("Emotion " + emotion + " set to " + state);
        }
        
        public override int StartDetector()
        {
            // native call
            return affdexStart(nativeHandle);
        }
        
        public override void StopDetector()
        {
            // native call
            affdexStop(nativeHandle);
        }

        public override void Release()
        {
            // native call
            affdexRelease(nativeHandle);
            h1.Free();
            h2.Free();
            h3.Free();
        }
    }
}