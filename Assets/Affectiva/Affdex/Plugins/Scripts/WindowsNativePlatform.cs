using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;
using System.Collections;

namespace Affdex
{
    internal class WindowsNativePlatform : NativePlatform
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ImageResults(IntPtr i);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void FaceResults(Int32 i);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void LogCallback(IntPtr l);
       
        [DllImport("affdex-native")]
        private static extern int affdexRegisterListeners(IntPtr handle,
            [MarshalAs(UnmanagedType.FunctionPtr)] ImageResults imageCallback,
            [MarshalAs(UnmanagedType.FunctionPtr)] FaceResults foundCallback, 
            [MarshalAs(UnmanagedType.FunctionPtr)] FaceResults lostCallback);

        [DllImport("affdex-native")]
        private static extern int affdexProcessFrame(IntPtr handle, IntPtr rgba, Int32 w, Int32 h, Int32 orientation, float timestamp);

        [DllImport("affdex-native")]
        private static extern int affdexStart(IntPtr handle);

        [DllImport("affdex-native")]
        private static extern void affdexRelease(IntPtr handle);

        [DllImport("affdex-native")]
        private static extern int affdexStop(IntPtr handle);

        [DllImport("affdex-native")]
        private static extern void affdexSetExpressionState(IntPtr handle, int expression, int state);

        [DllImport("affdex-native")]
        private static extern void affdexSetEmotionState(IntPtr handle, int emotion, int state);

        [DllImport("affdex-native")]
        private static extern IntPtr affdexInitialize(int discrete, string affdexDataPath);

        [DllImport("affdex-native")]
        private static extern string affdexGetLastErrorMessage();

        private IntPtr nativeHandle;

        public override IEnumerator Initialize(Detector detector, int discrete)
        {
            WindowsNativePlatform.detector = detector;

            //load our lib!
            string affdexDataPath = Application.streamingAssetsPath + "/affdex-data-3";
            affdexDataPath = affdexDataPath.Replace('/', '\\');
            nativeHandle = affdexInitialize(discrete, affdexDataPath);

            FaceResults faceFound = new FaceResults(this.onFaceFound);
            FaceResults faceLost = new FaceResults(this.onFaceLost);
            ImageResults imageResults = new ImageResults(this.onImageResults);

            h1 = GCHandle.Alloc(faceFound, GCHandleType.Pinned);
            h2 = GCHandle.Alloc(faceLost, GCHandleType.Pinned);
            h3 = GCHandle.Alloc(imageResults, GCHandleType.Pinned);

            int status = affdexRegisterListeners(nativeHandle, imageResults, faceFound, faceLost);
            Debug.Log("Registered listeners: " + status);
            yield break;
        }

        public override void ProcessFrame(byte[] rgba, int w, int h, Frame.Orientation orientation, float timestamp)
        {
            try
            {
                IntPtr addr = Marshal.AllocHGlobal(rgba.Length);

                Marshal.Copy(rgba, 0, addr, rgba.Length);
                int result = affdexProcessFrame(nativeHandle, addr, w, h, (int)orientation, timestamp);
                if (result != 1)
                {
                    Debug.Log(affdexGetLastErrorMessage());
                }
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
            Debug.Log("WindowsNativePlatform.StartDetector");
            int result = affdexStart(nativeHandle);
            if (result != 1)
            {
                Debug.Log(affdexGetLastErrorMessage());
            }
            return result;
        }

        public override void StopDetector()
        {
            affdexStop(nativeHandle);
        }

        public override void Release()
        {
            affdexRelease(nativeHandle);
            h1.Free();
            h2.Free();
            h3.Free();
        }
    }
}
