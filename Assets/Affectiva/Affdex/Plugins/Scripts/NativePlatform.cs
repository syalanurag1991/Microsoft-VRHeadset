using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;
using System.Collections;

namespace Affdex
{
    internal enum NativeEventType
    {
        ImageResults,
        FaceFound,
        FaceLost
    }

    internal struct NativeEvent
    {
        public NativeEventType type;
        public object eventData;

        public NativeEvent(NativeEventType t, object data)
        {

            type = t;
            eventData = data;
        }
    }

    public abstract class NativePlatform : MonoBehaviour
    {
        protected static Detector detector;

        //Free these when platform closes!
        protected GCHandle h1, h2, h3; //handles to unmanaged function pointer callbacks

        /// <summary>
        /// Initialize the detector.  Creates the instance for later calls
        /// </summary>
        /// <param name="discrete"></param>
        /// <param name="detector">Core detector object.  Handles all communication with the native APIs.</param>
        public abstract IEnumerator Initialize(Detector detector, int discrete);

        /// <summary>
        /// Start the detector
        /// </summary>
        /// <returns>Non-zero error code</returns>
        public abstract int StartDetector();

        /// <summary>
        /// Stop the detector
        /// </summary>
        public abstract void StopDetector();

        /// <summary>
        /// Enable or disable an expression
        /// </summary>
        /// <param name="expression">ID of the expression to set the state of</param>
        /// <param name="state">ON/OFF state for the expression</param>
        public abstract void SetExpressionState(int expression, bool state);

        /// <summary>
        /// Get the ON/OFF state of the expression
        /// </summary>
        /// <param name="expression">ID of the expression</param>
        /// <returns>0/1 for OFF/ON state</returns>
        bool GetExpressionState(int expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Enable or disable an emotion
        /// </summary>
        /// <param name="emotion">ID of the emotion to set the state of</param>
        /// <param name="state">ON/OFF state for the emotion</param>
        public abstract void SetEmotionState(int emotion, bool state);

        /// <summary>
        /// Get the ON/OFF state of the emotion
        /// </summary>
        /// <param name="emotion">emotion id to get the state of</param>
        /// <returns>0/1 for OFF/ON state</returns>
        bool GetEmotionState(int emotion)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Process a single frame of data
        /// </summary>
        /// <param name="rgba">Representation of RGBA colors in 32 bit format.</param>
        /// <param name="width">Width of the frame. Value has to be greater than zero</param>
		/// <param name="height">Height of the frame. Value has to be greater than zero</param>
		/// <param name="angle">Rotation angle of the frame, expressed as positive clockwise angle. Value must be one of {0, 90, 180, 270} </param>
        /// <param name="timestamp">The timestamp of the frame (in seconds). Can be used as an identifier of the frame.  If you use Time.timeScale to pause and use the same time units then you will not be able to process frames while paused.</param>
        public abstract void ProcessFrame(byte[] rgba, int width, int height, Frame.Orientation orientation, float timestamp);


        /// <summary>
        /// Notify the native plugin to release memory and cleanup
        /// </summary>
        public abstract void Release();

        public void onFaceFound(Int32 id)
        {
            detector.AddEvent(new NativeEvent(NativeEventType.FaceFound, id));
        }

        public void onFaceLost(Int32 id)
        {

            detector.AddEvent(new NativeEvent(NativeEventType.FaceLost, id));
        }

        /// <summary>
        /// ImageResults callback from native plugin!
        /// </summary>
        /// <param name="faceData">Platform-specific pointer to image results</param>
        public void onImageResults(IntPtr faceData)
        {
            System.Collections.Generic.Dictionary<int, Face> faces = new System.Collections.Generic.Dictionary<int, Face>();
            if (faceData != IntPtr.Zero)
            {
                try
                {
                    //todo: Face ID might not always be zero, or there might be more faces!!!
                    FaceData f = (FaceData)Marshal.PtrToStructure(faceData, typeof(FaceData));
                    faces[0] = new Face(f);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message + " " + e.StackTrace);
                }
            }

            detector.AddEvent(new NativeEvent(NativeEventType.ImageResults, faces));
        }
    }

}
    